using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Anony.Controls;
using Anony.Models;
using Anony.Primitives;
using Caliburn.Micro;

namespace Anony.ViewModels
{
    public partial class DetailViewModel:ViewModelBase
    {
        private double LastHeight = 0;
        private bool loading = false;
        public int Id { get; set; }
       
        private int page = 1;
        private bool _Isdown = false;
        private AcInfo info;
        public bool IsDown
        {
            get { return _Isdown; }
            set { this.Set(ref _Isdown, value); }
        }

        public DetailViewModel(INavigationService navigationService) : base(navigationService)
        {
        }

        protected override void OnViewReady(object view)
        {
            base.OnViewReady(view);
            if (Id != 0)
            {
                StatusBar.GetForCurrentView().ProgressIndicator.Text = "No."+Id;
                StatusBar.GetForCurrentView().ProgressIndicator.ProgressValue = 0;
            }
            var SlideGoBack = (bool)SettingHelper.GetValue("SlideGoBack", false);
            var grid =
                (view as FrameworkElement).GetChildObject<ListView>("ListView")
                    .GetChildByType<ScrollViewer>()
                    .GetChildByType<Grid>();
            if (SlideGoBack)
            {
                grid.ManipulationMode = ManipulationModes.TranslateX |
                                        ManipulationModes.TranslateRailsX | ManipulationModes.TranslateInertia |
                                        ManipulationModes.System;
                grid.ManipulationStarted -= grid_ManipulationStarted;
                grid.ManipulationCompleted -= grid_ManipulationCompleted;
                grid.ManipulationStarted += grid_ManipulationStarted;
                grid.ManipulationCompleted += grid_ManipulationCompleted;
            }
            else
            {
                if (grid.ManipulationMode != ManipulationModes.System)
                    grid.ManipulationMode = ManipulationModes.System;
            }
        }

        void grid_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (isSlide)
            {
                Debug.WriteLine(e.Velocities.Linear.X);
                if (e.Velocities.Linear.X > 1)
                {
                    navigationService.GoBack();
                }
                isSlide = false;
            }
        }

        private bool isSlide = false;
        void grid_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            isSlide = true;
        }

        protected override async void OnViewLoaded(object view)
        {
            IsDown = true;
            base.OnViewLoaded(view);
            info = await DataService.GetThread(Bunches, Id, page, Spare);
            ErrorVisib = info == null ? Visibility.Visible : Visibility.Collapsed;
            IsDown = false;
            var gv = (view as FrameworkElement).FindName("ListView") as ListView;
            if (gv != null)
            {
                var sv = gv.GetChildByType<ScrollViewer>();
                if (sv != null)
                {
                    sv.ViewChanging += async (s, e) =>
                    {
                        var scroll = sv;
                        var offset = (scroll.ScrollableHeight / Bunches.Count) * 3;
                        if (Spare)
                        {
                            if (info==null||info.CurrCount >= 19 || page <= info.TotalPage)
                            {
                                 if (LastHeight < scroll.VerticalOffset && !loading && scroll.VerticalOffset + offset > scroll.ScrollableHeight)
                                   await Next();
                            }
                        }
                        else
                        {
                            if (info == null || info.CurrCount >= 20 || page <= info.TotalPage)
                            {
                                if (LastHeight < scroll.VerticalOffset && !loading && scroll.VerticalOffset + offset > scroll.ScrollableHeight)
                                   await Next();
                            }
                        }
                        LastHeight = scroll.VerticalOffset;
                    };
                }
            }
        }

        private Popup popup;
        public void OnToPage()
        {
            if (popup != null)
            {
                popup.IsOpen = false;
                popup = null;
                return;
            }
            popup = new Popup();
            popup.Tag = "0";
            FlyoutHelper helper = new FlyoutHelper();
            FlipPage control = new FlipPage();
            var offset = 150;
            control.SetText(string.Format("当前第{0}页，共{1}页",page,info.TotalPage)); 
            helper.ShowPopup(control, popup, offset);
            control.Click += async (s, e) =>
            {
                if (e == 0 || e > info.TotalPage)
                {
                    Toast.ShowError("输入有误！");
                    return;
                }
                page = e;
                loading = true;
                Bunches.Clear();
                StatusBar.GetForCurrentView().ProgressIndicator.ProgressValue = null;
                info = await DataService.GetThread(Bunches, Id, page, Spare);
                ErrorVisib = info == null ? Visibility.Visible : Visibility.Collapsed;
                loading = false;
                StatusBar.GetForCurrentView().ProgressIndicator.ProgressValue = 0;
            };
        }

        public async void RefreshClick()
        {
            loading = true;
            StatusBar.GetForCurrentView().ProgressIndicator.ProgressValue = null;
            if (Spare)
            {
                if (info.CurrCount == 19)
                    page++;
            }
            else
            {
                if (info.CurrCount == 20)
                    page++;
            }
            info = await DataService.GetThread(Bunches, Id, page, Spare);
            ErrorVisib = info == null ? Visibility.Visible : Visibility.Collapsed;
            loading = false;
            StatusBar.GetForCurrentView().ProgressIndicator.ProgressValue = 0;
        }

        private async Task Next()
        {
            loading = true;
            StatusBar.GetForCurrentView().ProgressIndicator.ProgressValue = null;
            page++;
            info = await DataService.GetThread(Bunches, Id, page, Spare);
            ErrorVisib = info == null ? Visibility.Visible : Visibility.Collapsed;
            loading = false;
            StatusBar.GetForCurrentView().ProgressIndicator.ProgressValue = 0;
        }

        public void ReplyClick(Bunch bunch)
        {
            navigationService.UriFor<ReplyViewModel>()
                .WithParam(x => x.ContentText, ">>No." + bunch.Id)
                .WithParam(x => x.Id, Id.ToString())
                .WithParam(x => x.CreateNew, false)
                .WithParam(x => x.Spare, Spare)
                .Navigate();
        }

        public void ReportClick(Bunch bunch)
        {
            var rid = Spare ? "5" : "值班室";
            navigationService.UriFor<ReplyViewModel>()
                .WithParam(x=>x.ContentText,">>No."+bunch.Id)
                .WithParam(x=>x.CreateNew,true)
                .WithParam(x=>x.Id,rid)
                .WithParam(x=>x.Spare,Spare)
                .WithParam(x=>x.Report,true)
                .Navigate();
        }
        public async void FeedClick()
        {
            if (!Spare)
            {
                var info =await DataService.AddFeed(Id);
                if (info != null)
                {
                    if(info.Success)
                        Toast.Show(info.Data);
                    else 
                        Toast.ShowError(info.Data);
                }
                else
                {
                    Toast.ShowError("订阅失败");
                }
            }
            else
            {
                Toast.ShowError("备胎岛不支持此功能");
            }
        }

        public void OnReply()
        {
            navigationService.UriFor<ReplyViewModel>()
                .WithParam(x => x.Spare, Spare)
                .WithParam(x=>x.Id,Id.ToString())
                .WithParam(x=>x.CreateNew,false)
                .Navigate();
        }

        public void FavoriteClick()
        {
            CollectlData.SaveData(App.AutoBunch);
            //Toast.ShowError("未完成");
        }

    }

    public partial class DetailViewModel
    {
        private Visibility _visibility = Visibility.Collapsed;
        public Visibility ErrorVisib
        {
            get { return _visibility; }
            set { this.Set(ref _visibility, value); }
        }
        private ObservableCollection<Bunch> _bunches
         = new ObservableCollection<Bunch>();

        public ObservableCollection<Bunch> Bunches
        {
            get { return _bunches; }
            set { this.Set(ref _bunches, value); }
        } 
    }
}
