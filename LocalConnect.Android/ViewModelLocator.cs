using System;
using Android.App;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using LocalConnect.ViewModel;
using LocalConnect.Interfaces;
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
        private readonly IDataProvider _dataProvider;
        private readonly SocketClient _socketClient;

        private static ViewModelLocator _instance;

        public static ViewModelLocator Instance => _instance ?? (_instance = new ViewModelLocator());

        private ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<PersonViewModel>();
            SimpleIoc.Default.Register<PeopleViewModel>();
            SimpleIoc.Default.Register<LoginViewModel>();

            _dataProvider = new RestClient();
            _socketClient = new SocketClient();
        }

        public T GetViewModel<T>(Activity activity = null, bool requestNewInstance = false) where T: ViewModelBase, new()
        {
            var viewModel = requestNewInstance ? new T() : ServiceLocator.Current.GetInstance<T>();
            
            if (viewModel is IUiInvokable)
            {
                if (activity != null)
                {
                    (viewModel as IUiInvokable).RunOnUiThread = activity.RunOnUiThread;
                }
                else
                {
                    throw new Exception("You must pass Activity object to initialize UiInvokableViewModel");
                }
            }

            if (viewModel is IDataFetchingViewModel)
            {
                (viewModel as IDataFetchingViewModel).DataProvider = _dataProvider;
            }

            if (viewModel is LoginViewModel)
            {
                (viewModel as LoginViewModel).DataProvider = _dataProvider;
            }

            if (viewModel is ISocketClientUsingViewModel)
            {
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