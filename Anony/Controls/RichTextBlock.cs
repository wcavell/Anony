using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Anony.Models;
using Anony.ViewModels;
using Anony.Views;
using Caliburn.Micro;

namespace Anony.Controls
{
    public class RichTextHelper:DependencyObject
    {
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
            "Source", typeof(object), typeof(RichTextHelper), new PropertyMetadata(default(object), OnSourceChanged));

        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tb = d as RichTextBlock; 
            var bun = e.NewValue as Bunch;
            if (tb == null||bun==null) return;
            tb.Blocks.Clear();
            var str = WebUtility.HtmlDecode(bun.Content);
            str = Regex.Replace(str, @"</?\s?br\s?/?>", Environment.NewLine, RegexOptions.IgnoreCase);
            str = str.Replace("�", "");
            //if (tb.GetParentByType<HomeView>() != null)
            //{
            //    if (str.Length > 200)
            //        str = str.Substring(0, 200) + "...";
            //}
            //<a[^>]*href=\s*[""']?(?<url>[^""']*)[""']?[^>]*?>(?<text>(?!<a).*)</a>
            var rex = @"(<font[\s\S]*?>)([\s\S]*?)(</font>)" +
                @"|<a[^>]+\s*[^>]*>(?<text>.*?)</a>|([a-zA-z]+://[^\s]*)|(```[\s\S]+```)|(\[emot=\w+,\d+/\])|ac\d+|>>No.\d+|aa\d+"
               ;
            var regEx = new Regex(rex,RegexOptions.IgnoreCase | RegexOptions.Multiline);
           

            int nextOffset = 0;

            foreach (Match match in regEx.Matches(str))
            {
                if (match.Index >= nextOffset)
                {
                    //if (match.Index == 0)
                    AppendText((str.Substring(nextOffset, match.Index - nextOffset)),tb);
                    nextOffset = match.Index + match.Length;
                    Debug.WriteLine(match.Value);
                    AppendMatch(match.Value,tb);
                }
            }

            if (nextOffset < str.Length)
            {
                AppendText((str.Substring(nextOffset)), tb);
            }

            Paragraph paragraph = new Paragraph();
            tb.Blocks.Add(paragraph);
            if (string.IsNullOrEmpty(bun.Thumb)||App.NoImage) return;
            Image image = new Image();
            image.Stretch = Stretch.Uniform;
            image.Height = 180;
            InlineUIContainer container = new InlineUIContainer
            {
                Child = image
            };
            paragraph.Inlines.Add(container);
            image.Source = new BitmapImage(new Uri(bun.Thumb));
            image.Tapped += (s, e1) =>
            {
                e1.Handled = true;
                var page = (s as FrameworkElement).GetParentByType<Page>();
                Popup popup=new Popup();
                ImagePopup imagePopup=new ImagePopup(popup,page);
                imagePopup.SetSource(bun.Image);
                FlyoutHelper helper=new FlyoutHelper();
                helper.Show(imagePopup, popup);
                Debug.WriteLine("ImageTapped");
            };
        }

       
        private static void AppendMatch(string str, RichTextBlock tb)
        {
            var block = tb.Blocks;
            Paragraph paragraph;
            HyperlinkButton linkButton = new HyperlinkButton();
            linkButton.Style = (Style) Application.Current.Resources["Hylink"];
            linkButton.FontSize = tb.FontSize;
            linkButton.Tapped -= linkButton_Tapped;
            linkButton.Tapped += linkButton_Tapped; 
            InlineUIContainer container = new InlineUIContainer();
            container.Child = linkButton;
            if (block.Count == 0 ||
                (paragraph = block[block.Count - 1] as Paragraph) == null)
            {
                paragraph = new Paragraph();
                block.Add(paragraph);
            }
            
            if (str.Contains("```"))
            {
                Run run = new Run();
                run.Text =(str);
                paragraph.Inlines.Add(run);
            }
            else if (Regex.IsMatch(str,
                @"(<font[\s\S]*?>)([\s\S]*?)(</font>)", RegexOptions.IgnoreCase | RegexOptions.Multiline))
            {
                var groups =
                    Regex.Match(str,
                        @"<font[^>]+color=\s*(?:'(?<color>[^']+)'|""(?<color>[^""]+)""|(?<color>[^>\s]+))\s*[^>]*>(?<text>[\s\S]*?)</font>")
                        .Groups;
                var color = groups["color"].Value;
                var text = groups["text"].Value;
                if (Regex.IsMatch(str, @"No.\d+"))
                {
                    linkButton.Content =(text);
                    linkButton.Tag = "No";
                    linkButton.Foreground = new SolidColorBrush(ColorParse(color));
                    paragraph.Inlines.Add(container);
                }
                else
                {
                    var run = new Run();
                    run.Text =(text);
                    run.Foreground = new SolidColorBrush(ColorParse(color));
                    paragraph.Inlines.Add(run);
                }
            }
            else if (str.Contains("font"))
            {
                
            }
            else if (str.Contains(">>No."))
            {
                linkButton.Content = (str);
                linkButton.Foreground = new SolidColorBrush(ColorParst("#FF789922"));
                linkButton.Tag = "No";
                paragraph.Inlines.Add(container);
            }
            else if (str.ToLower().Contains("[emot="))
            {
                Image image = new Image();
                image.Stretch = Stretch.None;
                int start = str.IndexOf("=", StringComparison.Ordinal);
                int end = str.IndexOf(",", StringComparison.Ordinal);
                var stru = str.Substring(start + 1, end - start - 1);
                var index = Regex.Match(str, @"\d+").Value;
                var url = string.Format("ms-appx:///Assets/Emotions/{0}/{1}.png", stru, index);
                image.Source = new BitmapImage(new Uri(url));
                var inline = new InlineUIContainer {Child = image};
                paragraph.Inlines.Add(inline);
            }
            else if (Regex.IsMatch(str, @"<a[^>]+href=\s*(?:'(?<href>[^']+)'|""(?<href>[^""]+)""|(?<href>[^>\s]+))\s*[^>]*>(?<text>.*?)</a>"))
            {
                try
                {
                    var hrefs = Regex.Match(str,
                        @"<a[^>]+href=\s*(?:'(?<href>[^']+)'|""(?<href>[^""]+)""|(?<href>[^>\s]+))\s*[^>]*>(?<text>.*?)</a>");
                    var url = hrefs.Groups["href"].Value;
                    var content = hrefs.Groups["text"].Value;
                    linkButton.Foreground = new SolidColorBrush(Colors.DodgerBlue);
                    if (!url.Contains("http"))
                    {
                        linkButton.Tag = "http";
                    }
                    else if (!url.Contains("http://hacfun.tv/t/") && !url.Contains("http://h.nimingban.com/t/"))
                    {
                        linkButton.NavigateUri = new Uri(url);
                        linkButton.Tag = "link";
                    }
                    else
                    {
                        linkButton.Tag = "http";
                    }
                    linkButton.Content =(content);
                    paragraph.Inlines.Add(container);
                }
                catch(Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
            else if (str.ToLower().Contains("http://"))
            {
                try
                {
                    //HyperlinkButton link = new HyperlinkButton();
                    //link.NavigateUri = new Uri(str);
                    
                    if (str.ToLower().Contains("http://www.acfun.tv/") && str.ToLower().Contains("/ac"))
                    {
                        var result = Regex.Match(str, @"\d+").Value;
                        linkButton.Foreground = new SolidColorBrush(Colors.DodgerBlue);
                        var url = new Uri("acfun://detail/video/" + result);
                        linkButton.NavigateUri = url;
                        linkButton.Content = str;
                        linkButton.Tag = "http";
                        paragraph.Inlines.Add(container);
                    }
                    else if (str.ToLower().Contains("http://www.acfun.tv/") && str.ToLower().Contains("/aa"))
                    {
                        //Collection
                        var result = Regex.Match(str, @"\d+").Value;
                        linkButton.Foreground = new SolidColorBrush(Colors.DodgerBlue);
                        var url = new Uri("acfun://detail/collection/" + result);
                        linkButton.NavigateUri = url;
                        linkButton.Content = str;
                        linkButton.Tag = "http";
                        paragraph.Inlines.Add(container);
                    }
                    else
                    {
                        try
                        {
                            var path =
                                @"((http|ftp|https)://)(([a-zA-Z0-9\._-]+\.[a-zA-Z]{2,6})|([0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}))(:[0-9]{1,4})*(/[a-zA-Z0-9\&%_\./-~-]*)?";
                            var strvalue = Regex.Match(str, path).Value;
                            linkButton.Foreground = new SolidColorBrush(Colors.DodgerBlue);
                            linkButton.NavigateUri = new Uri(strvalue);
                            linkButton.Content = str;
                            linkButton.Tag = "http";
                            paragraph.Inlines.Add(container);
                        }
                        catch
                        {
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
            else if (str.ToLower().Contains("ac"))
            {
                linkButton.Content = str;
                linkButton.Tag = "ac";
                var result = Regex.Match(str, @"\d+").Value;
                var url = new Uri("acfun://detail/video/" + result);
                linkButton.NavigateUri = url;
                linkButton.Foreground = (SolidColorBrush) App.Current.Resources["AcThemeBrush"];
                paragraph.Inlines.Add(container);
            }
            else if (str.Contains("aa"))
            {
                linkButton.Content = str;
                linkButton.Tag = "aa";
                var result = Regex.Match(str, @"\d+").Value;
                var url = new Uri("acfun://detail/collection/" + result);
                linkButton.NavigateUri = url;
                linkButton.Foreground = (SolidColorBrush) App.Current.Resources["AcThemeBrush"];
                paragraph.Inlines.Add(container);
            }
            else
            {
                Run run = new Run();
                run.Text = (str);
                paragraph.Inlines.Add(run);
            }
        }

        static async void linkButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var hylink = sender as HyperlinkButton;
            if (hylink == null || hylink.Tag == null || hylink.Content == null) return;
            var tag = hylink.Tag.ToString();
            var content = hylink.Content.ToString();
            e.Handled = true;
            if (tag == "link")
            {
                return;
            }
            else if (tag == "No")
            {
                int id = 0;
                int.TryParse(Regex.Match(content, @"\d+").Value, out id);
                var view = (sender as FrameworkElement).GetParentByType<DetailView>();
                if (view != null)
                {
                    var model = view.DataContext as DetailViewModel;
                    if (model == null) return;
                    var bun = (e.OriginalSource as FrameworkElement).DataContext as Bunch;
                    var item = model.Bunches.FirstOrDefault(x => x.Id == id);
                    if (item == null)
                    {
                        if (model.Spare)
                            Toast.ShowError("找不到引用串");
                        else
                        {
                            var p = await DataService.GetOneThread(id);
                            if (p == null)
                                return;
                            FlyoutHelper helper = new FlyoutHelper();
                            Popup popup = new Popup();
                            BunchPopup bunchPopup = new BunchPopup();
                            bunchPopup.SetBunch(p);
                            helper.Show(bunchPopup, popup);
                        }
                    }
                    else
                    {
                        FlyoutHelper helper = new FlyoutHelper();
                        Popup popup = new Popup();
                        BunchPopup bunchPopup = new BunchPopup();
                        bunchPopup.SetBunch(item);
                        helper.Show(bunchPopup, popup);
                    }
                }
            }
            else if (tag == "http" && (content.Contains("http://hacfun.tv/t/") || content.Contains("http://h.nimingban.com/t")))
            {
                var strnum = Regex.Match(content, @"\d+").Value;
                int num = 0;
                if (int.TryParse(strnum, out num))
                {
                    var page = (sender as FrameworkElement).GetParentByType<Page>();
                    try
                    {
                        if (page is HomeView)
                        {
                            var vm = page.DataContext as HomeViewModel;
                            vm.navigationService.UriFor<DetailViewModel>()
                                .WithParam(x => x.Id, num)
                                .WithParam(x => x.Spare, false)
                                .Navigate();
                        }
                        else if (page is DetailView)
                        {
                            var vm = page.DataContext as DetailViewModel;
                            vm.navigationService.UriFor<DetailViewModel>()
                                .WithParam(x => x.Id, num)
                                .WithParam(x => x.Spare, false)
                                .Navigate();
                        }

                    }
                    catch
                    {

                    }
                }
            }
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
        /// <summary>
        /// such Red or #FFFFFF .etc
        /// </summary>
        /// <returns></returns>
        public static Color ColorParse(string str)
        {
            if (str[0] == '#')
            {
                return ColorParst(str);
            }
            else
                switch (str.ToLower())
                {
                    case "red":
                        return Colors.Red;
                    case "black":
                        return Colors.Black;
                    case "blue":
                        return Colors.Blue;
                    case "brown":
                        return Colors.Brown;
                    case "cyan":
                        return Colors.Cyan;
                    case "darkgray":
                        return Colors.DarkGray;
                    case "gray":
                        return Colors.Gray;
                    case "green":
                        return Colors.Green;
                    case "lightgrary":
                        return Colors.LightGray;
                    case "magenta":
                        return Colors.Magenta;
                    case "oranage":
                        return Colors.Orange;
                    case "purple":
                        return Colors.Purple;
                    case "transparent":
                        return Colors.Transparent;
                    case "white":
                        return Colors.White;
                    case "yellow":
                        return Colors.Yellow;
                    default:
                        return Colors.Black;
                }
        }

        private static void AppendText(string str, RichTextBlock tb)
        {
            Paragraph paragraph=new Paragraph();
            Run run = new Run();
            run.Text = str;
            paragraph.Inlines.Add(run);
            tb.Blocks.Add(paragraph);
        }

        public static string GetSource(DependencyObject obj)
        {
            return (string)obj.GetValue(SourceProperty);
        }

        public static void SetSource(DependencyObject obj, string value)
        {
            obj.SetValue(SourceProperty, value);
        }
    }
}
