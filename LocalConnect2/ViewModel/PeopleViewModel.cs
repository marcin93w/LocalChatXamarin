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

        public List<Person> People { set; get; }

        public event OnDataLoadEventHandler OnDataLoad;

        public PeopleViewModel()
        {
            _peopleModel = new PeopleModel();
            People = new List<Person>();
        }

        public async void FetchData()
        {
            string errorMsg = null;
            try
            {
                People = (await _peopleModel.FetchPeopleList()).ToList();
            }
            catch (ConnectionException ex)
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