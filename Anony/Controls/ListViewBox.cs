using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// “用户控件”项模板在 http://go.microsoft.com/fwlink/?LinkId=234235 上有介绍

namespace Anony.Controls
{
    public  class ListViewBox : ListBox, ISemanticZoomInformation
    {
        public ListViewBox()
            : base()
        {
            this.DefaultStyleKey = typeof(ListViewBox);
            //IsActiveView = true;
            //IsZoomedInView = true;
            
            Loaded += ListViewBox_Loaded;
            SelectionChanged += ListViewBox_SelectionChanged;
        }

        void ListViewBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        void ListViewBox_Loaded(object sender, RoutedEventArgs e)
        {
             
        }

        public void CompleteViewChange()
        {
           
        }

        public void CompleteViewChangeFrom(SemanticZoomLocation source, SemanticZoomLocation destination)
        {
           
        }

        public void CompleteViewChangeTo(SemanticZoomLocation source, SemanticZoomLocation destination)
        {
          
        }

        public void InitializeViewChange()
        {
            
        }
        
        public bool IsActiveView
        {
            get; 
            set;
            
        }

        public bool IsZoomedInView { get; set; }

        public void MakeVisible(SemanticZoomLocation item)
        {
            this.ItemsSource = item.Item;
        }

        public SemanticZoom SemanticZoomOwner { get; set; }

        public void StartViewChangeFrom(SemanticZoomLocation source, SemanticZoomLocation destination)
        {
            if (this.IsZoomedInView)
            {
                source.Item = this.SelectedItem;
                destination.Item = this.SelectedItem.ToString()[0].ToString();
            }
        }

        public void StartViewChangeTo(SemanticZoomLocation source, SemanticZoomLocation destination)
        {
            
        }
    }
}
