using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Anony.Primitives;
using Caliburn.Micro;
using Coding4Fun.Toolkit.Controls;

namespace Anony.ViewModels
{
    public partial class SettingViewModel:ViewModelBase
    {
        private ColorPicker colorPicker;
        public SettingViewModel(INavigationService navigationService) : base(navigationService)
        {
            NoImage = (bool) SettingHelper.GetValue("NoImage", false);
            ThemeTypeIndex = (int) SettingHelper.GetValue("ThemeIndex", 0);
            HomeVisibility=Visibility.Visible;
            TitleFontSize = (double) FontManager.GetValue("TitleFontSize", 13d);
            ContentFontSize = (double)FontManager.GetValue("ContentSize", 18d);
            AutoCollect = (bool) SettingHelper.GetValue("AutoCollect", false);
            SpareShow = (bool) SettingHelper.GetValue("SpareShow", false);
            Slide = (bool) SettingHelper.GetValue("Slide", true);
            SlideGoBack = (bool)SettingHelper.GetValue("SlideGoBack", false);
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            colorPicker = (view as FrameworkElement).FindName("ColorPicker") as ColorPicker;
            colorPicker.Color = ThemeInfo.GetIndexColor(0);
        }

        public void ChangedColor()
        {
            ThemeInfo.SaveColorIndex(TargetIndex, colorPicker.Color);
        }
    }
    public partial class SettingViewModel
    {
        private int _themetypeindex = 0;

        public int ThemeTypeIndex
        {
            get { return _themetypeindex; }
            set
            {
                this.Set(ref _themetypeindex, value);
                SettingHelper.SetValue("ThemeIndex",value);
                switch (value)
                {
                    case 0:
                        CustomThemeVis = Visibility.Collapsed;
                        ThemeManager.DarkTheme();
                        break;
                    case 1:
                        CustomThemeVis = Visibility.Collapsed;
                        ThemeManager.LightTheme();
                        break;
                    case 2:
                        CustomThemeVis = Visibility.Visible;
                        ThemeManager.CustomTheme();
                        break;
                }
            }
        }

        private int _themePlaceIndex = 0;

        public int ThemePlaceIndex
        {
            get { return _themePlaceIndex; }
            set { this.Set(ref _themePlaceIndex, value); }
        }

        private bool _noImage = false;

        public bool NoImage
        {
            get { return _noImage; }
            set
            {
                if (value != _noImage)
                {
                    App.NoImage = value;
                    SettingHelper.SetValue("NoImage",value);
                }
                this.Set(ref _noImage, value);
            }
        }
        private Visibility _homeVisibility=Visibility.Collapsed;

        public Visibility HomeVisibility
        {
            get { return _homeVisibility; }
            set
            {
                this.Set(ref _homeVisibility, value);
                DetailVisibility = value == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        private Visibility _customThemeVis = Visibility.Collapsed;

        public Visibility CustomThemeVis
        {
            get { return _customThemeVis; }
            set { this.Set(ref _customThemeVis, value); }
        }
        private Visibility _detailVisibility=Visibility.Collapsed;

        public Visibility DetailVisibility
        {
            get { return _detailVisibility; }
            set { this.Set(ref _detailVisibility, value); }
        }

        private int _targetIndex = 0;

        public int TargetIndex
        {
            get { return _targetIndex; }
            set { this.Set(ref _targetIndex, value); }
        }

        private double _titleFontSize;

        public double TitleFontSize
        {
            get { return _titleFontSize; }
            set
            {
                this.Set(ref _titleFontSize, value);
                FontManager.TitleFontSize = value;
            }
        }

        private double _contentFontSize;

        public double ContentFontSize
        {
            get { return _contentFontSize; }
            set
            {
                if (_contentFontSize != value)
                {
                    FontManager.ContentSize = value;
                }
                this.Set(ref _contentFontSize, value);
            }
        }

        private bool _AutoCollect;

        public bool AutoCollect
        {
            get { return _AutoCollect; }
            set
            {
                this.Set(ref _AutoCollect, value);
                App.AutoCollect = value;
                SettingHelper.SetValue("AutoCollect",value);
            }
        }

        private bool _spareShow;

        public bool SpareShow
        {
            get { return _spareShow; }
            set
            {
                this.Set(ref _spareShow, value);
                SettingHelper.SetValue("SpareShow",value);
            }
        }

        private bool _Slide = false;

        public bool Slide
        {
            get { return _Slide; }
            set
            {
                if(_Slide!=value)
                    SettingHelper.SetValue("Slide",value);
                this.Set(ref _Slide, value);
                App.Slide = value;
            }
        }

        private bool _slideGoBack = false;

        public bool SlideGoBack
        {
            get { return _slideGoBack; }
            set
            {
                if(_slideGoBack!=value)
                    SettingHelper.SetValue("SlideGoBack", value);
                this.Set(ref _slideGoBack, value);
            }
        }
    }
}
