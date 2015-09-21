using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LocalConnect.Interfaces
{
    public delegate void RunOnUiThreadHandler(Action action);

    public interface IUiInvokable 
    {
        RunOnUiThreadHandler RunOnUiThread { set; }
    }
}