using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Anony.Models;
using Anony.Primitives;
using Anony.ViewModels;
using Anony.Views;
using Caliburn.Micro;

// “空白应用程序”模板在 http://go.microsoft.com/fwlink/?LinkId=391641 上有介绍

namespace Anony
{
    /// <summary>
    /// 提供特定于应用程序的行为，以补充默认的应用程序类。
    /// </summary>
    public sealed partial class App 
    {
        private WinRTContainer container;

        public static bool AutoCollect = false;
        public static Bunch AutoBunch = new Bunch();
        public static bool NoImage { get; set; }
        public static bool Slide { get; set; }

        /// <summary>
        /// 初始化单一实例应用程序对象。    这是执行的创作代码的第一行，
        /// 逻辑上等同于 main() 或 WinMain()。
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            UnhandledException += App_UnhandledException;
            NoImage = false;
        }

        void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            
        }
        protected override void Configure()
        {
            MessageBinder.SpecialValues.Add("$clickeditem", c => ((ItemClickEventArgs)c.EventArgs).ClickedItem);
            
            container = new WinRTContainer();

            container.RegisterWinRTServices();

            container.PerRequest<CollectViewModel>()
                .PerRequest<DetailViewModel>()
                .PerRequest<HomeViewModel>()
                .PerRequest<ReplyViewModel>()
                .PerRequest<SearchViewModel>()
                .PerRequest<DrawViewModel>()
                .PerRequest<SettingViewModel>();
            ApplicationView.GetForCurrentView().SetDesiredBoundsMode(ApplicationViewBoundsMode.UseCoreWindow);
            StatusBar.GetForCurrentView().ProgressIndicator.ProgressValue = 0;
            StatusBar.GetForCurrentView().ProgressIndicator.Text = "  ";
            StatusBar.GetForCurrentView().ForegroundColor = Color.FromArgb(255, 247, 143, 12);
            StatusBar.GetForCurrentView().ProgressIndicator.ShowAsync();
            PrepareViewFirst();
            NoImage = (bool)SettingHelper.GetValue("NoImage", false);
            Slide = (bool) SettingHelper.GetValue("Slide", true);
        }
        protected override void PrepareViewFirst(Frame rootFrame)
        {
            container.RegisterNavigationService(rootFrame);
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            Initialize();
            RootFrame.ContentTransitions = new TransitionCollection() { new ContentThemeTransition { HorizontalOffset = 40 } };
            if (args.PreviousExecutionState == ApplicationExecutionState.Running)
                return;

            if (RootFrame.Content == null)
                DisplayRootView<HomeView>();
            var theme = (int)SettingHelper.GetValue("ThemeIndex", 0);
            if (theme == 0)
                ;
            else if (theme == 1)
                ThemeManager.LightTheme();
            else
                ThemeManager.CustomTheme();
            AutoCollect = (bool)SettingHelper.GetValue("AutoCollect", false);
        }

        protected override object GetInstance(Type service, string key)
        {
            return container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            container.BuildUp(instance);
        }

        protected override void OnActivated(IActivatedEventArgs args)
        {
            base.OnActivated(args);
            var continuationEventArgs = args as IContinuationActivatedEventArgs;
            if (continuationEventArgs != null)
            {
                // It is a continuation. Assuming we have already navigated
                // to ChatPage when restoring the state.
                var replyView = RootFrame.Content as ReplyView;
                if (replyView != null)
                {
                    var model = replyView.DataContext as ReplyViewModel;
                    if (model != null)
                    {
                        model.Continue(continuationEventArgs);
                    }
                }
            }
            Window.Current.Activate();
        }
    }
}