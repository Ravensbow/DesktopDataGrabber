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
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows;

namespace DesktopDataGrabber.ViewModel
{
    class JoystickViewModel : INotifyPropertyChanged
    {
        private IConfig config;
        private IDataMeasure dataMeasureService;
        private ICancelTaskService cancelTaskService;
        private Joystick actualState;
        public StackPanel Cross { get; set; }

        public SolidColorBrush boxUp { get; set; } = new SolidColorBrush(System.Windows.Media.Colors.Gray);
        public SolidColorBrush boxRight { get; set; } = new SolidColorBrush(System.Windows.Media.Colors.Gray);
        public SolidColorBrush boxDown { get; set; } = new SolidColorBrush(System.Windows.Media.Colors.Gray);
        public SolidColorBrush boxLeft { get; set; } = new SolidColorBrush(System.Windows.Media.Colors.Gray);
        public SolidColorBrush boxCenter { get; set; } = new SolidColorBrush(System.Windows.Media.Colors.Gray);

        private CancellationTokenSource source;
        private CancellationToken cts;

        public JoystickViewModel(IConfig configuration, IDataMeasure dataMeasure, ICancelTaskService ctss)
        {
            config = configuration;
            cancelTaskService = ctss;
            dataMeasureService = dataMeasure;
        }
        public async Task UpdateJoystickAsync()
        {
            cancelTaskService.AddNew(new System.Threading.CancellationTokenSource(), "joystick");
            source = cancelTaskService.Get("joystick");
            cts = source.Token;
            while (true)
            {
                if (cts.IsCancellationRequested)
                    return;

                var joyS = await dataMeasureService.GetJoystickAsync();
                if (joyS == null)
                    continue;

                Update(joyS);
                await Task.Delay(30);
            }
        }

        private void Update(Joystick js)
        {
            if (actualState != null && js.direction != actualState.direction)
            {
                switch (actualState.direction)
                {
                    case JoystickStatu.Down:
                        boxDown.Color = System.Windows.Media.Colors.Gray;
                        OnPropertyChanged("boxDown");
                        break;
                    case JoystickStatu.Up:
                        boxUp.Color = System.Windows.Media.Colors.Gray;
                        OnPropertyChanged("boxUp");
                        break;
                    case JoystickStatu.Right:
                        boxRight.Color = System.Windows.Media.Colors.Gray;
                        OnPropertyChanged("boxRight");
                        break;
                    case JoystickStatu.Left:
                        boxLeft.Color = System.Windows.Media.Colors.Gray;
                        OnPropertyChanged("boxLeft");
                        break;
                    case JoystickStatu.Middle:
                        boxCenter.Color = System.Windows.Media.Colors.Gray;
                        OnPropertyChanged("boxCenter");
                        break;
                }
            }
            switch (js.direction)
            {
                case JoystickStatu.Down:
                    switch (js.action)
                    {
                        case JoystickEvent.Pressed:
                            boxDown.Color = System.Windows.Media.Colors.LightGray;
                            OnPropertyChanged("boxDown");
                            break;
                        case JoystickEvent.Held:
                            boxDown.Color = System.Windows.Media.Colors.WhiteSmoke;
                            OnPropertyChanged("boxDown");
                            break;
                        case JoystickEvent.Released:
                            boxDown.Color = System.Windows.Media.Colors.Gray;
                            OnPropertyChanged("boxDown");
                            break;
                    }
                    break;
                case JoystickStatu.Up:
                    switch (js.action)
                    {
                        case JoystickEvent.Pressed:
                            boxUp.Color = System.Windows.Media.Colors.LightGray;
                            OnPropertyChanged("boxUp");
                            break;
                        case JoystickEvent.Held:
                            boxUp.Color = System.Windows.Media.Colors.WhiteSmoke;
                            OnPropertyChanged("boxUp");
                            break;
                        case JoystickEvent.Released:
                            boxUp.Color = System.Windows.Media.Colors.Gray;
                            OnPropertyChanged("boxUp");
                            break;
                    }
                    break;
                case JoystickStatu.Right:
                    switch (js.action)
                    {
                        case JoystickEvent.Pressed:
                            boxRight.Color = System.Windows.Media.Colors.LightGray;
                            OnPropertyChanged("boxRight");
                            break;
                        case JoystickEvent.Held:
                            boxRight.Color = System.Windows.Media.Colors.WhiteSmoke;
                            OnPropertyChanged("boxRight");
                            break;
                        case JoystickEvent.Released:
                            boxRight.Color = System.Windows.Media.Colors.Gray;
                            OnPropertyChanged("boxRight");
                            break;
                    }
                    break;
                case JoystickStatu.Left:
                    switch (js.action)
                    {
                        case JoystickEvent.Pressed:
                            boxLeft.Color = System.Windows.Media.Colors.LightGray;
                            OnPropertyChanged("boxLeft");
                            break;
                        case JoystickEvent.Held:
                            boxLeft.Color = System.Windows.Media.Colors.WhiteSmoke;
                            OnPropertyChanged("boxLeft");
                            break;
                        case JoystickEvent.Released:
                            boxLeft.Color = System.Windows.Media.Colors.Gray;
                            OnPropertyChanged("boxLeft");
                            break;
                    }
                    break;
                case JoystickStatu.Middle:
                    switch (js.action)
                    {
                        case JoystickEvent.Pressed:
                            boxCenter.Color = System.Windows.Media.Colors.LightGray;
                            OnPropertyChanged("boxCenter");
                            break;
                        case JoystickEvent.Held:
                            boxCenter.Color = System.Windows.Media.Colors.WhiteSmoke;
                            OnPropertyChanged("boxCenter");
                            break;
                        case JoystickEvent.Released:
                            boxCenter.Color = System.Windows.Media.Colors.Gray;
                            OnPropertyChanged("boxCenter");
                            break;
                    }
                    break;
            }
            actualState = js;
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
