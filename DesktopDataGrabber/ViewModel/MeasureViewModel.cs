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
using DesktopDataGrabber.Service;
using DesktopDataGrabber.Tools;
using System.Collections.Generic;
using DesktopDataGrabber.Model;
using System.Threading;
using System.Windows.Controls;

namespace DesktopDataGrabber.ViewModel
{
    class MeasureViewModel : INotifyPropertyChanged
    {
        private IConfig config;
        private IDataMeasure dataMeasureService;

        
        private CancellationTokenSource source;
        private CancellationToken cts;
        private Dictionary<string, Label> value_labels = new Dictionary<string, Label>();
        private Dictionary<string, Label> datelabel_labels = new Dictionary<string, Label>();
        private List<MeasureValues> _temp;
        public StackPanel MeasureStack { get; set; } = new StackPanel() { Margin = new System.Windows.Thickness(40, 0, 0, 0) };
        public List<MeasureValues> Temp
        {
            get
            {
                return _temp;
            }
            set
            {
                _temp = value;
                OnPropertyChanged(nameof(MeasureValues));
            }
        }

        public MeasureViewModel(IConfig configuration, IDataMeasure dataMeasure)
        {
            config = configuration;
            dataMeasureService = dataMeasure;          
        }
        ~MeasureViewModel()
        {
            Cancle();
        }

        public async Task GetData()
        {
            source = new CancellationTokenSource();
            cts = source.Token;
            while (true)
            {
                if (cts.IsCancellationRequested)
                    return;
                Temp = await dataMeasureService.GetMeasureAsync();
                if (Temp == null)
                    continue;
                foreach (var v in Temp)
                {
                    if (!haveTag(MeasureStack.Children,v.Name))
                    {
                        MeasureStack.Children.Add(SetMeasureView(v));
                        OnPropertyChanged("MeasureStack");
                    }
                    else
                    {
                        if (value_labels.ContainsKey(v.Name))
                        {
                            value_labels[v.Name].Content = v.Value.ToString() + " " + v.Unit;
                            OnPropertyChanged("MeasureStack");
                        }
                        if (datelabel_labels.ContainsKey(v.Name))
                        {
                            datelabel_labels[v.Name].Content = DateTime.Now.ToString("HH:mm:ss dd.MM.y");
                            OnPropertyChanged("MeasureStack");
                        }
                    }

                }
                await Task.Delay(1000);
            }
        }

        private StackPanel SetMeasureView(MeasureValues v)
        {
            var S = new StackPanel() { Tag = v.Name };

            var s = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                Tag = v.Name
            };
            s.Children.Add(new Label()
            {
                Content = v.Name,
                FontSize = 20
            });
            var dtlabel = new Label()
            {
                Content = DateTime.Now.ToString("HH:mm:ss dd.MM.y"),
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Right
            };
            s.Children.Add(dtlabel);
            var vlabel = new Label()
            {
                Content = v.Value.ToString() + " " + v.Unit,
                FontSize = 25,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                Margin = new System.Windows.Thickness(0, 0, 20, 0)
            };
            if (!value_labels.ContainsKey(v.Name))
                value_labels.Add(v.Name, vlabel);
            if (!datelabel_labels.ContainsKey(v.Name))
                datelabel_labels.Add(v.Name, dtlabel);
            S.Children.Add(s);
            S.Children.Add(vlabel);

            return S;
        }
        private bool haveTag( UIElementCollection collection,string value)
        {
            foreach(var item in collection)
            {
                if ((string)((StackPanel)item).Tag == value)
                    return true;
            }
            return false;
        }

        private void Cancle()
        {
            source.Cancel();
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
