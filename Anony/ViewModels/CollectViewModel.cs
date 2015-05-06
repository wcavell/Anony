using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.ViewManagement;
using Anony.Controls;
using Anony.Models;
using Caliburn.Micro;

namespace Anony.ViewModels
{
    public class CollectViewModel:ViewModelBase
    {
        public CollectViewModel(INavigationService navigationService) : base(navigationService)
        {
            
        }

        private int page = 1;
        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            var item = await CollectlData.GetData();
            Bunches=new ObservableCollection<Bunch>(item);
            Info =await DataService.GetFeeds(Feeds, page);
        }

        protected override void OnViewReady(object view)
        {
            base.OnViewReady(view);
            StatusBar.GetForCurrentView().ProgressIndicator.Text = "     ";
            StatusBar.GetForCurrentView().ProgressIndicator.ProgressValue = 0;
        }

        public void DeleteClick(Bunch bunch)
        {
            Bunches.Remove(bunch);
            CollectlData.SaveData(Bunches);
        }

       
        public void DeleteAll()
        {
            CollectlData.DeleteAll();
        }

        public void BunchClick(Bunch bunch)
        {
            navigationService.UriFor<DetailViewModel>()
                .WithParam(x=>x.Id,bunch.Id)
                .WithParam(x=>x.Spare,bunch.Spare)
                .Navigate();
        }
        public void FeedBunchClick(Bunch bunch)
        {
            navigationService.UriFor<DetailViewModel>()
                .WithParam(x=>x.Id,bunch.ThreadsId)
                .WithParam(x=>x.Spare,false)
                .Navigate();
        }

        public async void FeedDeleteClick(Bunch bunch)
        {
            var info =await DataService.DeleteFeed(bunch.ThreadsId);
            if (info != null)
            {
                if(info.Success)
                    Toast.Show(info.Data);
                else 
                    Toast.ShowError(info.Data);
            }
            else
            {
                Toast.ShowError("发生未知错误");
            }
        }
        private ObservableCollection<Bunch> _bunches;

        public ObservableCollection<Bunch> Bunches
        {
            get { return _bunches; }
            set { this.Set(ref _bunches, value); }
        } 
        private ObservableCollection<Bunch> _collection=new BindableCollection<Bunch>();

        public ObservableCollection<Bunch> Feeds
        {
            get { return _collection; }
            set { this.Set(ref _collection, value); }
        }

        private AcInfo Info;
    }
}
