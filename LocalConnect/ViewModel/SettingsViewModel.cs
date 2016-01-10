using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using LocalConnect.Models;
using LocalConnect.Services;

namespace LocalConnect.ViewModel
{
    public class SettingsViewModel : ViewModelBase
    {
        private static readonly List<int> DisruptionValuesList = new [] { 0, 100, 500, 1000, 5000, 10000, 20000 }.ToList();

        private Settings _settings;
        private readonly IRestClient _restClient;

        public SettingsViewModel(IRestClient restClient)
        {
            _restClient = restClient;
        }

        public int? LocationDisruption
        {
            set
            {
                if (value != null && _settings != null)
                    _settings.LocationDisruption = value.Value;
            }
            get
            {
                return _settings?.LocationDisruption;
            }
        }

        public bool DataLoaded { set; get; }
        public bool LoadingError { set; get; }

        public bool DataSending { set; get; }
        public bool SendingError { set; get; }

        public string PeopleDisplayCountString
        {
            get { return _settings?.PeopleDisplayCount.ToString(); }
            set
            {
                if (_settings != null)
                {
                    try
                    {
                        _settings.PeopleDisplayCount = int.Parse(value);
                    }
                    catch (Exception e)
                    {
                        _settings.PeopleDisplayCount = 20;
                    }
                }
            }
        }

        public List<int> DisruptionValues => DisruptionValuesList;

        public async Task LoadSettings()
        {
            LoadingError = false;
            try
            {
                _settings = await Settings.FetchSettings(_restClient);
            }
            catch (Exception ex)
            {
                LoadingError = true;
            }
            DataLoaded = true;
        }

        public async Task UpdateSettings()
        {
            DataSending = true;
            SendingError = false;
            try
            {
                await _settings.SendUpdate(_restClient);
            }
            catch (Exception ex)
            {
                SendingError = true;
            }
            DataSending = false;
        }

    }
}
