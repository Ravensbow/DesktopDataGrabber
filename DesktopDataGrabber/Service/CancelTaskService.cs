using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DesktopDataGrabber.Service
{
    class CancelTaskService : ICancelTaskService
    {
        public Dictionary<string, CancellationTokenSource> TokenSources { get; set; } = new Dictionary<string, CancellationTokenSource>();

        public void AddNew(CancellationTokenSource cts,string name)
        {
            if (!TokenSources.ContainsKey(name))
                TokenSources.Add(name, cts);
            else
            {
                TokenSources[name].Cancel();
                TokenSources[name] = cts;
            }
        }

        public void CancelAll()
        {
            TokenSources.Values.ToList().ForEach(v => v.Cancel());
        }

        public void CancelTask(string name)
        {
            if (TokenSources.ContainsKey(name))
                TokenSources[name].Cancel();
        }

        public CancellationTokenSource Get(string name)
        {
            if (TokenSources.ContainsKey(name))
                return TokenSources[name];
            else
                return null;
        }
    }
}
