using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GalaSoft.MvvmLight;
using LocalConnect.Models;
using LocalConnect.Interfaces;

namespace LocalConnect.ViewModel
{
    public class PeopleViewModel : ViewModelBase, IDataFetchingViewModel
    {
        private readonly People _people;

        public List<Person> People => _people.PeopleList;

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
        }

        public async void FetchDataAsync()
        {
            _errorMessage = null;
            try
            {
                var fetchPeopleAsync = _people.FetchPeopleList();

                if (!await fetchPeopleAsync)
                {
                    _errorMessage = "People list could not be downloaded";
                }
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