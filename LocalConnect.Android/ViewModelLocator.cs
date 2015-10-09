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
        private readonly ChatClient _chatClient;

        private static ViewModelLocator _instance;

        public static ViewModelLocator Instance => _instance ?? (_instance = new ViewModelLocator());

        private ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<ChatViewModel>();
            SimpleIoc.Default.Register<PeopleViewModel>();
            SimpleIoc.Default.Register<LoginViewModel>();

            _dataProvider = new RestClient();
            _chatClient = new ChatClient();
        }

        public T GetViewModel<T>(Activity activity = null) where T: ViewModelBase
        {
            var viewModel = ServiceLocator.Current.GetInstance<T>();

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

            if (viewModel is IChatClientUsingViewModel)
            {
                (viewModel as IChatClientUsingViewModel).ChatClient = _chatClient;
            }

            return viewModel;
        } 
        
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}