using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Anony.Controls;
using Anony.Models;
using Anony.Primitives;
using Caliburn.Micro;

namespace Anony.ViewModels
{
    public partial class ReplyViewModel : ViewModelBase
    {
        public ReplyViewModel(INavigationService navigationService) : base(navigationService)
        {
            Task.Factory.StartNew(async () =>
            {
                Emos = await DataService.GetEmotions();
            });
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            var textbox = (view as FrameworkElement).FindName("TextBox") as TextBox;
            textbox.GotFocus += (s, e) =>
            {
                CommandBarVisibility = Visibility.Visible;
                EmosVisibility = Visibility.Collapsed;
            };
            textbox.LostFocus += (s, e) => { CommandBarVisibility = Visibility.Collapsed; };
            if (Report) StatusBar.GetForCurrentView().ProgressIndicator.Text = "举报";
            else StatusBar.GetForCurrentView().ProgressIndicator.Text = "     ";
            if (!string.IsNullOrEmpty(ContentText))
            {
                ContentText += "\r";
                SelectionStart = ContentText.Length;
            }
        }



        public void DrawClick()
        {
            navigationService.UriFor<DrawViewModel>()
                .Navigate();
        }

        public void ShowClick()
        {
            EmosVisibility = EmosVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        public void EmoClick(string em)
        {
            ContentText += em;
            SelectionStart = ContentText.Length;
        }

        public void PhotoClick()
        {
            FileOpenPicker open = new FileOpenPicker();
            open.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            open.ViewMode = PickerViewMode.Thumbnail;
            open.ContinuationData["Action"] = "SendPicture";
            open.FileTypeFilter.Clear();
            open.FileTypeFilter.Add(".bmp");
            open.FileTypeFilter.Add(".png");
            open.FileTypeFilter.Add(".jpeg");
            open.FileTypeFilter.Add(".jpg");
            open.FileTypeFilter.Add(".gif");
            open.PickSingleFileAndContinue();
        }

        public StorageFile File { get; set; }

        public async void Continue(IContinuationActivatedEventArgs args)
        {
            if (args.Kind == ActivationKind.PickFileContinuation)
            {
                var openPickerContinuationArgs = args as FileOpenPickerContinuationEventArgs;

                // Recover the "Action" info we stored in ContinuationData
                string action = (string) openPickerContinuationArgs.ContinuationData["Action"];
                if (action == "SendPicture")
                    if (openPickerContinuationArgs.Files.Count > 0)
                    {
                        File = openPickerContinuationArgs.Files[0];
                    }
                    else
                    {
                        File = null;
                    }
            }
        }

        public async void SendClick()
        {
            StatusBar.GetForCurrentView().ProgressIndicator.Text = "正在上传";
            StatusBar.GetForCurrentView().ProgressIndicator.ProgressValue = null;
            CookieCollection cookie = await CookieSetting.GetCookieCollection();
            TokenInfo info;
            if (Spare)
            {

                info =
                    await
                        DataService.PostContent(cookie, Id, ContentText, !CreateNew, File);
            }
            else
            {
                info =
                    await
                        DataService.PostNew(cookie, Id, ContentText, !CreateNew,
                            File);
            }
            if (info == null || !info.Success)
            {
                Toast.ShowError("发生错误了");
                StatusBar.GetForCurrentView().ProgressIndicator.Text = "    ";
                StatusBar.GetForCurrentView().ProgressIndicator.ProgressValue = 0;
                return;
            }
            if (info.Cookie != null)
            {
                foreach (Cookie c in info.Cookie)
                {
                    try
                    {
                        cookie[c.Name].Value = c.Value;
                        cookie[c.Name].Expires = c.Expires;
                    }
                    catch
                    {
                        cookie.Add(c);
                    }
                }
                CookieSetting.SaveCookie(cookie);
            }
            if (info.Success)
            {
                Toast.Show("发送成功");
                if (App.AutoCollect)
                {
                    CollectlData.SaveData(App.AutoBunch);
                }
                StatusBar.GetForCurrentView().ProgressIndicator.Text = "    ";
                StatusBar.GetForCurrentView().ProgressIndicator.ProgressValue = 0;
                navigationService.GoBack();
            }
        }

        public void ClearClick()
        {
            if (File != null)
            {
                File = null;
                Toast.Show("清除图像成功");
            }
        }

        private Popup popup;

        public void AcExpreClick()
        {
            if (popup != null) return;
            popup = new Popup();
            AcEmotion em = new AcEmotion();
            FlyoutHelper helper = new FlyoutHelper();
            helper.ShowPopup(em, popup);
            em.Clicked += (s, e) =>
            {
                ContentText += e;
                SelectionStart = ContentText.Length;
            };
            popup.Closed += (s, e) =>
            {
                em = null;
                popup = null;
            };
        }

    }

    public partial class ReplyViewModel
    {
        private string _ContentText = "";
        private int _SelectionStart = 0;
        public bool Spare { get; set; }
        public string Id { get; set; }
        public bool CreateNew { get; set; }
        private bool _Report = false;

        public bool Report
        {
            get { return _Report; }
            set { _Report = value; }
        }
        public int SelectionStart
        {
            get { return _SelectionStart; }
            set { this.Set(ref _SelectionStart, value); }
        }

        public string ContentText
        {
            get { return _ContentText; }
            set { this.Set(ref _ContentText, value); }
        }

        private Visibility _emoVisibility = Visibility.Collapsed;

        public Visibility EmosVisibility
        {
            get { return _emoVisibility; }
            set { this.Set(ref _emoVisibility, value); }
        }

        private List<string> _emos;

        public List<string> Emos
        {
            get { return _emos; }
            set { this.Set(ref _emos, value); }
        }

        private Visibility _CommandBarVisibility = Visibility.Collapsed;

        public Visibility CommandBarVisibility
        {
            get { return _CommandBarVisibility; }
            set { this.Set(ref _CommandBarVisibility, value); }
        }
    }
}
