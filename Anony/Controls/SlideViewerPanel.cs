using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Anony.Controls
{
   public class SlideViewerPanel:Panel
    {
        private SlideViewer _owner;

        private readonly List<SlideViewPanelItemOffset> _childrenPositions;
        private double _viewportWidth;
        private double _viewportHeight;
        private double _totalWidth;

        #region PanelIndex (Attached DependencyProperty)

        public static readonly DependencyProperty PanelIndexProperty =
            DependencyProperty.RegisterAttached("PanelIndex", typeof(int), typeof(SlideViewerPanel), new PropertyMetadata(0, new PropertyChangedCallback(OnPanelIndexChanged)));

        public static void SetPanelIndex(DependencyObject o, int value)
        {
            o.SetValue(PanelIndexProperty, value);
        }

        public static int GetPanelIndex(DependencyObject o)
        {
            return (int)o.GetValue(PanelIndexProperty);
        }

        private static void OnPanelIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var panel = VisualTreeHelper.GetParent(d) as SlideViewerPanel;
            if (panel != null)
                panel.InvalidateArrange();
        }

        #endregion


        public SlideViewerPanel()
        {
            _childrenPositions = new List<SlideViewPanelItemOffset>();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            FindSlideView();

            _viewportWidth = _owner.ViewportWidth;
            _viewportHeight = _owner.ViewportHeight;

            var childSize = new Size(_viewportWidth, _viewportHeight);
            var desiredSize = new Size(0, _viewportHeight);

            foreach (FrameworkElement child in Children)
            {
                child.Measure(childSize);
                desiredSize.Width += child.Width > 0 ? child.DesiredSize.Width : childSize.Width;
            }

            return desiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double offset = 0;
            var finalRect = new Rect(0, 0, 0, finalSize.Height);

            var childrenList = Children.OrderBy(GetPanelIndex);

            _childrenPositions.Clear();
            foreach (FrameworkElement child in childrenList)
            {
                finalRect.X = offset;
                _childrenPositions.Add(new SlideViewPanelItemOffset(child, offset));

                finalRect.Width = child.Width > 0 ? child.DesiredSize.Width : _viewportWidth;
                if (finalRect.Width <= 0)
                    finalRect.Width = _viewportWidth;

                child.Arrange(finalRect);

                offset += finalRect.Width;
            }

            finalSize.Width = _totalWidth = offset;

            // Constraint the different panel positions
            foreach (var childPosition in _childrenPositions)
            {
                childPosition.OffsetX = Math.Min(_totalWidth - _viewportWidth, childPosition.OffsetX);
            }

            return finalSize;
        }

        internal double GetOffset(int index)
        {
            if (Children.Count == 0)
                return 0;

            index = Math.Max(0, Math.Min(Children.Count - 1, index));
            return _childrenPositions[index].OffsetX * (-1);
        }


        private void FindSlideView()
        {
            FrameworkElement frameworkElement = this;
            SlideViewer owner;
            do
            {
                frameworkElement = (FrameworkElement)VisualTreeHelper.GetParent(frameworkElement);
                owner = frameworkElement as SlideViewer;
            }
            while (frameworkElement != null && owner == null);

            if (owner != null)
            {
                _owner = owner;
                owner.Panel = this;
            }
        }
    }
}
