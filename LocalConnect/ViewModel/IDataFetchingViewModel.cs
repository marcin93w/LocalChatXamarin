using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LocalConnect.ViewModel
{
    public class OnDataLoadEventArgs : EventArgs
    {
        public bool IsSuccesful { get; }

        public string ErrorMessage { get; }

        public OnDataLoadEventArgs(string errorMessage)
        {
            ErrorMessage = errorMessage;
            IsSuccesful = string.IsNullOrEmpty(errorMessage);
        }
    }

    public delegate void OnDataLoadEventHandler(object sender, OnDataLoadEventArgs e);

    public interface IDataFetchingViewModel
    {
        //string ErrorMessage { get; }
        //bool IsLoaded { get; }

        event OnDataLoadEventHandler OnDataLoad;

        void FetchData();
    }

    //public static class DataFetchingViewModelExtensions
    //{
    //    public static bool HasLoadingErrors(this IDataFetchingViewModel viewModel)
    //    {
    //        return viewModel.IsLoaded && !string.IsNullOrEmpty(viewModel.ErrorMessage);
    //    }
    //}

}