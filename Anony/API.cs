using System;

namespace Anony
{
    internal class API
    {
        public static string MenuContent = "http://ano-zhai-so.n1.yun.tf:8999/Home/Api/getForumList";
        public static string ChannelContent = "http://ano-zhai-so.n1.yun.tf:8999/Home/Api/showt?";
        public static string BunchContent = "http://ano-zhai-so.n1.yun.tf:8999/Home/Api/thread?";
        public static string Host = "http://ano-zhai-so.n1.yun.tf";
        /// <summary>
        /// User Agent设置为 “HAvfun Client”
        /// </summary>
        public static string TokenContent = "http://ano-zhai-so.n1.yun.tf:8999/Home/Api/getCookie";
        public static string ImageHost = "http://ano-zhai-so.n1.yun.tf:8999/Public/Upload/";
        public static string CreateBunchUrl = "http://ano-zhai-so.n1.yun.tf:8999/Home/Forum/doPostThread.html";
        public static string CreateReplyUrl = "http://ano-zhai-so.n1.yun.tf:8999/Home/Forum/doReplyThread.html";

        public static string MainHost = "http://h.nimingban.com";
        public static string MainImageHost = "http://cdn.ovear.info:8999";
        private static Random random;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="page"></param>
        /// <param name="spare">备胎</param>
        /// <returns></returns>
        public static string GetChannelUrl(string id, int page,bool spare)
        {
            random = new Random(DateTime.Now.Millisecond);
            if (spare)
                return ChannelContent + "id=" + id + "&ran=" + random.NextDouble() + "&page=" + page + ".html";
            return MainHost + "/api/" + id + "?page=" + page + "&ran=" + random.NextDouble();
        }

        public static string GetBunchUrl(int id, int page, bool spare)
        {
            random = new Random(DateTime.Now.Millisecond);
            if (spare)
                return BunchContent + "id=" + id + "&ran=" + random.NextDouble() + "&page=" + page + ".html";
            return MainHost + "/api/" + "t/" + id + "?page=" + page + "&ran=" + random.NextDouble();
        }
        /// <summary>
        /// 订阅
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public static string GetFeedUrl(int page)
        {
            return MainHost + "/api/feed?page=" + page;
        }

        public static string AddFeedUrl(int id)
        {
            return MainHost + "/api/feed/create?threadsId=" + id;
        }

        public static string DeleteFeedUrl(int id)
        {
            return MainHost + "/api/feed/remove?id=" + id;
        }
        /// <summary>
        /// 备胎Image
        /// </summary>
        /// <param name="img"></param>
        /// <param name="ext"></param>
        /// <returns></returns>
        public static string GetImage(string img,string ext)
        {
            if (string.IsNullOrEmpty(img)) return null;
            return ImageHost + img +ext;
        }

        public static string GetThumb(string img,string ext)
        {
            if (string.IsNullOrEmpty(img)) return null;
            return ImageHost + img + "_t"+ext;
        }
    }
}
