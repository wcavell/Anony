using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Anony.Models;

namespace Anony.Controls
{
    public class DetailSelector : DataTemplateSelector
    {
        public DataTemplate ReplyTemplate { get; set; }
        public DataTemplate PoTemplate { get; set; }
        protected override DataTemplate SelectTemplateCore(object item)
        {
            var b = item as Bunch;
            if (b == null) return PoTemplate;
            return b.IsMySelf ? PoTemplate : ReplyTemplate;
        }
    }
}
