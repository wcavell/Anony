using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Anony.Models
{
    public class TokenInfo
    {
        public bool Success { get; set; }
        public CookieCollection Cookie { get; set; }
        public string Message { get; set; }
    }
}
