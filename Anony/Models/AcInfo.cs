using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anony.Models
{
    public class AcInfo
    {
        public int TotalPage { get; set; }
        public int TotalCount { get; set; }
        private int _currcount;
        
        public int CurrCount { get; set; }
        public bool Success { get; set; }
        public string Title { get; set; }
        public int Size { get; set; }
        public int Page { get; set; }
    }

    internal class RespHome
    {
        public int Code { get; set; }
        public RespHomeData Data { get; set; }
        public AcInfo Page { get; set; }

    }

    internal class RespHomeData
    {
        //public List<Bunch> Replys { get; set; }
        public List<Bunch> Threads { get; set; }
    }

    internal class RespFeedData
    {
        public AcInfo Page { get; set; }
        public List<Bunch> Threads { get; set; }
    }
    internal class RespArticle
    {
        public List<Bunch> Replys { get; set; }
        public AcInfo Page { get; set; }
        public Bunch Threads { get; set; }
    }

    internal class ThreadReap
    {
        public AcInfo Page { get; set; }
        public Bunch Threads { get; set; }
    }
    public class RespResult
    {
        public int Code { get; set; }
        public string Data { get; set; }
        public bool Success { get; set; }
    }
}
