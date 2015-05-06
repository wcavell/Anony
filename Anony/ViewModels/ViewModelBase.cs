using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Caliburn.Micro;

namespace Anony 
{
    public abstract class ViewModelBase : Screen
    {
        public readonly INavigationService navigationService;

        public bool Spare { get; set; }
        protected ViewModelBase(INavigationService navigationService)
        {
            this.navigationService = navigationService;
        }

        public void GoBack()
        {
            navigationService.GoBack();
        }

       
        public bool CanGoBack
        {
            get
            {
                return navigationService.CanGoBack;
            }
        }
    }
}
public static class ViewModelHelper
{
    public static bool Set<TProperty>(
        this INotifyPropertyChangedEx This,
        ref TProperty backingField,
        TProperty newValue,
        [CallerMemberName] string propertyName = null)
    {
        if (This == null)
            throw new ArgumentNullException("This");
        if (string.IsNullOrEmpty(propertyName))
            throw new ArgumentNullException("propertyName");

        if (EqualityComparer<TProperty>.Default.Equals(backingField, newValue))
            return false;

        backingField = newValue;
        This.NotifyOfPropertyChange(propertyName);
        return true;
    }
}