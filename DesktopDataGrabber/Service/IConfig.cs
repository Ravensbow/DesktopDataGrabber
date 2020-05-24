using DesktopDataGrabber.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopDataGrabber.Service
{
    public interface IConfig
    {
        ConfigParams GetSettings();
        void ChangeSettings(ConfigParams value);
    }
}
