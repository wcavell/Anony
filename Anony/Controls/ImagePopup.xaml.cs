using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// “用户控件”项模板在 http://go.microsoft.com/fwlink/?LinkId=234236 上提供

namespace Anony.Controls
{
    public sealed partial class ImagePopup : UserControl, INotifyPropertyChanged
    {
        private string _url;
        private BitmapImage bitmapImage;

        private Page _page;
        AppBar bar = new CommandBar();
        public ImagePopup(Popup popup,Page page)
        {
            this.InitializeComponent();
            _page = page;
            DataContext = this;
            Loaded += ImagePopup_Loaded;
            Width = Window.Current.Bounds.Width;
            Height = Window.Current.Bounds.Height;
            
            popup.Closed -= popup_Closed;
            popup.Closed += popup_Closed;
            bar = page.BottomAppBar;
        }

        void popup_Closed(object sender, object e)
        {
            _page.BottomAppBar = bar;
        }

        public void SetSource(string url)
        {
            _url = url;
            bitmapImage = new BitmapImage(new Uri(url));
            bitmapImage.DownloadProgress += bitmapImage_DownloadProgress;
            Image.Source = bitmapImage;
        }


        void bitmapImage_DownloadProgress(object sender, DownloadProgressEventArgs e)
        {
            if (e.Progress == 100)
            {
                ProgressTextBlock.Text = "100%";
                ProgressTextBlock.Visibility = Visibility.Collapsed;
            }
            else
            {
                ProgressTextBlock.Visibility = Visibility.Visible;
                ProgressTextBlock.Text = e.Progress + "%";
            }
        }

        void ImagePopup_Loaded(object sender, RoutedEventArgs e)
        {
            var combar=new CommandBar();
            combar.Opacity = 0.7;
            combar.ClosedDisplayMode=AppBarClosedDisplayMode.Minimal;
            AppBarButton btn=new AppBarButton();
            SymbolIcon symbol=new SymbolIcon(Symbol.Save);
            btn.Icon = symbol;
            btn.Label = "保存";
            combar.RequestedTheme=ElementTheme.Dark;
            btn.Click += btn_Click;
            combar.PrimaryCommands.Add(btn);
            _page.BottomAppBar = combar;
        }

        async void btn_Click(object sender, RoutedEventArgs e)
        {
            string lName = DateTime.Now.ToString("yyyyMMddhhmmss");
            if (Image.Source != null)
            {
                var str = _url.ToLower();
                if (str.Contains(".gif"))
                {
                    lName += ".gif";
                }
                else if (str.Contains(".jpg") || str.Contains(".jpeg"))
                {
                    lName += ".jpg";
                }
                else if (str.Contains(".png"))
                {
                    lName += ".png";
                }
                else if (str.Contains(".bmp"))
                {
                    lName += ".bmp";
                }
                else
                {
                    lName += ".jpg";
                }
                try
                {
                    var file = await KnownFolders.PicturesLibrary.CreateFileAsync(lName,
                        CreationCollisionOption.GenerateUniqueName);
                    var Rstream = RandomAccessStreamReference.CreateFromUri(new Uri(_url));
                    var stream = await Rstream.OpenReadAsync();
                    using (var picstream = await file.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        var output = picstream.AsStreamForWrite();
                        await stream.AsStreamForRead().CopyToAsync(output);
                    }
                    Toast.Show("保存成功！");
                }
                catch (Exception ex)
                {
                    Toast.ShowError("保存失败！");
                }
            }
        }

        private void ScrollViewer_OnDoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            var bitmap = Image.Source as BitmapImage;
            if (bitmap == null) return;
            var width = ScrollViewer.RenderSize.Width / bitmap.PixelWidth;
            var height = ScrollViewer.RenderSize.Height / bitmap.PixelHeight;
            ScrollViewer.ZoomToFactor(width < height
                ? float.Parse(width.ToString())
                : float.Parse(height.ToString()));
        }

        private void Image_OnImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            Toast.ShowError("打开图片失败");
        }

        private void Image_OnImageOpened(object sender, RoutedEventArgs e)
        {
            var bitmap = (sender as Image).Source as BitmapImage;
            if (bitmap != null)
            {
                if (bitmap.PixelWidth < ScrollViewer.RenderSize.Width &&
                    bitmap.PixelHeight < ScrollViewer.RenderSize.Height)
                {
                    Image.Stretch = Stretch.None;
                }
                if (bitmap.PixelWidth > ScrollViewer.RenderSize.Width ||
                    bitmap.PixelHeight > ScrollViewer.RenderSize.Height)
                {
                    Image.Stretch = Stretch.Uniform;
                    var width = bitmap.PixelWidth / ScrollViewer.RenderSize.Width;
                    var height = bitmap.PixelHeight / ScrollViewer.RenderSize.Height;
                    ScrollViewer.MaxZoomFactor = width > height
                        ? float.Parse(width.ToString())
                        : float.Parse(height.ToString());
                    width = ScrollViewer.RenderSize.Width / bitmap.PixelWidth;
                    height = ScrollViewer.RenderSize.Height / bitmap.PixelHeight;
                    ScrollViewer.ZoomToFactor(width < height
                        ? float.Parse(width.ToString())
                        : float.Parse(height.ToString()));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
