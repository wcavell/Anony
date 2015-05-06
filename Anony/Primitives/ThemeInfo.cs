using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Anony.Primitives
{
    public class ThemeInfo
    {
        /// <summary>
        /// 0
        /// </summary>
        public static Color AcBackground;
        /// <summary>
        /// 1
        /// </summary>
        public static Color AcHomeBorder;
        /// <summary>
        /// 2
        /// </summary>
        public static Color AcTitle;
        /// <summary>
        /// 3
        /// </summary>
        public static Color AcContent;
        /// <summary>
        /// 4
        /// </summary>
        public static Color AcPoBackground;
        /// <summary>
        /// 5
        /// </summary>
        public static Color AcReplyBackground;

        static ThemeInfo()
        {
            AcBackground = GetColor("AcBackground", Color.FromArgb(255, 234, 234, 234));
            AcHomeBorder = GetColor("AcHomeBorder", Color.FromArgb(255, 255, 255, 255));
            AcTitle = GetColor("AcTitle", Color.FromArgb(255, 96, 96, 96));
            AcContent = GetColor("AcContent", Color.FromArgb(255, 64, 64, 64));
            AcPoBackground = GetColor("AcPoBackground", Color.FromArgb(255, 214, 214, 214));
            AcReplyBackground = GetColor("AcReplyBackground", Color.FromArgb(255, 154, 191, 232));
        }
        public static Color GetIndexColor(int i)
        {
            switch (i)
            {
                case 0:
                    return AcBackground;
                case 1:
                    return AcHomeBorder;
                case 2:
                    return AcTitle;
                case 3:
                    return AcContent;
                case 4:
                    return AcPoBackground;
                case 5:
                    return AcReplyBackground;
                default:
                    return Colors.White;
            }
        }

        public static void SaveColorIndex(int i, Color color)
        {
            switch (i)
            {
                case 0:
                    AcBackground = color;
                    SetColor("AcBackground", color);
                    SaveColor("AcBackground", color);
                    break;
                case 1:
                    AcHomeBorder = color;
                    SetColor("AcHomeBorder", color);
                    SaveColor("AcHomeBorder", color);
                    break;
                case 2:
                    AcTitle = color;
                    SetColor("AcTitle", color);
                    SaveColor("AcTitle", color);
                    break;
                case 3:
                    AcContent = color;
                    SetColor("AcContent", color);
                    SaveColor("AcContent", color);
                    break;
                case 4:
                    AcPoBackground = color;
                    SetColor("AcPoBackground", color);
                    SaveColor("AcPoBackground", color);
                    break;
                case 5:
                    AcReplyBackground = color;
                    SetColor("AcReplyBackground", color);
                    SaveColor("AcReplyBackground", color);
                    break;
            }
        }
        private static void SetColor(string name, Color color)
        {
            ((SolidColorBrush)Application.Current.Resources[name]).Color = color;
        }

        private static void SaveColor(string name, Color color)
        {
            ((SolidColorBrush)Application.Current.Resources[name]).Color = color;
            ApplicationData.Current.LocalSettings.Values[name] = color.ToString();
        }

        public static void ChangedTheme()
        {
            SetColor("AcBackground", AcBackground);
            SetColor("AcHomeBorder", AcHomeBorder);
            SetColor("AcTitle", AcTitle);
            SetColor("AcContent", AcContent);
            SetColor("AcPoBackground", AcPoBackground);
            SetColor("AcReplyBackground", AcReplyBackground);
        }

        public static void Redo()
        {
            SaveColor("AcBackground", Color.FromArgb(255, 234, 234, 234));
            SaveColor("AcHomeBorder", Color.FromArgb(255, 255, 255, 255));
            SaveColor("AcTitle", Color.FromArgb(255, 96, 96, 96));
            SaveColor("AcContent", Color.FromArgb(255, 64, 64, 64));
            SaveColor("AcPoBackground", Color.FromArgb(255, 214, 214, 214));
            SaveColor("AcReplyBackground", Color.FromArgb(255, 154, 191, 232));
        }

        private static Color GetColor(string name, Color def)
        {
            var color = ApplicationData.Current.LocalSettings.Values[name];
            if (color == null)
            {
                ApplicationData.Current.LocalSettings.Values[name] = def.ToString();
                return def;
            }
            return ColorParst(color.ToString());
        }
        /// <summary>
        /// Hexa(#FF000000) To Color
        /// </summary>
        /// <param name="hexaColor"></param>
        /// <returns></returns>
        public static Color ColorParst(string hexaColor)
        {
            if (hexaColor.Length == 7)
                return Color.FromArgb(255,
                    Convert.ToByte(hexaColor.Substring(1, 2), 16),
                    Convert.ToByte(hexaColor.Substring(3, 2), 16),
                    Convert.ToByte(hexaColor.Substring(5, 2), 16)
                    );
            return Color.FromArgb(
                Convert.ToByte(hexaColor.Substring(1, 2), 16),
                Convert.ToByte(hexaColor.Substring(3, 2), 16),
                Convert.ToByte(hexaColor.Substring(5, 2), 16),
                Convert.ToByte(hexaColor.Substring(7, 2), 16)
                );
        }
    }
}
