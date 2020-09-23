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
        public Page page { get; set; }
        #endregion

        #region Fields
        IConfig config;
        IPanelLED panelLedService;
        IDataMeasure dataMeasureService; 
        #endregion

        public MainViewModel(IConfig c, IPanelLED pl, IDataMeasure dm)
        {
            ChartButton = new ButtonCommand(GoToChart);
            DisplayButton = new ButtonCommand(GoToLED);
            MeasureButton = new ButtonCommand(GoToMeasure);
            config = c;
            panelLedService = pl;
            dataMeasureService = dm;
            page = new MeasurePage(config,dataMeasureService);
        }

        #region ButtonCommands
        private void GoToChart()
        {
            page = new PrzebiegiPage(config,dataMeasureService);
            OnPropertyChanged("page");
        }
        private void GoToLED()
        {
            page = new LEDPage(config,panelLedService);
            OnPropertyChanged("page");
        }
        private void GoToMeasure()
        {
            page = new MeasurePage(config, dataMeasureService);
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
