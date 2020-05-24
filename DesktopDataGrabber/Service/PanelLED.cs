﻿
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DesktopDataGrabber.Service
{
    class PanelLED : IPanelLED
    {
        public int[] LEDs { get; set; } = new int[64];

        HttpClient client;
        private IConfig config;

        private bool IsConnected() 
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("http://google.com/generate_204"))
                    return true;
            }
            catch
            {
                return false;
            }
        }

        public PanelLED(IConfig c)
        {
            config = c;
            client = new HttpClient();
            client.BaseAddress = new Uri($"{"http://" + config.GetSettings().IpAddress}/");
        }

        public bool DrawSymbol()
        {
            throw new NotImplementedException();
        }

        public async Task<int[]> GetLEDsState()
        {
            if (IsConnected())
            {
                try
                {
                    var json = await client.GetStringAsync($"api/LED/GetLEDs");
                    return await Task.Run(() => JsonConvert.DeserializeObject<int[]>(json));
                }
                catch (WebException e)
                {
                    Console.WriteLine(e.Message);
                }
                catch (JsonException e)
                {
                    Console.WriteLine(e.Message);
                }
                catch
                {
                    Console.WriteLine("Unknown exeption");
                }

            }

            return null;
        }

        public async Task<bool> SetLEDs(int[] leds)
        {
            if (IsConnected())
            {
                try
                {
                    var json = await client.GetStringAsync($"api/LED/SetLEDs?data={Newtonsoft.Json.JsonConvert.SerializeObject(leds)}");
                    return await Task.Run(() => JsonConvert.DeserializeObject<bool>(json));
                }
                catch (WebException e)
                {
                    Console.WriteLine(e.Message);
                }
                catch (JsonException e)
                {
                    Console.WriteLine(e.Message);
                }
                catch
                {
                    Console.WriteLine("Unknown exeption");
                }

            }

            return false;
        }
    }
}
