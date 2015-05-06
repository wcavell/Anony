using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Anony.Primitives
{
    internal class CookieSetting
    {
        private static string CookieFile = "cookie.cfg";

        public static async void SaveCookie(CookieCollection cookies)
        {
            var c = await GetCookieCollection();
            foreach (Cookie newCookie in cookies)
            {
                try
                {
                    bool exit = string.IsNullOrEmpty(c[newCookie.Name].Value);
                    c[newCookie.Name].Value = newCookie.Value;
                    c[newCookie.Name].Expires = newCookie.Expires;
                }
                catch
                {
                    c.Add(newCookie);
                }
            }
            var sb = new StringBuilder();
            foreach (Cookie cookie in c)
            {
                sb.AppendFormat("{0}#@cookie#{1}#@cookie#{2}#@cookie#{3}#@end#",
                    cookie.Name, cookie.Value, cookie.Expires
                    , cookie.HttpOnly.ToString()
                    );
            }
            var file =
                await
                    ApplicationData.Current.LocalFolder.CreateFileAsync(CookieFile,
                        CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(file, sb.ToString());
        }

        public static async Task<CookieCollection> GetCookieCollection()
        {
            StorageFile file = null;
            try
            {
                file = await ApplicationData.Current.LocalFolder.GetFileAsync(CookieFile);
            }
            catch
            {
                return new CookieCollection();
            }
            if (file == null) return new CookieCollection();
            var str = await FileIO.ReadTextAsync(file);
            if (string.IsNullOrEmpty(str)) return new CookieCollection();
            var par = new string[] { "#@end#" };
            string[] cookies = str.Split(par, StringSplitOptions.RemoveEmptyEntries);
            var collection = new CookieCollection();
            foreach (string c in cookies)
            {
                var _par = new string[] { "#@cookie#" };
                string[] cc = c.Split(_par, StringSplitOptions.None);
                var ck = new Cookie();
                ck.Name = cc[0];
                ck.Value = cc[1];
                ck.Expires = DateTime.Parse(cc[2]);
                ck.HttpOnly = bool.Parse(cc[3]);
                collection.Add(ck);
            }
            return collection;
        }
    }
}
