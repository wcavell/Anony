using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Anony.Models;
using Anony.Primitives;

// “用户控件”项模板在 http://go.microsoft.com/fwlink/?LinkId=234236 上提供

namespace Anony.Controls
{
    public sealed partial class MenusControl : UserControl,INotifyPropertyChanged
    {
        public event EventHandler<Section> Click; 
        public MenusControl(List<KeyedList<string, Section>> s)
        {
            this.InitializeComponent();
            Sections = s;
            Width = Window.Current.Bounds.Width;
            Height = Window.Current.Bounds.Height;
            var cvs = this.Resources["CollectionViewSource"] as CollectionViewSource;
            cvs.Source = s;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private List<KeyedList<string, Section>> sections; //= new List<KeyedList<string, Section>>();

        public List<KeyedList<string, Section>> Sections
        {
            get { return sections; }
            set
            {
                sections = value;
                OnPropertyChanged();
            }
        }

        private void ListViewMenu_OnItemClick(object sender, ItemClickEventArgs e)
        {
            if(Click!=null)
                Click(this,e.ClickedItem as Section);
        }
    }
}
