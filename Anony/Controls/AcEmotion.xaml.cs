using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Anony.Models;

// “用户控件”项模板在 http://go.microsoft.com/fwlink/?LinkId=234236 上提供

namespace Anony.Controls
{
    public sealed partial class AcEmotion : UserControl
    {
        public event EventHandler<string> Clicked;
        public AcEmotion()
        {
            this.InitializeComponent();
            Init();
        }
        private async void Init()
        {
            //var assets = await Package.Current.InstalledLocation.GetFolderAsync("Assets");
           // var emotions = await assets.GetFolderAsync("Emotions");
            ObservableCollection<Emoji> emojiExs = new ObservableCollection<Emoji>();
            EmojiFlipView.ItemsSource = emojiExs;
            //foreach (var folder in await emotions.GetFoldersAsync())
            //{
            //    Emoji emojiex = new Emoji();
            //    emojiex.Name = FolderToName(folder.Name);
            //    emojiex.Emotions = new ObservableCollection<Emotion>();
            //    foreach (var file in await folder.GetFilesAsync())
            //    {
            //        Emotion emoji = new Emotion();
            //        emoji.Img = "ms-appx:///Assets/Emotions/" + folder.Name + "/" + file.Name;
            //        emoji.Name = "[emot=" + folder.Name + "," + file.DisplayName + "/]";
            //        emojiex.Emotions.Add(emoji);
            //    }
            //    emojiExs.Add(emojiex);
            //}
            Dictionary<string,int> dictionary=new Dictionary<string, int>
            {
                {"ac",54},{"ais",40},{"brd",40},{"td",40},{"tsj",40}
            };
            foreach (var e in dictionary)
            {
                Emoji emojiex = new Emoji();
                emojiex.Name = FolderToName(e.Key);
                emojiex.Emotions = new ObservableCollection<Emotion>();
                for (int i = 1; i <= e.Value; i++)
                {
                    Emotion emoji = new Emotion();
                    emoji.Img = "ms-appx:///Assets/Emotions/" + e.Key + "/" + i.ToString("00")+".png";
                    emoji.Name = "[emot=" + e.Key + "," + i.ToString("00") + "/]";
                    emojiex.Emotions.Add(emoji);
                }
                emojiExs.Add(emojiex);
            }
        }
        private string FolderToName(string name)
        {
            switch (name)
            {
                case "ac":
                    return "AC娘";
                case "ais":
                    return "匿名版";
                case "brd":
                    return "皮尔德";
                case "td":
                    return "口水喵";
                case "tsj":
                    return "兔斯基";
                default:
                    return "";
            }
        }

        private void ListViewBase_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var emo = e.ClickedItem as Emotion;
            if (emo != null && Clicked != null)
            {
                Clicked(null, emo.Name);
            }
        }
    }
}
