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

        private bool _dataLoaded;
        private string _errorMessage;

        private OnDataLoadEventHandler _onDataLoad;
        public event OnDataLoadEventHandler OnDataLoad
        {
            add
            {
                if (_dataLoaded)
                {
                    value(this, new OnDataLoadEventArgs(_errorMessage));
                }
                _onDataLoad += value;
            }
            remove
            {
                _onDataLoad -= value;
            }
        }

        public PeopleViewModel()
        {
            _peopleModel = new PeopleModel();
            _meModel = new MeModel();
        }

        public async void FetchData()
        {
            _errorMessage = null;
            try
            {
                var fetchPeopleAsync = _peopleModel.FetchPeopleList();
                var fetchMyNameAsync = _meModel.FetchMyName();
                People = (await fetchPeopleAsync).ToList();
                MyName = await fetchMyNameAsync;
            }
            catch (Exception ex)
            {
                _errorMessage = ex.Message;
            }
            finally
            {
                _dataLoaded = true;
                if (_onDataLoad != null)
                {
                    _onDataLoad(this, new OnDataLoadEventArgs(_errorMessage));
                }
            }
        }
    }
}