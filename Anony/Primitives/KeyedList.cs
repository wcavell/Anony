using System.Collections.Generic;
using System.Linq;

namespace Anony.Primitives
{
    public class KeyedList<TKey, TItem> : List<TItem>
    {
        public TKey GroupKey { protected set; get; }

        public KeyedList(TKey key, IEnumerable<TItem> items)
            : base(items)
        {
            GroupKey = key;
        }

        public KeyedList(IGrouping<TKey, TItem> grouping)
            : base(grouping)
        {
            GroupKey = grouping.Key;
        }
    }
}
