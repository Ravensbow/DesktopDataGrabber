using DesktopDataGrabber.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DesktopDataGrabber.ViewModel
{
    class LEDViewModel : INotifyPropertyChanged
    {
        public Grid PanelLed { get; set; }

        List<Button> chosenLeds = new List<Button>();
        Dictionary<Tuple<int, int>, Button> panel = new Dictionary<Tuple<int, int>, Button>();

        IConfig configService;
        IPanelLED panelLEDService;
        public LEDViewModel(IConfig c, IPanelLED pl)
        {
            configService = c;
            panelLEDService = pl;
            InitializePanelLed();
        }


        private void InitializePanelLed()
        {
            var bc = new BrushConverter();
            //int[] sensLEDs= {65535,65535,65535,65535,65535,65535,65535,65535,65535,65535,16744703,16744703,16744703,16744703,65535,65535,65535,16744703,16744703,16744703,16744703,16744703,16744703,65535,65535,16711935,16744703,0,16744703,0,16744703,65535,16711935,16744703,16744703,8388863,16744703,8388863,16744703,16711935,16711935,16711935,16711935,16744703,16744703,16744703,16711935,16711935,65535,12976326,16711935,16711935,16744703,16744703,12976326,65535,65280,12976326,12976326,12976326,65280,12976326,12976326,65280};
            int[] sensLEDs = panelLEDService.GetLEDsState();
            if (sensLEDs == null)
                return;
            Grid grid = new Grid() { VerticalAlignment=VerticalAlignment.Center,HorizontalAlignment=HorizontalAlignment.Center};
            for (int i =0;i<8; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition() { Height=GridLength.Auto});
                for(int j=0;j<8;j++)
                {
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto});
                    var but = new Button()
                    {
                        Margin = new Thickness(5, 5, 0, 0),
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Stretch,
                        Style = null,
                        MinHeight=30,
                        MinWidth=30,
                        Background = new SolidColorBrush(Color.FromRgb((byte)((sensLEDs[i * 8 + j] >> 16) & 0xFF), (byte)((sensLEDs[i * 8 + j] >> 8) & 0xFF), (byte)((sensLEDs[i * 8 + j] >> 0) & 0xFF)))
                    };
                    Grid.SetRow(but,i);
                    Grid.SetColumn(but,j);
                    grid.Children.Add(but);
                    panel.Add(Tuple.Create(i, j), but);
                }    
            }
            PanelLed = grid;
            int[] b = buttonToArray();
        }

        private int[] buttonToArray()
        {
            int[] temp = new int[64];
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Button b;
                    if (panel.TryGetValue(Tuple.Create(i, j), out b))
                    {
                        string s = ((SolidColorBrush)b.Background).Color.ToString().Replace("#","").Substring(2);
                        temp[i * 8 + j] = int.Parse(s, System.Globalization.NumberStyles.HexNumber);
                    }
                }
            }
            return temp;
        }

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
