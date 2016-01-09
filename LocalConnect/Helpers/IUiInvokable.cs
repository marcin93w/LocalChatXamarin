using System;

namespace LocalConnect.Helpers
{
    public delegate void RunOnUiThreadHandler(Action action);

    public interface IUiInvokable 
    {
        RunOnUiThreadHandler RunOnUiThread { set; }
    }
}