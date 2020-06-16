using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DesktopDataGrabber.Service
{
    public interface IPanelLED
    {
        int[] GetLEDsState();
        Task<bool> SetLEDs(int[] leds);
    }
}
