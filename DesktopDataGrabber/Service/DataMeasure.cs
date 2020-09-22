using DesktopDataGrabber.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

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
            client.BaseAddress = new Uri($"{"http://" + config.GetSettings().IpAddress}/");
        }

        bool IsConnected => Tools.InternetAvailability.IsInternetAvailable();

        public async Task<List<MeasureValues>> GetMeasureAsync()
        {
            if (IsConnected)
            {
                try
                {
                    var json = await client.GetByteArrayAsync($"api/measure.php");
                    string s = Encoding.UTF8.GetString(json);
                    var data = await Task.Run(() => JsonConvert.DeserializeObject<List<MeasureValues>>(s));
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

        public MeasureValues GetMeasureFake()
        {
            Random random = new Random();
            return new MeasureValues();
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
