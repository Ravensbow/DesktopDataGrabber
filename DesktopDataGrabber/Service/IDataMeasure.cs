using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DesktopDataGrabber.Model;

namespace DesktopDataGrabber.Service
{
    public interface IDataMeasure
    {
        Task<List<MeasureValues>> GetMeasureAsync();
        Task<Joystick> GetJoystickAsync();
    }
}
