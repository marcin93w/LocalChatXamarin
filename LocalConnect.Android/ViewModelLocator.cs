using System;
using Android.App;
using Android.Content;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using LocalConnect.Android.Views.Helpers;
using LocalConnect.ViewModel;
using LocalConnect.Helpers;
using LocalConnect.Services;
using Microsoft.Practices.ServiceLocation;

namespace LocalConnect.Android
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        private IRestClient _restClient;
        private SocketClient _socketClient;

        private static ViewModelLocator _instance;

        public static ViewModelLocator Instance => _instance ?? (_instance = new ViewModelLocator());

        private ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<PersonChatViewModel>();
            SimpleIoc.Default.Register<PeopleViewModel>();
            SimpleIoc.Default.Register<LoginViewModel>();
            SimpleIoc.Default.Register<MyProfileViewModel>();
            SimpleIoc.Default.Register<SettingsViewModel>();
        }

        public T GetUiInvokableViewModel<T>(Activity activity) where T : ViewModelBase, IUiInvokable
        {
            var viewModel = GetViewModel<T>(activity);
            viewModel.RunOnUiThread = activity.RunOnUiThread;

            return viewModel;
        }

        public T GetViewModel<T>(Context context) where T: ViewModelBase
        {
            var viewModel = ServiceLocator.Current.GetInstance<T>();

            if (viewModel is IRestClientUsingViewModel)
            {
                if (_restClient == null)
                {
                    _restClient = new RestClient(new SessionInfoManager(context));
                }
                (viewModel as IRestClientUsingViewModel).RestClient = _restClient;
            }

            if (viewModel is ISocketClientUsingViewModel)
            {
                if (_socketClient == null)
                {
                    _socketClient = new SocketClient(new SessionInfoManager(context));
                }
                (viewModel as ISocketClientUsingViewModel).SocketClient = _socketClient;
            }

            return viewModel;
        }

        public void ResetViewModel<T>() where T : ViewModelBase
        {
            if (SimpleIoc.Default.IsRegistered<T>())
                SimpleIoc.Default.Unregister<T>();
            SimpleIoc.Default.Register<T>();
        }

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}