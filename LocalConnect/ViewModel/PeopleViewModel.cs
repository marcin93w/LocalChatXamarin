using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GalaSoft.MvvmLight;
using LocalConnect.Models;
using LocalConnect.Interfaces;
using LocalConnect.Services;

namespace LocalConnect.ViewModel
{
    public class PeopleViewModel : ViewModelBase, IDataFetchingViewModel
    {
        private readonly People _people;

        public List<Person> People => _people.PeopleList;

        private bool _dataLoaded;
        private string _errorMessage;

        private OnDataLoadEventHandler _onDataLoad;
        public IDataProvider DataProvider { private get; set; }

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
            bool authTokenMissing = false;
            try
            {
                await _people.FetchPeopleList(DataProvider);
            }
            catch (Exception ex)
            {
                _errorMessage = ex.Message;
                authTokenMissing = ex is MissingAuthenticationTokenException;
            }
            finally
            {
                _dataLoaded = true;
                if (_onDataLoad != null)
                {
                    _onDataLoad(this, new OnDataLoadEventArgs(_errorMessage, authTokenMissing));
                }
            }
        }
    }
}