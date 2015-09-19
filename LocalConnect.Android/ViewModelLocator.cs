using System;
using Android.App;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using LocalConnect2.ViewModel;
using Microsoft.Practices.ServiceLocation;

namespace LocalConnect2
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        private static ViewModelLocator _instance;

        public static ViewModelLocator Instance => _instance ?? (_instance = new ViewModelLocator());

        private ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<ChatViewModel>();
            SimpleIoc.Default.Register<PeopleViewModel>();
            SimpleIoc.Default.Register<LoginViewModel>();
        }

        public T GetViewModel<T>(Activity activity = null) where T: ViewModelBase
        {
            var viewModel = ServiceLocator.Current.GetInstance<T>();

            if (viewModel is IUiInvokableViewModel)
            {
                if (activity != null)
                {
                    (viewModel as IUiInvokableViewModel).RunOnUiThread = activity.RunOnUiThread;
                }
                else
                {
                    throw new Exception("You must pass Activity object to initialize UiInvokableViewModel");
                }
            }

            return viewModel;
        } 
        
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}