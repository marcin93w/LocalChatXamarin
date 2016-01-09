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
        private Context _context;

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
            SimpleIoc.Default.Register<Context>(() => _context);
            SimpleIoc.Default.Register<ISessionInfoManager, SessionInfoManager>();
            SimpleIoc.Default.Register<IRestClient, RestClient>();
            SimpleIoc.Default.Register<ISocketClient, SocketClient>();
        }

        public T GetUiInvokableViewModel<T>(Activity activity) where T : ViewModelBase, IUiInvokable
        {
            var viewModel = GetViewModel<T>(activity);
            viewModel.RunOnUiThread = activity.RunOnUiThread;

            return viewModel;
        }

        public T GetViewModel<T>(Context context) where T: ViewModelBase
        {
            _context = context;
            return SimpleIoc.Default.GetInstance<T>();
        }

        public void ResetViewModel<T>() where T : ViewModelBase
        {
            if (SimpleIoc.Default.IsRegistered<T>())
                SimpleIoc.Default.Unregister<T>();
            SimpleIoc.Default.Register<T>();
        }
    }

    public interface IContextProvider
    {
    }
}