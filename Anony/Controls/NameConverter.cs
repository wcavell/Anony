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
    internal class NameConverter : DependencyObject
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
               typeof(NameConverter), new PropertyMetadata(null, OnTextChanged));

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tb = d as TextBlock;
            var bun = e.NewValue as Bunch;
            if (tb == null || bun == null) return;
            tb.Inlines.Clear();
            Run run = new Run();
            run.Text = bun.Name;
            if (bun.Admin != 0&&string.IsNullOrEmpty(bun.Uid))
            {
                run.Foreground = new SolidColorBrush(Colors.Red);
            }
            tb.Inlines.Add(run);
        }
    }
}
