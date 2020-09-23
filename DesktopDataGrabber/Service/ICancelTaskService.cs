using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DesktopDataGrabber.Service
{
    public interface ICancelTaskService
    {
        CancellationTokenSource Get(string name);
        void CancelTask(string tokenSourceName);
        void AddNew(CancellationTokenSource cts,string name);
        void CancelAll();
    }
}
