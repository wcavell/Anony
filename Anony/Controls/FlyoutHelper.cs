using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace Anony.Controls
{
    public class FlyoutHelper
    {
        protected Popup m_Popup;
        private Action CloseAction;
        public Popup Show(Popup popup, FrameworkElement button, double offset = 35d)
        {
            if (popup == null)
                throw new Exception("Popup is not defined");
            m_Popup = popup;
            if (button == null)
                throw new Exception("Button is not defined");
            if (double.IsNaN(offset))
                throw new Exception("Offset is not defined");
            var _Child = popup.Child as FrameworkElement;
            if (_Child == null)
                throw new Exception("Popup.Child is not defined");
            if (double.IsNaN(_Child.Height))
                throw new Exception("Popup.Child.Height is not defined");
            if (double.IsNaN(_Child.Width))
                throw new Exception("Popup.Child.Width is not defined");

            // get position of the button
            var _Page = Window.Current.Content as Page;
            var _Visual = button.TransformToVisual(_Page);
            var _Point = _Visual.TransformPoint(new Point(0, 0));
            var _Button = new
            {
                Top = _Point.Y,
                Left = _Point.X,
                Width = button.ActualWidth,
                Height = button.ActualHeight,
            };

            // determine location
            var _TargetTop = (_Button.Top) - _Child.Height - offset;
            var _TargetLeft = (_Button.Left + (_Button.Width / 2)) - (_Child.Width / 2);

            if ((_TargetLeft + _Child.Width) > Window.Current.Bounds.Width)
                _TargetLeft = Window.Current.Bounds.Width - _Child.Width - offset;
            if (_TargetLeft < 0)
                _TargetLeft = offset;

            // setup popup
            popup.VerticalOffset = _TargetTop;
            popup.HorizontalOffset = _TargetLeft;

            // add pretty animation(s)
            popup.ChildTransitions = new Windows.UI.Xaml.Media.Animation.TransitionCollection
            {
                new Windows.UI.Xaml.Media.Animation.EntranceThemeTransition
                {
                    FromHorizontalOffset = 0,
                    FromVerticalOffset = 20
                }
            };

            // setup
            m_Popup.IsLightDismissEnabled = true;
            m_Popup.IsOpen = true;

            // handle when it closes
            m_Popup.Closed -= popup_Closed;
            m_Popup.Closed += popup_Closed;

            // handle making it close
            Window.Current.Activated -= Current_Activated;
            Window.Current.Activated += Current_Activated;

            // return
            return m_Popup;
        }
        

        private FrameworkElement _parentElement;
        public void Show(FrameworkElement element, Popup popup)
        {
            m_Popup = popup;
            m_Popup.VerticalAlignment = VerticalAlignment.Stretch;
            m_Popup.HorizontalAlignment = HorizontalAlignment.Stretch;
            element.HorizontalAlignment = HorizontalAlignment.Stretch;
            element.VerticalAlignment = VerticalAlignment.Stretch;
            element.Width = Window.Current.Bounds.Width;
            element.Height = Window.Current.Bounds.Height;
            m_Popup.Child = element;

            //m_Popup.VerticalOffset = Window.Current.Bounds.Height;
            m_Popup.IsLightDismissEnabled = true;
            m_Popup.ChildTransitions = new Windows.UI.Xaml.Media.Animation.TransitionCollection
            {
                new Windows.UI.Xaml.Media.Animation.EntranceThemeTransition
                {
                    FromHorizontalOffset = 0,
                    FromVerticalOffset = 20
                }
            };
            m_Popup.IsLightDismissEnabled = true;
            m_Popup.IsOpen = true;

            // handle when it closes
            m_Popup.Closed -= popup_Closed;
            m_Popup.Closed += popup_Closed;

            // handle making it close
            Window.Current.Activated -= Current_Activated;
            Window.Current.Activated += Current_Activated;
        }

        public void Show(FrameworkElement element, Popup popup, Action action)
        {
            Show(element, popup);
            CloseAction = action;
        }
        public void ShowPopup(FrameworkElement element, Popup popup,double offset=0)
        {
            m_Popup = popup;
            m_Popup.Child = element;
            m_Popup.VerticalOffset = offset;
            m_Popup.IsLightDismissEnabled = true;
            m_Popup.ChildTransitions = new Windows.UI.Xaml.Media.Animation.TransitionCollection
            {
                new Windows.UI.Xaml.Media.Animation.EntranceThemeTransition
                {
                    FromHorizontalOffset = 0,
                    FromVerticalOffset = 20
                }
            };

            m_Popup.IsLightDismissEnabled = true;
            m_Popup.IsOpen = true;

            // handle when it closes
            m_Popup.Closed -= popup_Closed;
            m_Popup.Closed += popup_Closed;

            // handle making it close
            Window.Current.Activated -= Current_Activated;
            Window.Current.Activated += Current_Activated;
        }
        public void ShowInDown(FrameworkElement element)
        {
            element.Width = Window.Current.Bounds.Width;
            element.Height = 300;
            m_Popup.Child = element;
            m_Popup.IsLightDismissEnabled = true;
            m_Popup.ChildTransitions = new Windows.UI.Xaml.Media.Animation.TransitionCollection
            {
                new Windows.UI.Xaml.Media.Animation.EntranceThemeTransition
                {
                    FromHorizontalOffset = 0,
                    FromVerticalOffset = 20
                }
            };

            m_Popup.VerticalOffset = Window.Current.Bounds.Height - 340 - 130;

            m_Popup.IsLightDismissEnabled = true;
            m_Popup.IsOpen = true;


            m_Popup.Closed -= popup_Closed;
            m_Popup.Closed += popup_Closed;

            // handle making it close
            Window.Current.Activated -= Current_Activated;
            Window.Current.Activated += Current_Activated;
        }

        protected void Current_Activated(object sender, Windows.UI.Core.WindowActivatedEventArgs e)
        {
            if (m_Popup == null)
                return;
            if (e.WindowActivationState == Windows.UI.Core.CoreWindowActivationState.Deactivated)
                m_Popup.IsOpen = false;
        }

        protected async void popup_Closed(object sender, object e)
        {
            if (CloseAction != null)
                CloseAction();
            Window.Current.Activated -= Current_Activated;
            if (m_Popup == null)
                return;
            //PrepareCloseStory();
            //await Task.Delay(1500);
            try
            {
                if (_parentElement == null) return;
                var page = _parentElement.GetParentByType<Page>();
                if (page != null && page.BottomAppBar != null)
                {
                    page.BottomAppBar.Visibility = Visibility.Visible;
                }
            }
            catch
            {

            }
            finally
            {
                m_Popup.IsOpen = false;
                m_Popup = null;
            }
        }


        void StoryReverse_Completed(object sender, object e)
        {
            m_Popup.IsOpen = false;
        }
    }
}
