using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “用户控件”项模板在 http://go.microsoft.com/fwlink/?LinkId=234236 上提供

namespace Anony.Controls
{
    public sealed partial class FlipPage : UserControl
    {
        public FlipPage()
        {
            this.InitializeComponent();
            Width = Window.Current.Bounds.Width;
        }

        public event EventHandler<int> Click;
        public event EventHandler<string> StrClick;

        public void SetText(string text)
        {
            AcPhoneTextBox.PlaceholderText = text;
        }

        public void SetOkText(string text)
        {
            SearchButton.Content = text;
        }
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (Click != null)
            {
                int page;
                if (int.TryParse(AcPhoneTextBox.Text, out page))
                {
                    Click(null, page);
                }
            }
            if (StrClick != null)
                StrClick(null, AcPhoneTextBox.Text);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            var ps = VisualTreeHelper.GetOpenPopups(Window.Current);
            foreach (var p in ps)
            {
                if (p.Tag == "0")
                    p.IsOpen = false;
            }
        }
    }
}
