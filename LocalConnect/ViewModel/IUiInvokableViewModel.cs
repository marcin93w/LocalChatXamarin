using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LocalConnect2.ViewModel
{
    public delegate void RunOnUiThreadHandler(Action action);

    public interface IUiInvokableViewModel 
    {
        RunOnUiThreadHandler RunOnUiThread { set; get; }
    }
}