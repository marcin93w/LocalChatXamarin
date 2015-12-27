using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using LocalConnect.Models;
using LocalConnect.Services;

namespace LocalConnect.ViewModel
{
    public class SettingsViewModel : ViewModelBase, IRestClientUsingViewModel
    {
        private static readonly List<int> DisruptionValuesList = new [] { 0, 100, 500, 1000, 5000, 10000, 50000 }.ToList();

        private Settings _settings;
        public IRestClient RestClient { get; set; }

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
                _settings = await Settings.FetchSettings(RestClient);
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
                await _settings.SendUpdate(RestClient);
            }
            catch (Exception ex)
            {
                SendingError = true;
            }
            DataSending = false;
        }

    }
}
