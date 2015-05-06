using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Anony.Primitives
{
    public enum Theme
    {
        Light,
        Dark,
        Custom
    }

    public static class ThemeManager
    {
        /// <summary>
        /// 白
        /// </summary>
        public static void DarkTheme()
        {
            SetColor("AcBackground", Color.FromArgb(255, 234, 234, 234));
            SetColor("AcHomeBorder", Color.FromArgb(255, 255, 255, 255));
            SetColor("AcTitle", Color.FromArgb(255, 96, 96, 96));
            SetColor("AcContent", Color.FromArgb(255, 64, 64, 64));
            SetColor("AcPoBackground", Color.FromArgb(255, 214, 214, 214));
            SetColor("AcReplyBackground", Color.FromArgb(255, 154, 191, 232));
        }

        private static void SetColor(string name, Color color)
        {
            ((SolidColorBrush)Application.Current.Resources[name]).Color = color;

        }
        /// <summary>
        /// 黑
        /// </summary>
        public static void LightTheme()
        {
            SetColor("AcBackground", Color.FromArgb(255, 16, 16, 16));
            SetColor("AcHomeBorder", Color.FromArgb(255, 21, 21, 21));
            SetColor("AcTitle", Color.FromArgb(255, 144, 144, 144));
            SetColor("AcContent", Color.FromArgb(255, 160, 160, 160));
            SetColor("AcPoBackground", Color.FromArgb(255, 81, 77, 87));
            SetColor("AcReplyBackground", Color.FromArgb(255, 48, 66, 85));
        }
        /// <summary>
        /// 用户
        /// </summary>
        public static void CustomTheme()
        {
            ThemeInfo.ChangedTheme();
        }
    }
}
