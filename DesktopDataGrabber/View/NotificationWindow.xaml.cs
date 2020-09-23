using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace DesktopDataGrabber.View
{
    /// <summary>
    /// Logika interakcji dla klasy NotificationWindow.xaml
    /// </summary>
    public partial class NotificationWindow : Window
    {
        public NotificationWindow()
        {
            InitializeComponent();

            Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() =>
            {
                var workingArea = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea;
                var helper = new WindowInteropHelper(this);
                var hwndSource = HwndSource.FromHwnd(helper.EnsureHandle()).CompositionTarget.TransformFromDevice;
                var corner = hwndSource.Transform(new Point(workingArea.Right, workingArea.Bottom));
                this.ShowActivated=false;
            }));
        }
        public void Show(string message)
        {
            this.textMessage.Text = message;
            
            var mainWindow = System.Windows.Application.Current.MainWindow;
            this.Owner = mainWindow;
            var workingArea = Screen.PrimaryScreen.WorkingArea;

            foreach (Window window in System.Windows.Application.Current.Windows)
            {
                string windowName = window.GetType().Name;

                if (windowName.Equals("NotificationWindow") && window != this)
                {
                    //window.Close();
                    return;
                }
            }
            base.Show();
            this.Left = mainWindow.Left + mainWindow.ActualWidth - this.ActualWidth - 20;
            double top = mainWindow.Top + SystemParameters.WindowCaptionHeight + 20;
            this.Top = top;
            Task.Delay(1000);
        }
        private void Storyboard_Completed(object sender, EventArgs e) { this.Close(); }
    }
}
