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
    using DesktopDataGrabber.Tools;
    using Model;
    using System.Collections.Generic;


    /** 
      * @brief View model for MainWindow.xaml 
      */
    public class PrzebiegiViewModel : INotifyPropertyChanged
    {
        #region Properties
        private string ipAddress;
        public string IpAddress
        {
            get
            {
                return ipAddress;
            }
            set
            {
                if (ipAddress != value)
                {
                    ipAddress = value;
                    OnPropertyChanged("IpAddress");
                }
            }
        }
        private int sampleTime;
        public string SampleTime
        {
            get
            {
                return sampleTime.ToString();
            }
            set
            {
                if (Int32.TryParse(value, out int st))
                {
                    if (sampleTime != st)
                    {
                        sampleTime = st;
                        OnPropertyChanged("SampleTime");
                    }
                }
            }
        }
        ~PrzebiegiViewModel()
        {
            Cancle();
        }
        
        public PlotModel DataPlotModel { get; set; }
        public ButtonCommand StartButton { get; set; }
        public IAsyncCommand StartButtonAsync{ get; set; }
        public ButtonCommand StopButton { get; set; }
        public ButtonCommand StopButton2 { get; set; }
        public ButtonCommand UpdateConfigButton { get; set; }
        public ButtonCommand DefaultConfigButton { get; set; }
        #endregion

        #region Fields
        private int timeStamp = 0;
        private IConfig config;
        private IDataMeasure dataMeasureService;
        //private IoTServer Server;
        private System.Threading.CancellationTokenSource source { get; set; }
        private System.Threading.CancellationToken cts { get; set; }
        #endregion

        public PrzebiegiViewModel(IConfig configuration, IDataMeasure dataMeasure)
        {
            config = configuration;
            dataMeasureService = dataMeasure;
            DataPlotModel = new PlotModel { Title = "Przebiegi czasowe" };
            source = new System.Threading.CancellationTokenSource();
            cts = source.Token;
            DataPlotModel.Axes.Add(new LinearAxis()
            {
                Position = AxisPosition.Bottom,
                Minimum = 0,
                Maximum = config.GetSettings().XAxisMax,
                Key = "Horizontal",
                Unit = "sec",
                Title = "Time"
            });
            DataPlotModel.Axes.Add(new LinearAxis()
            {
                Position = AxisPosition.Left,
                Minimum = -100,
                Maximum = 100,
                Key = "Temp",
                Unit = "C",
                Title = "temperature",
                TitlePosition=0.3
            });
            DataPlotModel.Axes.Add(new LinearAxis()
            {
                Position = AxisPosition.Right,
                Minimum = 0,
                Maximum = 4000,
                Key = "Press",
                Unit = "hPa",
                Title = "preasure"
            });
            DataPlotModel.Axes.Add(new LinearAxis()
            {
                Position = AxisPosition.Left,
                Minimum = 0,
                Maximum = 100,
                Key = "Hum",
                Unit = "%",
                Title = "humidity",
                AxisDistance = 30.0,
                TitlePosition=0.7,
                TitleClippingLength =1.2,
                Angle=40,
            });

            DataPlotModel.Series.Add(new LineSeries() { Title = "Temperature", Color = OxyColor.Parse("#FFFF00FF"),YAxisKey = "Temp", XAxisKey= "Horizontal" });
            DataPlotModel.Series.Add(new LineSeries() { Title = "Preasure", Color = OxyColor.Parse("#FFFFFF00"), YAxisKey = "Press", XAxisKey = "Horizontal" });
            DataPlotModel.Series.Add(new LineSeries() { Title = "Humidity", Color = OxyColor.Parse("#FFFF0000"), YAxisKey = "Hum", XAxisKey = "Horizontal" });

           
            StartButtonAsync = new AsyncCommand(Start);
            StopButton2 = new ButtonCommand(Cancle);
            UpdateConfigButton = new ButtonCommand(UpdateConfig);
            DefaultConfigButton = new ButtonCommand(DefaultConfig);

            ipAddress = config.GetSettings().IpAddress;
            sampleTime = config.GetSettings().SampleTime;
        }

        public async Task UpdatePlotAsync()
        {
            while (true)
            {
                if (cts.IsCancellationRequested)
                    return;
                
                var measureData = await dataMeasureService.GetMeasureAsync();
                if (measureData == null)
                    continue;

                UpdateChart(measureData);

                await Task.Delay(config.GetSettings().SampleTime);
            }
        }
        private void UpdateChart(List<MeasureValues> ms)
        {
            var temp = ms.Find(m => m.Name == "Temperatura");
            var hum = ms.Find(m => m.Name == "Wilgotność");
            var pres = ms.Find(m => m.Name == "Ciśnienie");
            if (temp == null || hum == null || pres == null)
                return;
            UpdatePlot(timeStamp / 1000.0, (double)temp.Value, (double)pres.Value, (double)hum.Value);
            timeStamp += config.GetSettings().SampleTime;
        }

        public void Cancle()
        {
            source.Cancel();
        }
        public async Task Start()
        {
            source = new System.Threading.CancellationTokenSource();
            cts = source.Token;
            await UpdatePlotAsync();

        }
        /**
          * @brief Time series plot update procedure.
          * @param t X axis data: Time stamp [ms].
          * @param d Y axis data: Real-time measurement [-].
          */
        private void UpdatePlot(double t, double d,double p,double h)
        {
            LineSeries lineSeriesTemp = DataPlotModel.Series[0] as LineSeries;
            lineSeriesTemp.Points.Add(new DataPoint(t, d));
            if (lineSeriesTemp.Points.Count > config.GetSettings().MaxSampleNumber)
                lineSeriesTemp.Points.RemoveAt(0);

            LineSeries lineSeriesPress = DataPlotModel.Series[1] as LineSeries;
            lineSeriesPress.Points.Add(new DataPoint(t, p));
            if (lineSeriesPress.Points.Count > config.GetSettings().MaxSampleNumber)
                lineSeriesPress.Points.RemoveAt(0);

            LineSeries lineSeriesHum = DataPlotModel.Series[2] as LineSeries;
            lineSeriesHum.Points.Add(new DataPoint(t, h));
            if (lineSeriesHum.Points.Count > config.GetSettings().MaxSampleNumber)
                lineSeriesHum.Points.RemoveAt(0);
            if (t >= config.GetSettings().XAxisMax)
            {
                DataPlotModel.Axes[0].Minimum = (t - config.GetSettings().XAxisMax);
                DataPlotModel.Axes[0].Maximum = t + config.GetSettings().SampleTime / 1000.0; ;
            }

            DataPlotModel.InvalidatePlot(true);
        }

        #region ButtonCommands

      
        /**
         * @brief Configuration parameters update
         */
        private void UpdateConfig()
        {
            config.ChangeSettings(new ConfigParams(ipAddress, sampleTime));
        }

        /**
          * @brief Configuration parameters defualt values
          */
        private void DefaultConfig()
        {
            config.ChangeSettings(new ConfigParams());
            IpAddress = config.GetSettings().IpAddress;
            SampleTime = config.GetSettings().SampleTime.ToString();
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
