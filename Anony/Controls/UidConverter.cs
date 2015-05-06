using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using Anony.Models;

namespace Anony.Controls
{
    internal class UidConverter : DependencyObject
    {
        public static object GetSource(DependencyObject obj)
        {
            return obj.GetValue(SourceProperty);
        }

        public static void SetSource(DependencyObject obj, object value)
        {
            obj.SetValue(SourceProperty, value);
        }
        public static readonly DependencyProperty SourceProperty =
               DependencyProperty.RegisterAttached("Source", typeof(object),
               typeof(UidConverter), new PropertyMetadata(null, OnTextChanged));

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tb = d as TextBlock;
            var bun = e.NewValue as Bunch;
            if (tb == null || bun == null) return;
            tb.Inlines.Clear();
            Run run = new Run();
            run.Text = bun.Uid;
            if (bun.IsAdmin && string.IsNullOrEmpty(bun.UserId))
            {
                var index = bun.Uid.IndexOf(">", StringComparison.Ordinal);
                if (index != -1)
                {
                    var end = bun.Uid.IndexOf('<', index);
                    var str = bun.Uid.Substring(index + 1, end - index - 1);
                    run.Text = str;
                }
                run.Foreground = new SolidColorBrush(Colors.Red);
            }
            tb.Inlines.Add(run);
        }
    }
}
