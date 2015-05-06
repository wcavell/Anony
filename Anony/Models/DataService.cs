using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.ApplicationModel;
using Windows.Storage;
using Anony.Primitives;
using Newtonsoft.Json;

namespace Anony.Models
{
    public class DataService
    {
        private static HttpClient client = new HttpClient();
        public static async Task<List<KeyedList<string, Section>>> GetSection(bool hasSpare=true)
        {
            var list=new List<KeyedList<string,Section>>();
            if(hasSpare)
            try
            {
                var str = await client.GetStringAsync(API.MenuContent);
                Debug.WriteLine(WebUtility.UrlDecode(str));
                var resp = JsonConvert.DeserializeObject<List<Section>>(str);
                list.Add(new KeyedList<string, Section>("备胎岛", resp));
            }
            catch
            {
                 
            }
            try
            {
                XElement xElement;
                var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///section.xml"));
                using (var reader = new StreamReader(await file.OpenStreamForReadAsync()))
                {
                    xElement = XElement.Parse(reader.ReadToEnd());
                    foreach (var x in xElement.Descendants("keys"))
                    {
                        string name = x.Attribute("name").Value;
                        var ls = new List<Section>();
                        foreach (var d in x.Descendants("param"))
                        {
                            Section menu = new Section();
                            menu.Spare = false;
                            menu.Id = d.Attribute("id").Value;
                            menu.Name = d.Value;
                            ls.Add(menu);
                        }
                        list.Add(new KeyedList<string, Section>(name, ls));
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return list;
        }

        public static async Task<AcInfo> GetHomeData(ObservableCollection<Bunch> bunches,string id,int page,bool spare)
        {
            var url = API.GetChannelUrl(id, page, spare);
            try
            {
                var str = await client.GetStringAsync(url);
                if (spare)
                {
                    var resp = JsonConvert.DeserializeObject<List<Bunch>>(str);
                    if (resp == null)
                    {
                        return null;
                    }
                    foreach (var b in resp)
                    {
                        if (bunches.FirstOrDefault(x => x.Id == b.Id) == null) 
                            bunches.Add(b);
                    }
                    return new AcInfo()
                    {
                        Success = true,
                        CurrCount = resp.Count
                    };
                }
                else
                {
                    var result = JsonConvert.DeserializeObject<RespHome>(str);
                    result.Page.TotalPage = result.Page.Size;
                    foreach (var b in result.Data.Threads)
                    {
                        if (bunches.FirstOrDefault(x => x.Id == b.Id) == null)
                            bunches.Add(b);
                    }
                    result.Page.Success = true;
                    return result.Page;
                }
            }
            catch
            {
                
            }
            return null;
        }

        public static async Task<AcInfo> GetThread(ObservableCollection<Bunch> bunches, int id, int page, bool spare)
        {
            var url = API.GetBunchUrl(id, page, spare);
            try
            {
                var str = await client.GetStringAsync(url);
                if (spare)
                {
                    var resp = JsonConvert.DeserializeObject<Bunch>(str);
                    resp.IsMySelf = true;
                    if (bunches.FirstOrDefault(x => x.Id == resp.Id) == null)
                        bunches.Add(resp);
                    if (resp.Replys != null)
                        foreach (var r in resp.Replys)
                        {
                            r.IsMySelf = resp.UserId == r.UserId;
                            if (bunches.FirstOrDefault(x => x.Id == r.Id) == null)
                                bunches.Add(r);
                        }
                    var p = new AcInfo
                    {
                        Success = true,
                        TotalCount = resp.ReplyCount,
                        TotalPage = (int)Math.Ceiling((resp.ReplyCount / 19.0))
                    };
                    if (resp.Replys != null)
                        p.CurrCount = resp.Replys.Count;
                    return p;
                }
                else
                {
                    var resp = JsonConvert.DeserializeObject<RespArticle>(str);
                    if (bunches.FirstOrDefault(x => x.Uid == resp.Threads.Uid) == null)
                    {
                        resp.Threads.IsMySelf = true;
                        bunches.Add(resp.Threads);
                    }
                    if(resp.Replys!=null)
                        foreach (var r in resp.Replys)
                        {
                            if (r.Uid == resp.Threads.Uid)
                                r.IsMySelf = true;
                            if (bunches.FirstOrDefault(x => x.Id == r.Id) == null)
                            {
                                bunches.Add(r);
                            }
                        }
                    if (resp.Replys != null)
                        resp.Page.CurrCount = resp.Replys.Count;
                    resp.Page.TotalPage = resp.Page.Size;
                    return resp.Page;
                }
            }
            catch
            {
                
            }
            return null;
        }

        public static async Task<Bunch> GetOneThread(int Id)
        {
            try
            {
                var url = "http://h.nimingban.com/api/t/" + Id;
                var str = await client.GetStringAsync(url);
                var item = JsonConvert.DeserializeObject<ThreadReap>(str);
                return item.Threads;
            }
            catch
            {
                return null;
            }
        }
        public static async Task<List<string>> GetEmotions()
        {
            //var assets = await Package.Current.InstalledLocation.GetFolderAsync("Assets");
            //var emotions = await assets.GetFolderAsync("Emotions");
            List<string> list = new List<string>();
            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///section.xml"));
            using (var reader = new StreamReader(await file.OpenStreamForReadAsync()))
            {
                var xElement = XElement.Parse(reader.ReadToEnd());
                list.AddRange(xElement.Descendants("select").First().Descendants("option").Select(x => x.Value));
            }
            return list;
        }

        public static async Task<TokenInfo> PostContent(CookieCollection cookie, string id, string content, bool IsReply, StorageFile file=null)
        {
            try
            {
                var handler = new HttpClientHandler
                {
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                    UseCookies = true
                };
                handler.CookieContainer = new CookieContainer();
                handler.CookieContainer.Add(new Uri(API.Host), cookie);
                using (var client = new HttpClient(handler))
                {
                    client.DefaultRequestHeaders.Add("UserAgent", "HAvfun Client");
                    using (var Datacontent = new MultipartFormDataContent())
                    {
                        Datacontent.Add(new StringContent(""), "\"name\"");
                        Datacontent.Add(new StringContent(""), "\"title\"");
                        Datacontent.Add(new StringContent(""), "\"email\"");
                        Datacontent.Add(new StringContent(content), "\"content\"");
                        Datacontent.Add(new StringContent(""), "\"pwd\"");
                        Datacontent.Add(new StringContent(""), "\"category\"");
                        Datacontent.Add(new StringContent("regist"), "\"mode\"");
                        Datacontent.Add(new StringContent("2048000"), "\"MAX_FILE_SIZE\"");
                        if (IsReply)
                        {
                            Datacontent.Add(new StringContent(id + ""), "\"resto\""); //回复的Id
                        }
                        else
                        {
                            Datacontent.Add(new StringContent(id + ""), "fid");
                        }
                        if (file != null)
                        {
                            var time = (DateTime.Now - new DateTime(1971, 1, 1)).TotalSeconds;
                            var stream = await file.OpenStreamForReadAsync();
                            var streamcontent = new StreamContent(stream);
                            streamcontent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");
                            Datacontent.Add(streamcontent, "\"upfile\"", file.Name);
                            Datacontent.Add(new StringContent(@"C:\Image\" + file.Name), "\"upfile_path\"");
                        }
                        var resp =
                            await client.PostAsync(IsReply ? API.CreateReplyUrl : API.CreateBunchUrl, Datacontent);
                        var str = await resp.Content.ReadAsStringAsync();
                        foreach (Cookie c in handler.CookieContainer.GetCookies(new Uri(API.Host)))
                        {
                            Debug.WriteLine("Name:" + c.Name + "    Value:" + c.Value);
                        }

                        if (str.Contains("回复成功") || str.Contains("发帖成功"))
                        {
                            var token = new TokenInfo
                            {
                                Success = true,
                                Cookie = handler.CookieContainer.GetCookies(new Uri(API.Host))
                            };
                            return token;
                        }
                        else
                        {
                            Debug.WriteLine(str);
                            return new TokenInfo
                            {
                                Success = false
                            };
                        }

                    }
                }
            }
            catch
            {
                return new TokenInfo { Success = false };
            }
        }

        public static async Task<TokenInfo> PostNew(CookieCollection cookie, string id, string body, bool IsReply, StorageFile file = null)
        {
            try
            {
                var handler = new HttpClientHandler();
                if (handler.CookieContainer == null)
                    handler.CookieContainer = new CookieContainer();
                handler.CookieContainer.Add(new Uri(API.MainHost), cookie);
                using (var client = new HttpClient(handler))
                {
                    using (var content = new MultipartFormDataContent())
                    {
                        content.Add(new StringContent(""), "\"name\"");
                        content.Add(new StringContent(""), "\"title\"");
                        content.Add(new StringContent(""), "\"emotion\"");
                        content.Add(new StringContent(body), "\"content\"");
                        if (file != null)
                        {
                            var stream = await file.OpenStreamForReadAsync();
                            var sc = new StreamContent(stream);
                            sc.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");
                            content.Add(sc, "\"image\"", file.Name);
                        }
                        var url = "";
                        if (IsReply)
                            url = API.MainHost + "/api/t/" + id + "/create";
                        else
                            url = API.MainHost + "/api/" + id + "/create";
                        Debug.WriteLine(await content.ReadAsStringAsync());
                        var resp = await client.PostAsync(url, content);
                        var str = await resp.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<RespResult>(str);
                        var info = new TokenInfo();
                        info.Message = result.Data;
                        info.Success = result.Success;
                        info.Cookie = handler.CookieContainer.GetCookies(new Uri(API.MainHost));
                        return info;
                    }

                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return new TokenInfo() { Success = false, Message = "提交失败！" };
        }

        public static async Task<AcInfo> GetFeeds(ObservableCollection<Bunch> bunches,int page)
        {
            var url = API.GetFeedUrl(page);
            try
            {
                var handler = new HttpClientHandler();
                if (handler.CookieContainer == null)
                    handler.CookieContainer = new CookieContainer();
                handler.CookieContainer.Add(new Uri(API.MainHost), await CookieSetting.GetCookieCollection());
                var client = new HttpClient(handler);
                var str = await client.GetStringAsync(url);
                var result = JsonConvert.DeserializeObject<RespFeedData>(str);
                foreach (var b in result.Threads)
                {
                    bunches.Add(b);
                }
                return result.Page;
            }
            catch
            {
                
            }
            return null;
        }
       

        public static async Task<RespResult> AddFeed(int id)
        {
            var url = API.AddFeedUrl(id);
            try
            {
                var handler = new HttpClientHandler();
                if (handler.CookieContainer == null)
                    handler.CookieContainer = new CookieContainer();
                handler.CookieContainer.Add(new Uri(API.MainHost), await CookieSetting.GetCookieCollection());
                var client = new HttpClient(handler);
                var str = await client.GetStringAsync(url);
                var result = JsonConvert.DeserializeObject<RespResult>(str);
                return result;
            }
            catch
            {
                
            }
            return new RespResult(){ Data = "失败",Success = false};
        }

        public static async Task<RespResult> DeleteFeed(int id)
        {
            var url = API.DeleteFeedUrl(id);
            try
            {
                var handler = new HttpClientHandler();
                if (handler.CookieContainer == null)
                    handler.CookieContainer = new CookieContainer();
                handler.CookieContainer.Add(new Uri(API.MainHost), await CookieSetting.GetCookieCollection());
                var client = new HttpClient(handler);
                var str = await client.GetStringAsync(url);
                var result = JsonConvert.DeserializeObject<RespResult>(str);
                return result;
            }
            catch
            {

            }
            return new RespResult() { Data = "失败", Success = false };
        }
    }
}
