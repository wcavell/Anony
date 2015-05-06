using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Anony.Primitives
{
    public class FontManager : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public static FontManager Fonts;

        protected static void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (Fonts == null) return;
            var handler = Fonts.PropertyChanged;
            if (handler != null) handler(Fonts, new PropertyChangedEventArgs(propertyName));
        }

        private static double _TitleFontSize = 13;

        public static double TitleFontSize
        {
            get { return _TitleFontSize; }
            set
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (_TitleFontSize != value)
                {
                    _TitleFontSize = value;
                    OnPropertyChanged();
                    SetValue("TitleFontSize", value);
                }
            }
        }

        private static double _ContentSize = 18;

        public static double ContentSize
        {
            get { return _ContentSize; }
            set
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (_ContentSize != value)
                {
                    _ContentSize = value;
                    OnPropertyChanged();
                    SetValue("ContentSize", value);
                }
            }
        }
        public FontManager()
        {
            Fonts = this;
            TitleFontSize = (double)GetValue("TitleFontSize", 13d);
            ContentSize = (double)GetValue("ContentSize", 18d);
        }

        //static FontManager()
        //{
        //    if(Fonts==null) Fonts=new FontManager();
        //}

        public static object GetValue(string name, object value)
        {
            var res = ApplicationData.Current.LocalSettings.Values[name];
            if (res == null)
            {
                ApplicationData.Current.LocalSettings.Values[name] = value;
                res = value;
            }
            return res;
        }

        public static void SetValue(string name, object value)
        {
            ApplicationData.Current.LocalSettings.Values[name] = value;
        }
    }
}
