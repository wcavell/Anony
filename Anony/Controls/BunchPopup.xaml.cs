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

// “用户控件”项模板在 http://go.microsoft.com/fwlink/?LinkId=234236 上提供

namespace Anony.Controls
{
    public sealed partial class BunchPopup : UserControl,INotifyPropertyChanged
    {
        private Bunch _bunch;

        public Bunch Bunch
        {
            get { return _bunch; }
            set
            {
                _bunch = value;
                OnPropertyChanged();
            }
        }
        public BunchPopup()
        {
            this.InitializeComponent();
           
        }

        public void SetBunch(Bunch bun)
        {
            Bunch = bun;
            RootGrid.DataContext = bun;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
