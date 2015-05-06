using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anony.Models
{
    /// <summary>
    /// 板块
    /// </summary>
    public class Section
    {
        private bool spare = true;

        /// <summary>
        /// 是否为备胎
        /// </summary>
        internal bool Spare
        {
            get { return spare; }
            set { spare = value; }
        }

        public string Id { get; set; }
        public string Name { get; set; }
    }
}
