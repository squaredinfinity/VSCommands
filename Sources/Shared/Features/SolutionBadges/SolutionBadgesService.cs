using SquaredInfinity.Foundation;
using SquaredInfinity.Foundation.Settings;
using SquaredInfinity.Foundation.Win32Api;
using SquaredInfinity.VSCommands.Foundation.VisualStudioEvents;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows;
using SquaredInfinity.Foundation.Extensions;
using System.Windows.Interop;
using EnvDTE;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.IO;
using System.Collections.Concurrent;
using EnvDTE80;
using SquaredInfinity.VSCommands.Foundation.Settings;

namespace SquaredInfinity.VSCommands.Features.SolutionBadges
{
    public class SolutionBadgesService
    {
        const string KnownSolutionsCacheId = @"SolutionBadges.KnownSolutionsCache";

        int FailureCount = 0;
        readonly int MaxFailureCount = 13;

        readonly object Sync = new object();

        Bitmap CurrentBadgeIconicBitmap;
        Bitmap CurrentBadgeLivePreviewBitmap;

        IntPtr _mainWindowHandle = IntPtr.Zero;
        IntPtr MainWindowHandle
        {
            get
            {
                if (_mainWindowHandle == IntPtr.Zero)
                {
                    lock (Sync)
                    {
                        if (Application.Current != null && Application.Current.MainWindow != null && _mainWindowHandle == IntPtr.Zero)
                        {
                            var interopHelper = new WindowInteropHelper(Application.Current.MainWindow);

                            _mainWindowHandle = interopHelper.EnsureHandle();
                        }
                    }
                }

                return _mainWindowHandle;
            }
        }

        protected ISettingsService SettingsService { get; private set; }
        protected IVisualStudioEventsService VisualStudioEventsService { get; private set; }
        protected IServiceProvider ServiceProvider { get; private set; }

        InvocationThrottle RefreshThrottle = new InvocationThrottle(min: TimeSpan.FromMilliseconds(250), max:TimeSpan.FromSeconds(1));

        public SolutionBadgesService(
            ISettingsService settingsService, 
            IVisualStudioEventsService visualStudioEventsService,
            IServiceProvider serviceProvider)
        {
            this.SettingsService = settingsService;
            this.VisualStudioEventsService = visualStudioEventsService;
            this.ServiceProvider = serviceProvider;

            VisualStudioEventsService.AfterSolutionOpened += (s, e) => RequestRefresh();
            VisualStudioEventsService.AfterSolutionClosed += (s, e) => RequestRefresh();
            VisualStudioEventsService.AfterActiveDocumentChanged += (s, e) => RequestRefresh();
            VisualStudioEventsService.AfterActiveSolutionConfigChanged += (s, e) => RequestRefresh();
            VisualStudioEventsService.AfterDebuggerEnterRunMode += (s, e) => RequestRefresh();
            VisualStudioEventsService.AfterDebuggerEnterDesignMode += (s, e) => RequestRefresh();
            VisualStudioEventsService.AfterDebuggerEnterBreakMode += (s, e) => RequestRefresh();

            VisualStudioEventsService.RegisterVisualStudioUILoadedAction(() => InitializeWin32Hooks());
        }

        void RequestRefresh()
        {
            RefreshThrottle.Invoke(Refresh);
        }

        void Refresh(CancellationToken ct)
        {
            InvalidateCurrentBadge();
        }

        void InvalidateCurrentBadge()
        {
            if (MainWindowHandle == IntPtr.Zero)
                return;

            dwmapi.DwmInvalidateIconicBitmaps(MainWindowHandle);
        }

        void InitializeWin32Hooks()
        {
            user32.ChangeWindowMessageFilter((int)dwmapi.WM_Messages.WM_DWMSENDICONICTHUMBNAIL, user32.ChangeWindowMessageFilterFlags.MSGFLT_ADD);
            user32.ChangeWindowMessageFilter((int)dwmapi.WM_Messages.WM_DWMSENDICONICLIVEPREVIEWBITMAP, user32.ChangeWindowMessageFilterFlags.MSGFLT_ADD);

            HwndSource hwndSource = HwndSource.FromDependencyObject(Application.Current.MainWindow) as HwndSource;
            hwndSource.AddHook(HwndSourceHook);

            //if (Config.IsEnabled)
            //{
                dwmapi.EnableCustomWindowPreview(MainWindowHandle);
            //}
        }

        IntPtr HwndSourceHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            try
            {
                // Check if a System Command has been executed (this is for title menu click support)
                if (msg == WM.SYSCOMMAND)
                {
                    //// Execute the appropriate code for the System Menu item that was clicked
                    //switch (wParam.ToInt32())
                    //{
                    //    case 1313:
                    //        var vm = Container.Resolve<ChangeWindowTitleViewModel>();
                    //        vm.View = Container.ResolveView<ChangeWindowTitleViewModel>();

                    //        UIService.ShowDialog(vm);

                    //        if (vm.InteractionOutcome == UserInteractionOutcome.OtherSuccess)
                    //        {
                    //            SessionBadgeTitle = vm.BadgeTitle;
                    //            SessionBadgeSubtitle = vm.Subtitle;
                    //        }
                    //        else
                    //        {
                    //            SessionBadgeTitle = string.Empty;
                    //            SessionBadgeSubtitle = string.Empty;
                    //        }

                    //        CreateBadgeForCurrentSolution();

                    //        handled = true;
                    //        break;
                    //}
                }
                else if (msg == (int)dwmapi.WM_Messages.WM_DWMSENDICONICTHUMBNAIL)
                {
                    lock (Sync)
                    {
                        CreateBadgeForCurrentSolution();

                        if (CurrentBadgeIconicBitmap != null)
                        {
                            var hBitmap = CurrentBadgeIconicBitmap.GetHbitmap();

                            try
                            {
                                dwmapi.DwmSetIconicThumbnail(MainWindowHandle, hBitmap, 0);
                            }
                            finally
                            {
                                gdi32.DeleteObject(hBitmap);
                            }
                        }
                    }
                }
                else if (msg == (int)dwmapi.WM_Messages.WM_DWMSENDICONICLIVEPREVIEWBITMAP)
                {
                    lock (Sync)
                    {
                        RefreshLivePreviewBitmap();

                        if (CurrentBadgeLivePreviewBitmap != null)
                        {
                            var offset = new dwmapi.NativePoint(8, 8); // TODO: this may need to be 0,0 in VS 10 (because the window does not use metro style)

                            var hBitmap = CurrentBadgeLivePreviewBitmap.GetHbitmap();

                            try
                            {
                                dwmapi.DwmSetIconicLivePreviewBitmap(
                                    MainWindowHandle,
                                    hBitmap,
                                    ref offset,
                                    0); // todo: this may be needed (drawing frame) in VS 10
                            }
                            finally
                            {
                                gdi32.DeleteObject(hBitmap);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.TryAddContextData("WM MSG", () => msg);

                // todo: log exception


                // NOTE:    in very rare cases creating solution badge may keep failing (but system will keep sending WM to retry leading to endless spiral of misery)
                //          i was unable to find out the exact cause, but it may have been caused by low memory or other system-wide issues
                //          to prevent such scenarios Solution Badge will be disabled after several failed attempts.

                Interlocked.Increment(ref FailureCount);

                if (FailureCount >= MaxFailureCount)
                {
                    //Logger.TraceWarning(() => "Solution Badge has failed {0} times and will be disabled.");

                    user32.ChangeWindowMessageFilter(
                        (int)dwmapi.WM_Messages.WM_DWMSENDICONICTHUMBNAIL,
                        user32.ChangeWindowMessageFilterFlags.MSGFLT_REMOVE);

                    user32.ChangeWindowMessageFilter(
                        (int)dwmapi.WM_Messages.WM_DWMSENDICONICLIVEPREVIEWBITMAP,
                        user32.ChangeWindowMessageFilterFlags.MSGFLT_REMOVE);

                    dwmapi.EnableCustomWindowPreview(MainWindowHandle, false);

                    RequestRefresh();
                }
            }

            return IntPtr.Zero;
        }

        SolutionBadgeView GetSolutionBadgeView()
        {
            var view = new SolutionBadgeView();

            // NOTE:    this will be used outside of application Visual Tree
            //          must initialize manually
            view.BeginInit();
            view.EndInit();
            view.ApplyTemplate();

            return view;
        }

        void CreateBadgeForCurrentSolution()
        {
            var view = GetSolutionBadgeView();

            RefreshIconicBitmap(view);
        }

        void CreateBadgeForCurrentSolution(
            string solutionPath,
            string solutionName,
            string branchName,
            string activeDocumentName,
            dbgDebugMode debugMode)
        {
            try
            {
                var view = GetSolutionBadgeView();

                //CreateBadgeForSolution(solutionPath, solutionName, branchName, activeDocumentName, debugMode, view);

                RefreshIconicBitmap(view);
               // RefreshStartPageBitmap(view);
            }
            catch (Exception ex)
            {
                ex.TryAddContextData("solution path", () => solutionPath);
                ex.TryAddContextData("solution name", () => solutionName);
                ex.TryAddContextData("debug mode", () => debugMode);
                throw;
            }
        }
    


        void RefreshIconicBitmap(SolutionBadgeView view)
        {
            lock (Sync)
            {
                if (CurrentBadgeIconicBitmap != null)
                {
                    CurrentBadgeIconicBitmap.Dispose();
                }

                CurrentBadgeIconicBitmap = PrepareBadgeViewBitmap(view, new System.Windows.Size(196, 106));
            }
        }

        Bitmap PrepareBadgeViewBitmap(SolutionBadgeView badgeView, System.Windows.Size size)
        {
            try
            {
                (badgeView).Measure(size);
                (badgeView).Arrange(new System.Windows.Rect(size));
                (badgeView).Refresh();

                RenderTargetBitmap targetBitmap = new RenderTargetBitmap((int)size.Width, (int)size.Height, 96.0, 96.0, PixelFormats.Default);

                targetBitmap.Render(badgeView);

                BmpBitmapEncoder encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(targetBitmap));

                MemoryStream ms = new MemoryStream();

                encoder.Save(ms);

                ms.Flush();

                var badgeBitmap = System.Drawing.Image.FromStream(ms) as Bitmap;

                return badgeBitmap;
            }
            catch (Exception ex)
            {
                ex.TryAddContextData("size", () => size);
                throw;
            }
        }

        void RefreshLivePreviewBitmap()
        {
            lock (Sync)
            {
                // release old resources if needed
                if (CurrentBadgeLivePreviewBitmap != null)
                {
                    CurrentBadgeLivePreviewBitmap.Dispose();
                    CurrentBadgeLivePreviewBitmap = null;
                }

                // create bitmap from main window to show in live preview
                var mainWindow = Application.Current.MainWindow;

                RenderTargetBitmap bitmap = new RenderTargetBitmap((int)mainWindow.ActualWidth, (int)mainWindow.ActualHeight, 96, 96, PixelFormats.Pbgra32);

                bitmap.Render(mainWindow);

                BmpBitmapEncoder encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmap));

                using (MemoryStream ms = new MemoryStream())
                {
                    encoder.Save(ms);

                    ms.Flush();

                    CurrentBadgeLivePreviewBitmap = System.Drawing.Image.FromStream(ms) as Bitmap;
                }
            }
        }
        
        void ClearPlaceholderMappings()
        {
            foreach (var key in PlaceholderMappings.Keys)
                PlaceholderMappings[key] = "";
        }

        readonly Dictionary<string, string> PlaceholderMappings = new Dictionary<string, string>();


        CurrentVSStateInfo GetCurrentVSState()
        {
            var result = new CurrentVSStateInfo();

            Dictionary<string, object> properties = new Dictionary<string, object>();

            ClearPlaceholderMappings();

            var dte2 = ServiceProvider.GetDte2();

            var solution = dte2.Solution;

            if (solution != null)
            {
                var solutionBuild = solution.SolutionBuild;

                if (solutionBuild != null)
                {
                    var activeConfig = solutionBuild.ActiveConfiguration as SolutionConfiguration2;

                    if (activeConfig != null)
                    {
                        properties["sln:activeConfig"] = activeConfig.Name;
                        properties["sln:activePlatform"] = activeConfig.PlatformName;
                    }
                }
            }

            var activeDocument = dte2.GetActiveDocument();

            if (activeDocument == null
                || activeDocument.FullName.IsNullOrEmpty())
            {
                properties["activeDocument:fileName"] = "";
                properties["activeDocument:fullPath"] = "";

                // todo: not supported at the moment, would need to get notifications when new tool window is open for it to work nice
                // no document is open, try to get open window name (e.g. start page)
                //try
                //{
                //    var activeWindow = dte2.ActiveWindow; // this may throw exception if there is no active window (?)

                //    if (activeWindow != null
                //        && !activeWindow.Linkable) // we don't want to process docked windows such as solution explorer
                //    {
                //        activeDocumentName = activeWindow.Caption;
                //    }
                //}
                //catch (Exception ex)
                //{
                //    Logger.DiagnosticOnlyLogException(ex);
                //}
            }
            else
            {
                var active_document_path = activeDocument.Name;

                properties["activeDocument:fullPath"] = active_document_path;
                properties["activeDocument:fileName"] = Path.GetFileName(activeDocument.Name);
            }

            var solution_full_path = solution.FullName;

            if (solution_full_path == string.Empty)
            {
                properties["sln:isOpen"] = false;
            }
            else
            {
                properties["sln:isOpen"] = true;

                // todo:    allow user to override solution name
                //var custom_solution_name = SettingsService.GetSetting<string>("SolutionBadges.SolutionName", VscSettingScope.Solution, () => null);

                properties["sln:fullPath"] = solution_full_path;
                properties["sln:fileName"] = Path.GetFileNameWithoutExtension(solution_full_path);

                // todo:    this could perhaps happen on a higher level,
                //          where decision has to be made if to use this or custom name etc.
                    //.Replace('.', ' ')
                    //.Replace('-', ' ')
                    //.Replace('_', ' ')
                    //.SplitCamelCase();
                    //.SplitCamelCase(splitNumbers: false);
            }

            var branch_name_regex = SettingsService.GetSetting<string>("SolutionBadges.BranchNameRegex", VscSettingScope.All, () => null);

            // Set Branch Name (Subtitle of the badge)
            if (!branch_name_regex.IsNullOrEmpty())
            {
                branchName = TryConstructFromPattern(Config.BranchNamePattern.CurrentValue, Config.BranchNameRegex.CurrentValue, sozlutionPath);

                var solutionFileName = Path.GetFileNameWithoutExtension(solutionPath).ToLower();
                var lowerCaseTrimmedBranchName = branchName.ToLower().Trim();

                // do not use branch name if it is same as solution name
                if (lowerCaseTrimmedBranchName != solutionFileName
                    && !lowerCaseTrimmedBranchName.IsIn("src", "source", "sources", "src branch", "source branch", "sources branch")
                    && lowerCaseTrimmedBranchName.Replace("branch", string.Empty).Trim() != solutionFileName)
                {
                }
                else
                {
                    branchName = "";
                }

                PlaceholderMappings["vsc:branchName"] = branchName;
            }

            branchName = TrimSeparatorCharactes(branchName);

            mainWindowTitle = "";

            // set main window title
            if (Config.ShouldChangeMainWindowTitle
                && !Config.MainWindowTitlePattern.CurrentValue.IsNullOrEmpty())
            {
                mainWindowTitle = TryConstructFromPattern(Config.MainWindowTitlePattern, Config.BranchNameRegex.CurrentValue, solutionPath);
                mainWindowTitle = ReplaceVariablePlaceholders(mainWindowTitle, branchName);
                mainWindowTitle = TrimSeparatorCharactes(mainWindowTitle);
            }

            solutionExplorerWindowTitle = "";

            // set solution explorer title
            if (Config.ShouldChangeSolutionExplorerWindowTitle
                && !Config.SolutionExplorerTitlePattern.CurrentValue.IsNullOrEmpty())
            {
                solutionExplorerWindowTitle = TryConstructFromPattern(Config.SolutionExplorerTitlePattern, Config.BranchNameRegex.CurrentValue, solutionPath);
                solutionExplorerWindowTitle = ReplaceVariablePlaceholders(solutionExplorerWindowTitle, branchName);
                solutionExplorerWindowTitle = TrimSeparatorCharactes(solutionExplorerWindowTitle, trimStart: false);
            }

            var debugger = dte2.Debugger;
            debugMode = debugger.CurrentMode;

            if (!SessionBadgeTitle.IsNullOrEmpty())
            {
                solutionName = SessionBadgeTitle;
                branchName = SessionBadgeSubtitle;

                mainWindowTitle = SessionBadgeTitle + " • " + SessionBadgeSubtitle;
            }

            try
            {
                var knownSolutionsCache = CacheService.GetOrCreate(KnownSolutionsCacheId, () => new ContainerCacheItem());

                var cacheItem =
                    knownSolutionsCache.GetOrCreate<Cache.SolutionDetailsCache>(
                    solutionPath,
                    () => new Cache.SolutionDetailsCache { SolutionFullPath = solutionPath });

                if (!string.IsNullOrWhiteSpace(branchName) && (cacheItem.BranchName != branchName || cacheItem.SolutionName != solutionName))
                {
                    cacheItem.BranchName = branchName;
                    cacheItem.SolutionName = solutionName;

                    CacheService.AddOrUpdate(KnownSolutionsCacheId, knownSolutionsCache);
                }
            }
            catch (Exception ex)
            {
                Logger.DiagnosticOnlyLogException(ex);
            }
        }
    }
}
