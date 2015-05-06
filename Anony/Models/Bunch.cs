using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anony.Models
{
    public class Bunch
    {
        public bool Spare { get; set; }
        public bool IsAdmin
        {
            get
            {
                if (!string.IsNullOrEmpty(UserId))
                    return Admin == 1;
                return Uid.Contains("font");
            }
        }
        public int Admin { get; set; }
        public string Content { get; set; }
        public string CreatedTime
        {
            get
            {
                if (string.IsNullOrEmpty(Now))
                    return CreatedAt.UtcToTime();
                return Now;
            }
        }
        public string CreatedAt { get; set; }
        public string Email { get; set; }
        public string Ext { get; set; }
        public int FeedId { get; set; }
        public int Id { get; set; }
        public string Img { get; set; }

        private string _Image;
        public string Image
        {
            get
            {
                if (string.IsNullOrEmpty(_Image))
                    return API.GetImage(Img, Ext);
                return API.MainImageHost + _Image.ToLower();
            }
            set { _Image = value; }
        }

        private string _thumb;
        public string Thumb
        {
            get
            {
                if (string.IsNullOrEmpty(_thumb))
                    return API.GetThumb(Img, Ext);
                return API.MainImageHost + _thumb.ToLower();
            }
            set { _thumb = value; }
        }
        public bool IsMySelf { get; set; }
        private string _name = "无名氏";
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    _name = value;
            }
        }
        public int ReplyCount { get; set; }
        private string _title = "无标题";

        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    _title = value;
            }
        }
        public int ThreadsId { get; set; }
        public string UserId { get; set; }
        private string uid;

        public string Uid
        {
            get
            {
                if (string.IsNullOrEmpty(uid))
                    return UserId;
                return uid;
            }
            set
            {
                uid = value;
            }
        }

        public string Now { get; set; }

        private string _upat;

        public string UpdatedTime
        {
            get
            {
                if (string.IsNullOrEmpty(_upat))
                    return null;
                return _upat.UtcToTime();
            }
        }
        public string UpdatedAt
        {
            get
            {
                return _upat;
            }
            set { _upat = value; }
        }
        public List<Bunch> Replys { get; set; }
    }

     static class BunchEx
    {
        public static string UtcToTime(this string str)
        {
            var dtTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);
            dtTime = dtTime.Add(new TimeSpan(long.Parse(str + "0000")));
            
            dtTime = dtTime.AddHours(8);
            return dtTime.ToString("MM/dd H:mm");  
        }
    }
}

