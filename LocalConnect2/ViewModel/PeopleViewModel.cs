using System;
using System.Collections.Generic;
using System.Linq;
using Android.Gms.Common.Apis;
using GalaSoft.MvvmLight;
using LocalConnect2.Models;
using LocalConnect2.Services;

namespace LocalConnect2.ViewModel
{

    public class PeopleViewModel : ViewModelBase, IDataFetchingViewModel
    {
        private readonly PeopleModel _peopleModel;
        private readonly MeModel _meModel;

        public List<Person> People { set; get; }
        public string MyName { set; get; }

        public event OnDataLoadEventHandler OnDataLoad;

        public PeopleViewModel()
        {
            _peopleModel = new PeopleModel();
            _meModel = new MeModel();
        }

        public async void FetchData()
        {
            string errorMsg = null;
            try
            {
                var fetchPeopleAsync = _peopleModel.FetchPeopleList();
                var fetchMyNameAsync = _meModel.FetchMyName();
                People = (await fetchPeopleAsync).ToList();
                MyName = await fetchMyNameAsync;
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
            }
            finally
            {
                if (OnDataLoad != null)
                {
                    OnDataLoad(this, new OnDataLoadEventArgs(errorMsg));
                }
            }
        }
    }
}