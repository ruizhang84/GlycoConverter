using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlycoConverter
{

    public class ProgressingEventArgs : EventArgs
    {
        public int Total { get; set; }
    }

    public class ProgressingCounter
    {
        public event EventHandler<ProgressingEventArgs> progressChange;

        protected virtual void OnProgressChanged(ProgressingEventArgs e)
        {
            EventHandler<ProgressingEventArgs> handler = progressChange;
            handler?.Invoke(this, e);
        }

        public void Add(int total)
        {
            ProgressingEventArgs e = new ProgressingEventArgs
            {
                Total = total
            };
            OnProgressChanged(e);
        }
    }

    public class Counter
    {
        public event EventHandler<EventArgs> progressChange;

        protected virtual void OnProgressChanged(EventArgs e)
        {
            EventHandler<EventArgs> handler = progressChange;
            handler?.Invoke(this, e);
        }

        public void Add()
        {
            EventArgs e = new EventArgs();
            OnProgressChanged(e);
        }
    }
}
