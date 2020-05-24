using DesktopDataGrabber.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopDataGrabber.Service
{
    public class Config : IConfig
    {
        ConfigParams settings = new ConfigParams();

        public void ChangeSettings(ConfigParams value)
        {
            settings = value;
        }

        public ConfigParams GetSettings()
        {
            return settings;
        }
    }
}
