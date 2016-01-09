using System;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using LocalConnect.Models;
using LocalConnect.Services;

namespace LocalConnect.ViewModel
{
    public class MyProfileViewModel : ViewModelBase
    {
        private Me _me;

        private readonly IRestClient _restClient;

        public MyProfileViewModel(IRestClient restClient)
        {
            _restClient = restClient;
        }

        public string FirstName
        {
            set { _me.FirstName = value; }
            get { return _me.FirstName; }
        }

        public string Surname
        {
            set { _me.Surname = value; }
            get { return _me.Surname; }
        }

        public string ShortDesription
        {
            set { _me.ShortDescription = value; }
            get { return _me.ShortDescription; }
        }

        public string LongDescription
        {
            set { _me.LongDescription = value; }
            get { return _me.LongDescription; }
        }

        public string Avatar => _me.Avatar;

        public bool DataLoaded { private set; get; }
        public bool IsInitialized => _me != null;
        public string Id => _me?.PersonId;

        public void Initialize(Me me)
        {
            _me = me;
        }

        public async Task<bool> LoadData()
        {
            try
            {
                await _me.LoadDetailedData(_restClient);
                DataLoaded = true;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> Save()
        {
            try
            {
                await _me.UpdateData(_restClient);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
