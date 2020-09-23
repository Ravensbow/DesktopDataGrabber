using DesktopDataGrabber.Model;
using DesktopDataGrabber.View;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace DesktopDataGrabber.Service
{
    public class DataMeasure : IDataMeasure
    {
        HttpClient client;
        private IConfig config;

        public DataMeasure(IConfig c)
        {
            config = c;
            client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(5);
            client.BaseAddress = new Uri($"{"http://" + config.GetSettings().IpAddress}/");
        }

        bool IsConnected => Tools.InternetAvailability.IsInternetAvailable();

        public async Task<List<MeasureValues>> GetMeasureAsync()
        {
            client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(5);
            client.BaseAddress = new Uri($"{"http://" + config.GetSettings().IpAddress}/");
            if (IsConnected)
            {
                try
                {
                    var json = await client.GetByteArrayAsync($"api/measure.php");
                    string s = Encoding.UTF8.GetString(json);
                    if (s == null)
                        throw new WebException("Brak odpowiedi z serwera.");
                    else
                    {
                        var data = JsonConvert.DeserializeObject<List<MeasureValues>>(s);
                        return data;
                    }
                }
                catch (WebException e)
                {
                    ShowNotificationExecute(e.Message);
                }
                catch (HttpRequestException e)
                {
                    ShowNotificationExecute(e.Message);
                }
                catch (TaskCanceledException e)
                {
                    ShowNotificationExecute("Przekroczono limit czasu oczekiwania.");
                }
                catch (JsonException e)
                {
                    ShowNotificationExecute("Błędna odpowiedź serwera");
                }
                catch (Exception e)
                {
                    ShowNotificationExecute("Nieznany wyjątek. "+e.Message);
                    return null;
                }

            }

            return null;
        }

        public MeasureValues GetMeasureFake()
        {
            Random random = new Random();
            return new MeasureValues();
        }

        private void ShowNotificationExecute(string message)
        {
            App.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(
                () =>
                {
                    foreach (Window window in System.Windows.Application.Current.Windows)
                    {
                        string windowName = window.GetType().Name;

                        if (windowName.Equals("NotificationWindow"))
                        {
                            return;
                        }
                    }
                    var notify = new NotificationWindow();
                    notify.Show(message);
                }));
        }
        public async Task<Joystick> GetJoystickAsync()
        {
            if (IsConnected)
            {
                try
                {
                    var json = await client.GetByteArrayAsync($"api/joystick.php");
                    string s = Encoding.UTF8.GetString(json);
                    var data = await Task.Run(() => JsonConvert.DeserializeObject<Joystick>(s));
                    return data;
                }
                catch (WebException e)
                {
                    //Alert
                }
                catch (JsonException e)
                {
                   //Alert
                }
                catch (Exception e)
                {
                    //Alert
                }

            }
            return null;
        }
    }
}
