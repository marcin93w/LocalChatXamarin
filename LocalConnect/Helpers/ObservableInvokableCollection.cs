using System;
using System.Collections.ObjectModel;

namespace LocalConnect.Helpers
{
    public class ObservableInvokableCollection<T> : ObservableCollection<T>, IUiInvokable
    {
        public RunOnUiThreadHandler RunOnUiThread { get; set; }

        public ObservableInvokableCollection(RunOnUiThreadHandler runOnUiThread)
        {
            RunOnUiThread = runOnUiThread;
        }

        public new void Add(T element)
        {
            var baseAdd = new Action<T>(base.Add);
            RunOnUiThread(() => baseAdd(element));
        }
    }
}
