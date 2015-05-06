using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Devices.Input;
using Windows.Phone.UI.Input;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Anony.Controls;
using Anony.Models;
using Anony.Primitives;
using Anony.Views;
using Caliburn.Micro;

namespace Anony.ViewModels
{
    public partial class HomeViewModel:ViewModelBase
    {
        private double LastHeight = 0;
        private bool loading = false;
        public Section Section { get; set; }
        private int page = 1;
        private bool _spshow = true;
        private Stopwatch stopwatch=new Stopwatch();
        private AppBarButton barButton = null;
        public FlipViewItem FlipMenuItem = null;
        public HomeViewModel(INavigationService navigationService) 
            : base(navigationService)
        {
            Task.Factory.StartNew(InitData);
            navigationService.BackPressed -= navigationService_BackPressed;
            navigationService.BackPressed += navigationService_BackPressed;
            
        }

        void navigationService_BackPressed(object sender, BackPressedEventArgs e)
        {
            if (navigationService.CurrentSourcePageType == typeof(HomeView))
            {
                if (!stopwatch.IsRunning)
                {
                    stopwatch.Start();
                    Toast.Show("再按一次退出！");
                    e.Handled = true;
                }
                else
                {
                    stopwatch.Stop();
                    if (stopwatch.ElapsedMilliseconds >= 1500)
                    {
                        stopwatch.Reset();
                        e.Handled = true;
                    }
                    else
                    {
                        SettingHelper.SetValue("LastSectionId", Section.Id);
                        SettingHelper.SetValue("LastSectionName",Section.Name);
                        SettingHelper.SetValue("LastSectionSpare",Section.Spare);
                        App.Current.Exit();
                    }
                }
            }
        }
        
        private async void InitData()
        {
            _spshow = (bool) SettingHelper.GetValue("SpareShow", false);
            var item = await DataService.GetSection(_spshow);
            Sections = item;
            
        }

        private async void InitLoadedData()
        {
            var se = new Section();
            se.Id =
                (string)
                    SettingHelper.GetValue("LastSectionId", "综合版1");
            se.Name = (string)
                SettingHelper.GetValue("LastSectionName", "综合版1");
            se.Spare = (bool)SettingHelper.GetValue("LastSectionSpare", false);
            Section = se;
            IsDown = true;
            Bunches.Clear();
            SelectedIndex = 0;
            page = 1;
            StatusBar.GetForCurrentView().ProgressIndicator.Text = Section.Name;
            info = await DataService.GetHomeData(Bunches, Section.Id, page, Section.Spare);
            IsDown = false;
            ErrorVisib = info == null ? Visibility.Visible : Visibility.Collapsed;

        }
        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            //InitEvent();
            InitLoadedData();
            
        }


        protected override void OnViewReady(object view)
        {
            base.OnViewReady(view);
            if (Section != null)
            {
                StatusBar.GetForCurrentView().ProgressIndicator.Text = Section.Name;
                StatusBar.GetForCurrentView().ProgressIndicator.ProgressValue = 0;
            }
            if ((bool) SettingHelper.GetValue("SpareShow", true) != _spshow)
            {
                InitData();
            }
            InitEvent();
        }

        private void InitEvent()
        {
            IsSlideEnabled = App.Slide;
            try
            {
                var lv = (GetView() as FrameworkElement).GetChildObject<ListView>("ListView");
               // var lvm = (GetView() as FrameworkElement).GetChildObject<ListView>("ListViewMenu");
                var svlv = lv.GetChildByType<ScrollViewer>();

                #region SVLV

                svlv.ViewChanging -= OnViewChanging;
                svlv.ViewChanging += OnViewChanging;
                

                #endregion

                var bars = (GetView() as Page).BottomAppBar as CommandBar;
                if (barButton == null)
                    barButton = bars.PrimaryCommands[2] as AppBarButton;
                if (IsSlideEnabled)
                {
                    barButton.Visibility = Visibility.Collapsed;
                    MenuVisibility=Visibility.Visible;
                }
                else
                {
                    barButton.Visibility = Visibility.Visible;
                    MenuVisibility=Visibility.Collapsed;
                }
            }
            catch
            {
                
            }

        }

        private void OnViewChanging(object sender, ScrollViewerViewChangingEventArgs e)
        {
            var scroll = sender as ScrollViewer; 
            var offset = (scroll.ScrollableHeight / Bunches.Count) * 2;
            if (LastHeight < scroll.VerticalOffset && !loading && scroll.VerticalOffset + offset > scroll.ScrollableHeight)
                Next(scroll);
            LastHeight = scroll.VerticalOffset;
        }

    
        private async void Next(ScrollViewer scroll)
        {
            loading = true;
            StatusBar.GetForCurrentView().ProgressIndicator.ProgressValue = null;
            page++;
            info = await DataService.GetHomeData(Bunches, Section.Id, page, Section.Spare);
            if (info == null)
            {
                ErrorVisib = Visibility.Visible;
            }
            else ErrorVisib = Visibility.Collapsed;
            loading = false;
            StatusBar.GetForCurrentView().ProgressIndicator.ProgressValue = 0;
        }

        public void BunchClick(Bunch bunch)
        {
            bunch.Spare = Section.Spare;
            App.AutoBunch = bunch;
            navigationService.UriFor<DetailViewModel>()
                .WithParam(x => x.Id, bunch.Id)
                .WithParam(x => x.Spare, Section.Spare)
                .Navigate();
        }

        private Popup _popup;
        public void GoToClick()
        {
            if (_popup != null)
            {
                _popup.IsOpen = false;
                _popup = null;
                return;
            }
            _popup=new Popup();
            _popup.Tag = "0";
            FlyoutHelper helper = new FlyoutHelper();
            FlipPage control = new FlipPage();
            var offset = 150;
            control.SetText("输入串Id（备:b123,主:a123）");
            control.SetOkText("确认");
            helper.ShowPopup(control, _popup, offset);
            control.StrClick += async (s, e) =>
            {
                try
                {
                    if (e.Contains("a"))
                    {
                        var str = Regex.Match(e, @"\d+").Value;
                        navigationService.UriFor<DetailViewModel>()
                            .WithParam(x => x.Id, int.Parse(str))
                            .WithParam(x => x.Spare, false)
                            .Navigate();
                        _popup.IsOpen = false;
                        _popup = null;
                    }
                    else if (e.Contains("b"))
                    {
                        var str = Regex.Match(e, @"\d+").Value;
                        navigationService.UriFor<DetailViewModel>()
                            .WithParam(x => x.Id, int.Parse(str))
                            .WithParam(x => x.Spare, true)
                            .Navigate();
                        _popup.IsOpen = false;
                        _popup = null;
                    }
                    else
                    {
                        Toast.ShowError("你输入有误！");
                    }
                }
                catch
                {
                    Toast.ShowError("你输入有误！");
                }
            };
        }
        public async void RefreshClick()
        {
            Bunches.Clear();
            page = 1;
            IsDown = true;
            info = await DataService.GetHomeData(Bunches, Section.Id, page, Section.Spare);
            ErrorVisib = info == null ? Visibility.Visible : Visibility.Collapsed;
            IsDown = false;
        }
        public void FavoriteClick()
        {
            navigationService.UriFor<CollectViewModel>()
                .Navigate();
        }
        public void CreateClick()
        {
            if(Section==null)return;
            navigationService.UriFor<ReplyViewModel>()
                .WithParam(x => x.Spare, Section.Spare)
                .WithParam(x => x.Id, Section.Id)
                .WithParam(x=>x.CreateNew,true)
                .Navigate();

        }

        public void SettingClick()
        {
            navigationService.UriFor<SettingViewModel>()
                .Navigate();
        }

        public async void SectionClick(Section section)
        {
            Section = section;
            StatusBar.GetForCurrentView().ProgressIndicator.Text = Section.Name;
            IsDown = true;
            Bunches.Clear();
            SelectedIndex = 0;
            page = 1;
            info = await DataService.GetHomeData(Bunches, section.Id, page, Section.Spare);
            IsDown = false;
            if(info==null)
                ErrorVisib=Visibility.Visible;
            else
                ErrorVisib=Visibility.Collapsed;
        }

        private Popup popup;
        public void ShowMenuClick()
        {
            if (popup != null)
            {
                popup.IsOpen = false;
                popup = null;
                return;
            }
            popup = new Popup();
            FlyoutHelper flyout=new FlyoutHelper();
            MenusControl control=new MenusControl(Sections);
            control.Click -= OnPopupMenusClick;
            control.Click += OnPopupMenusClick;
            flyout.Show(control, popup, () => { popup = null; });
        }

        private void OnPopupMenusClick(object sender, Section e)
        {
            if (popup != null)
            {
                popup.IsOpen = false;
                popup = null;
            }
            SectionClick(e);
        }
    }

    public partial class HomeViewModel
    {
        private AcInfo info;
        private int _selectedIndex = 0;
        private bool _IsDown = false;
        private Visibility _visibility=Visibility.Collapsed;
        public bool IsDown
        {
            get { return _IsDown; }
            set { this.Set(ref _IsDown, value); }
        }

        public Visibility ErrorVisib
        {
            get { return _visibility; }
            set { this.Set(ref _visibility, value); }
        }
        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set { this.Set(ref _selectedIndex, value); }
        }
        private ObservableCollection<Bunch> bunches = new BindableCollection<Bunch>();

        public ObservableCollection<Bunch> Bunches
        {
            get { return bunches; }
            set { this.Set(ref bunches, value); }
        } 
        private List<KeyedList<string,Section>> sections=new List<KeyedList<string, Section>>();

        public List<KeyedList<string, Section>> Sections
        {
            get { return sections; }
            set { this.Set(ref sections, value); }
        }

        private bool _isInit = false;

        public bool IsInit
        {
            get { return _isInit; }
            set { this.Set(ref _isInit, value); }
        }

        private bool _isSlideEnabled = false;

        public bool IsSlideEnabled
        {
            get { return _isSlideEnabled; }
            set { this.Set(ref _isSlideEnabled, value); }
        }

        private Visibility _MenuVisibility = Visibility.Visible;

        public Visibility MenuVisibility
        {
            get { return _MenuVisibility; }
            set { this.Set(ref _MenuVisibility, value); }
        }
    }
}
