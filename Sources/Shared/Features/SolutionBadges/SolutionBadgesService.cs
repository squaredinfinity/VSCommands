using SquaredInfinity.Foundation;
using SquaredInfinity.Foundation.Extensions;
using SquaredInfinity.Foundation.Settings;
using SquaredInfinity.Foundation.Win32Api;
using SquaredInfinity.VSCommands.Foundation.VisualStudioEvents;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using EnvDTE;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.IO;
using System.Collections.Concurrent;
using EnvDTE80;
using SquaredInfinity.VSCommands.Foundation.Settings;
using System.Text.RegularExpressions;
using SquaredInfinity.VSCommands.Features.SolutionBadges.SourceControl;
using System.ComponentModel.Composition;
using SquaredInfinity.VSCommands.Foundation;
using System.Diagnostics;

namespace SquaredInfinity.VSCommands.Features.SolutionBadges
{
    [Export(typeof(ISolutionBadgesService))]
    public class SolutionBadgesService : ISolutionBadgesService
    {
        [ImportMany(typeof(ISourceControlInfoProvider))]
        IEnumerable<ExportFactory<ISourceControlInfoProvider>> SourceControlInfoProvidersFactories = null;

        IEnumerable<ISourceControlInfoProvider> SourceControlInfoProviders { get; set; }

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
#if DEBUG
                if (!UIService.IsUIThread)
                    throw new InvalidOperationException("This property can only be accessed from UI thread.");
#endif

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

        readonly IVscUIService UIService;
        readonly IVscSettingsService SettingsService;
        readonly IVisualStudioEventsService VisualStudioEventsService;
        readonly IServiceProvider ServiceProvider;

        InvocationThrottle RefreshThrottle = new InvocationThrottle(min: TimeSpan.FromMilliseconds(500), max:TimeSpan.FromSeconds(2));

        readonly IDictionary<string, object> CurrentThemeInfo = new Dictionary<string, object>();
        readonly IDictionary<string, object> CurrentBadgeInfo = new Dictionary<string, object>();

        public SolutionBadgesService(
            IVscUIService uiService,
            IVscSettingsService settingsService, 
            IVisualStudioEventsService visualStudioEventsService,
            IServiceProvider serviceProvider)
        {
            this.UIService = uiService;
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

            VisualStudioEventsService.RegisterVisualStudioUILoadedAction(InitializeWin32Hooks);
            VisualStudioEventsService.RegisterVisualStudioUILoadedAction(CollectThemeInfo);

            visualStudioEventsService.AfterVisualStudioThemeChanged += VisualStudioEventsService_AfterVisualStudioThemeChanged;
        }

        void VisualStudioEventsService_AfterVisualStudioThemeChanged(object sender, EventArgs e)
        {
            CollectThemeInfo();
            InvalidateCurrentBadge();
        }

        void CollectThemeInfo()
        {
            var result = new Dictionary<string, object>();

            System.Windows.Media.Color accentColor = Colors.White;

            //if (Config.ShouldUseVisualStudioTheme)
            {
                var theme_color = ThemeInfo.GetThemeColor();

                accentColor = theme_color;

                result.AddOrUpdate(KnownProperties.AccentColor, accentColor);
            }

            CurrentThemeInfo.Clear();
            CurrentThemeInfo.AddOrUpdateFrom((IDictionary<string,object>)result);
        }

        public void Initialise()
        {
            var sourceControlInfoProviders = new List<ISourceControlInfoProvider>();

            foreach(var factory in SourceControlInfoProvidersFactories)
            {
                var provider = factory.CreateExport().Value;
                provider.CurrentBadgeRefreshRequested += Provider_CurrentBadgeRefreshRequested;

                sourceControlInfoProviders.Add(provider);
            }

            this.SourceControlInfoProviders = sourceControlInfoProviders;
        }

        void Provider_CurrentBadgeRefreshRequested(object sender, EventArgs e)
        {
            RequestRefresh();
        }

        public void RequestRefresh()
        {
            RefreshThrottle.Invoke(Refresh);
        }

        void Refresh(CancellationToken ct)
        {
            CurrentBadgeInfo.Clear();
            CurrentBadgeInfo.AddOrUpdateFrom(GetCurrentBadgeInfo());

            

            InvalidateCurrentBadge();
        }

        //IDictionary<string, object> GetCurrentUIInfo()
        //{
        //    if (!UIService.IsUIThread)
        //    {
        //        return UIService.Run(GetCurrentUIInfo);
        //    }

        //    var result = new Dictionary<string, object>();

        //    System.Windows.Media.Color accentColor = Colors.White;

        //    //if (Config.ShouldUseVisualStudioTheme)
        //    {
        //        var windowColorKey = Microsoft.VisualStudio.PlatformUI.EnvironmentColors.SystemWindowColorKey;
        //        var windowColor = (System.Windows.Media.Color)Application.Current.MainWindow.FindResource(windowColorKey);

        //        accentColor = windowColor;
        //    }

        //    return result;
        //}

        void InvalidateCurrentBadge()
        {
            if(!UIService.IsUIThread)
            {
                UIService.Run(InvalidateCurrentBadge);
                return;
            }

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

            RequestRefresh();
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
                            var offset = new dwmapi.NativePoint(8, 8); // TODO: this may need to be 0,0 in VS 10 (because the window border size difference)

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

            var all_properties = new Dictionary<string, object>();

            all_properties.AddOrUpdateFrom(CurrentBadgeInfo);
            all_properties.AddOrUpdateFrom(CurrentThemeInfo);

            view.ViewModel.RefreshFrom(all_properties);

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
                
                RefreshIconicBitmap(view);
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
       
        IDictionary<string, object> GetCurrentBadgeInfo()
        {
#if DEBUG
            if (UIService.IsUIThread)
                throw new InvalidOperationException("For performance reasons this method should never be called on UI thread.");
#endif

            var result = new CurrentVSStateInfo();

            Dictionary<string, object> properties = new Dictionary<string, object>();

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
            }
            
            foreach(var scip in SourceControlInfoProviders)
            {
                var sc_properties = (IDictionary<string, object>)null;

                try
                {
                    if (scip.TryGetSourceControlInfo(solution.FullName, out sc_properties))
                    {
                        properties.AddOrUpdateFrom(sc_properties);
                    }
                }
                catch(Exception ex)
                {
                    // todo: log
                }
            }

            return properties;
        }
    }
}
