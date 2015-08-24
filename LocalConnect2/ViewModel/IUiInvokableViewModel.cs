using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using GalaSoft.MvvmLight;

namespace LocalConnect2.ViewModel
{
    public delegate void RunOnUiThreadHandler(Action action);

    public interface IUiInvokableViewModel 
    {
        RunOnUiThreadHandler RunOnUiThread { set; get; }
    }
}