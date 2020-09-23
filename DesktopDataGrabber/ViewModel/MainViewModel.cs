#define CLIENT
#define GET
#define DYNAMIC

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Timers;
using System.Net.Http;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DesktopDataGrabber.ViewModel
{
    using DesktopDataGrabber.Service;
    using DesktopDataGrabber.View.Partial;
    using Model;
    using System.Windows.Controls;

    /** 
      * @brief View model for MainWindow.xaml 
      */
    public class MainViewModel : INotifyPropertyChanged
    {
        #region Properties
        public ButtonCommand ChartButton { get; set; }
        public ButtonCommand DisplayButton { get; set; }
        public ButtonCommand MeasureButton { get; set; }
        public ButtonCommand JoystickButton { get; set; }
        public Page page { get; set; }
        #endregion

        #region Fields
        IConfig config;
        IPanelLED panelLedService;
        IDataMeasure dataMeasureService;
        ICancelTaskService cancelTaskService;
        #endregion

        public MainViewModel(IConfig c, IPanelLED pl, IDataMeasure dm, ICancelTaskService cts)
        {
            ChartButton = new ButtonCommand(GoToChart);
            DisplayButton = new ButtonCommand(GoToLED);
            MeasureButton = new ButtonCommand(GoToMeasure);
            JoystickButton = new ButtonCommand(GoToJoystick);
            config = c;
            panelLedService = pl;
            dataMeasureService = dm;
            cancelTaskService = cts;
            page = new MeasurePage(config,dataMeasureService,cancelTaskService);
        }

        #region ButtonCommands
        private void GoToChart()
        {
            cancelTaskService.CancelAll();
            page = new PrzebiegiPage(config,dataMeasureService, cancelTaskService);
            OnPropertyChanged("page");
        }
        private void GoToLED()
        {
            cancelTaskService.CancelAll();
            page = new LEDPage(config,panelLedService);
            OnPropertyChanged("page");
        }
        private void GoToMeasure()
        {
            cancelTaskService.CancelAll();
            page = new MeasurePage(config, dataMeasureService, cancelTaskService);
            OnPropertyChanged("page");
        }
        private void GoToJoystick()
        {
            cancelTaskService.CancelAll();
            page = new JoystickPage(config, dataMeasureService, cancelTaskService);
            OnPropertyChanged("page");
        }
        #endregion

        #region PropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        /**
         * @brief Simple function to trigger event handler
         * @params propertyName Name of ViewModel property as string
         */
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
