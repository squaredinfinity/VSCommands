using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SquaredInfinity.Foundation.Presentation.Xaml.Styles.Modern.Windows
{
    public class ModernWindow : SquaredInfinity.Foundation.Presentation.Windows.ViewHostWindow
    {
        #region Title Horizontal Alignment

        public HorizontalAlignment TitleHorizontalAlignment
        {
            get { return (HorizontalAlignment)GetValue(TitleHorizontalAlignmentProperty); }
            set { SetValue(TitleHorizontalAlignmentProperty, value); }
        }

        public static readonly DependencyProperty TitleHorizontalAlignmentProperty =
            DependencyProperty.Register(
            "TitleHorizontalAlignment",
            typeof(HorizontalAlignment),
            typeof(ModernWindow),
            new PropertyMetadata(HorizontalAlignment.Left));

        #endregion

        #region Title Bar Additional Content

        public FrameworkElement TitleBarAdditionalContent
        {
            get { return (FrameworkElement)GetValue(TitleBarAdditionalContentProperty); }
            set { SetValue(TitleBarAdditionalContentProperty, value); }
        }

        public static readonly DependencyProperty TitleBarAdditionalContentProperty =
            DependencyProperty.Register(
            "TitleBarAdditionalContent",
            typeof(FrameworkElement),
            typeof(ModernWindow),
            new PropertyMetadata(null));

        #endregion

        static ModernWindow()
        {
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(ModernWindow), new FrameworkPropertyMetadata("Styles.ModernWindow"));
        }

        public ModernWindow()
        {
            this.CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, this.OnCloseWindow));
            this.CommandBindings.Add(new CommandBinding(SystemCommands.MaximizeWindowCommand, this.OnMaximizeWindow, this.OnCanResizeWindow));
            this.CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, this.OnMinimizeWindow, this.OnCanMinimizeWindow));
            this.CommandBindings.Add(new CommandBinding(SystemCommands.RestoreWindowCommand, this.OnRestoreWindow, this.OnCanResizeWindow));
        }

        protected override void OnViewModelEvent(SquaredInfinity.Foundation.Presentation.ViewModels.ViewModelEventArgs args)
        {
            base.OnViewModelEvent(args);
        }

        private void OnCanResizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.ResizeMode == ResizeMode.CanResize || this.ResizeMode == ResizeMode.CanResizeWithGrip;
        }

        private void OnCanMinimizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.ResizeMode != ResizeMode.NoResize;
        }

        private void OnCloseWindow(object target, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        private void OnMaximizeWindow(object target, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MaximizeWindow(this);
        }

        private void OnMinimizeWindow(object target, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        private void OnRestoreWindow(object target, ExecutedRoutedEventArgs e)
        {
            SystemCommands.RestoreWindow(this);
        }
    }
}
