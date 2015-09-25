using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GalaSoft.MvvmLight;
using LocalConnect.Models;
using LocalConnect.Android.Models;
using LocalConnect.Interfaces;

namespace LocalConnect.ViewModel
{
    public class PeopleViewModel : ViewModelBase, IDataFetchingViewModel, IPeopleFinder
    {
        private readonly People _people;
        private readonly MeModel _meModel;

        public List<Person> People => _people.PeopleList;
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
            _people = new People();
            _meModel = new MeModel();
        }

#region IPeopleFinder implementation

        public Person GetPersonForId(string personId)
        {
            if(!_dataLoaded || People == null)
                throw new InvalidAsynchronousStateException("Data has not been loaded yet");

            return People.First(p => p.PersonId == personId);
        }

#endregion

        public async void FetchDataAsync()
        {
            _errorMessage = null;
            try
            {
                var fetchPeopleAsync = _people.FetchPeopleList();
                var fetchMyNameAsync = _meModel.FetchMyName();

                if (!await fetchPeopleAsync)
                {
                    _errorMessage = "People list could not be downloaded";
                }

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