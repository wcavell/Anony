using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Anony.Primitives;
using Anony.Views;
using Caliburn.Micro;
using Coding4Fun.Toolkit.Controls;
using Coding4Fun.Toolkit.Controls.Common;

namespace Anony.ViewModels
{
    public class DrawViewModel : ViewModelBase
    {
        InkDrawing inkDrawing=new InkDrawing();
        private readonly SolidColorBrush _aliceBlueSolidColorBrush = new SolidColorBrush(Color.FromArgb(200, 240, 248, 255));
        private readonly SolidColorBrush _naturalBlueSolidColorBrush = new SolidColorBrush(Color.FromArgb(180, 0, 140, 180));
        private readonly SolidColorBrush _cornFlowerBlueSolidColorBrush = new SolidColorBrush(Color.FromArgb(140, 32, 32, 32));
        private Color Color = Colors.Black;
        private Canvas canvas;
        public DrawViewModel(INavigationService navigationService) : base(navigationService)
        {
            navigationService.BackPressed += navigationService_BackPressed;
            StrokeThickness = 2;
            IsChecked = true;
            SelectedIndex = 0;
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            canvas = (view as FrameworkElement).FindName("InkCanvas") as Canvas;
            inkDrawing.SetCanvas(canvas);
        }

        private void navigationService_BackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        {
            e.Handled = true;
            navigationService.BackPressed -= navigationService_BackPressed;
            MessagePrompt _prompt = new MessagePrompt();
            _prompt.IsCancelVisible = true;
            _prompt.Message = "是否确认当前涂鸦?";
            _prompt.Completed += async (s1, e1) =>
            {
                if (e1.PopUpResult == PopUpResult.Ok)
                {
                    var frame = Window.Current.Content as Frame;
                    navigationService.GoBack();
                    var view = frame.Content as ReplyView;
                    if (view != null)
                    {
                        RenderTargetBitmap render = new RenderTargetBitmap();
                        await render.RenderAsync(canvas);
                        var pixelbuff =await render.GetPixelsAsync();
                        var file =
                            await
                                ApplicationData.Current.LocalCacheFolder.CreateFileAsync("Ink.png",
                                    CreationCollisionOption.GenerateUniqueName);
                        using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                        {
                            var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
                            encoder.SetPixelData(
                                BitmapPixelFormat.Bgra8,
                                BitmapAlphaMode.Straight,
                                (uint)render.PixelWidth,
                                (uint)render.PixelHeight, 96d, 96d,
                                pixelbuff.ToArray());
                            await encoder.FlushAsync();
                        }
                        var model = view.DataContext as ReplyViewModel;
                        if (model != null)
                        {
                            model.File = file;
                        }
                    }
                }
                else
                {
                    navigationService.GoBack();
                }
            };
            _prompt.Show();
        }

        public void OnClick()
        {
            MessagePrompt _prompt = new MessagePrompt();
            _prompt.Background = _naturalBlueSolidColorBrush;
            _prompt.Foreground = _aliceBlueSolidColorBrush;
            _prompt.Overlay = _cornFlowerBlueSolidColorBrush;
            _prompt.IsCancelVisible = true;
            var colorpick = new ColorPicker();
            colorpick.Height = 300;
            colorpick.Color =Color;
            _prompt.Body = colorpick;
            _prompt.Completed += (s, e1) =>
            {
                if (e1.PopUpResult == PopUpResult.Ok)
                {
                    var color = colorpick.Color;
                    Color = color;
                    inkDrawing.Brush = new SolidColorBrush(color);
                    Debug.WriteLine(color);
                }
            };
            _prompt.Show();
        }

        private bool _IsChecked = true;

        public bool IsChecked
        {
            get { return _IsChecked; }
            set
            {
                this.Set(ref _IsChecked, value);
                if(value)
                    inkDrawing.Mode = InkDrawingMode.Draw;
                else
                    inkDrawing.Mode = InkDrawingMode.Erase;
            }
        }

       
        private double _StroeTheness = 2;

        public double StrokeThickness
        {
            get { return _StroeTheness; }
            set
            {
                this.Set(ref _StroeTheness, value);
                inkDrawing.StrokeThickness = value;
            }
        }

        private int _SelectedIndex = 0;

        public int SelectedIndex
        {
            get { return _SelectedIndex; }
            set
            {
                this.Set(ref _SelectedIndex, value);
                if (canvas != null)
                {
                    if (value == 0)
                        canvas.Background = new SolidColorBrush(Colors.Transparent);
                    else
                    {
                        canvas.Background = new SolidColorBrush(Colors.White);
                    }
                }
            }
        }
    }
}
