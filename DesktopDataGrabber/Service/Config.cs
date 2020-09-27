using DesktopDataGrabber.Model;
using DesktopDataGrabber.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DesktopDataGrabber.Service
{
    public class Config : IConfig
    {
        ConfigParams settings = new ConfigParams();

        public void ChangeSettings(ConfigParams value)
        {
            settings = value;
            ShowNotificationExecute("Pomyślnie zapisano ustawienia.");
        }

        public ConfigParams GetSettings()
        {
            return settings;
        }
        private void ShowNotificationExecute(string message)
        {
            App.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(
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
    }
}
