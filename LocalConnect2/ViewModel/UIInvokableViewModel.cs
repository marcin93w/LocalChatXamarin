using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;

namespace LocalConnect2.ViewModel
{
    public class UiInvokableViewModel : MainViewModel
    {
        private readonly Action<Action> _runOnUiThreadHandler;

        public UiInvokableViewModel(Activity activity)
        {
            _runOnUiThreadHandler = activity.RunOnUiThread;
        }

        public UiInvokableViewModel(Action<Action> runOnUiThreadHandler)
        {
            _runOnUiThreadHandler = runOnUiThreadHandler;
        }

        protected void RunOnUiThread(Action action)
        {
            if (_runOnUiThreadHandler != null)
            {
                _runOnUiThreadHandler(action);
            }
        }
    }
}