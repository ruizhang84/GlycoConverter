using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlycoConverter
{
    public class Counter
    {
        public event EventHandler progressChange;

        protected virtual void OnProgressChanged(EventArgs e)
        {
            EventHandler handler = progressChange;
            handler?.Invoke(this, e);
        }

        public void Add()
        {
            EventArgs e = new EventArgs();
            OnProgressChanged(e);
        }
    }
}
