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
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;
using DesktopDataGrabber.Model;
using System.Windows.Media.Imaging;
using DesktopDataGrabber.Tools;

namespace DesktopDataGrabber.ViewModel
{
    class LEDViewModel : INotifyPropertyChanged
    {
        public Grid PanelLed { get; set; }
        public Grid ButtonPanel{ get; set; }
        public Color ColorSelected { get; set; }

        public ButtonCommand Set{ get; set; }

        List<Button> chosenLeds = new List<Button>();
        Dictionary<Tuple<int, int>, Button> panel = new Dictionary<Tuple<int, int>, Button>();
        List<LedMap> LedMaps = new List<LedMap>();

        IConfig configService;
        IPanelLED panelLEDService;
        public LEDViewModel(IConfig c, IPanelLED pl)
        {
            configService = c;
            panelLEDService = pl;
            InitializePanelLed(panelLEDService.GetLEDsState());
            SetButtonPanel();
            Set = new ButtonCommand(SetColor);
            ColorSelected = Colors.Blue;
        }

        private void InitializePanelLed(int[] sensLEDs)
        {
            var bc = new BrushConverter();
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
                    but.Click += Button_Clicked;
                    Grid.SetRow(but,i);
                    Grid.SetColumn(but,j);
                    grid.Children.Add(but);
                    panel.Add(Tuple.Create(i, j), but);
                }    
            }
            PanelLed = grid;
        }
        private void SetPanelLed(int[] sensLEDs)
        {
            for(int i =0;i<8; i++)
            {
                for(int j =0;j<8; j++)
                {
                    if (panel.TryGetValue(Tuple.Create(i, j), out Button b))
                    {
                        b.Background = new SolidColorBrush(Color.FromRgb((byte)((sensLEDs[i * 8 + j] >> 16) & 0xFF), (byte)((sensLEDs[i * 8 + j] >> 8) & 0xFF), (byte)((sensLEDs[i * 8 + j] >> 0) & 0xFF)));
                    }
                }
            }
        }

        private void SetButtonPanel()
        {
            string json = File.ReadAllText("Resource/ledmaps.json");
            var ledmaps = JsonSerializer.Deserialize<List<LedMap>>(json);
            LedMaps = ledmaps;
            Grid grid = new Grid() { Margin = new Thickness(90,0,0,0), VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center };
            for (int i = 0; i < ledmaps.Count; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                if (i%2==0)
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                var butt = new Button
                {
                    Margin = new Thickness(5, 5, 0, 0),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Style = null,
                    MaxHeight = 32,
                    MaxWidth = 32,
                    Name = "a"+ i.ToString(),
                    

                };
                BitmapSource bitmapSource = BitmapSource.
                    Create(8, 8, 300, 300, PixelFormats.Bgr32, BitmapPalettes.WebPalette, ledmaps[i].Map.ToArray(), 32);
                butt.Content = new Image() { Source = bitmapSource };
                butt.ToolTip = new ToolTip { Content = ledmaps[i].Name};
                butt.Click += (o, e) =>
                {
                    var But = o as Button;
                    SetPanelLed(ledmaps[int.Parse(But.Name.Remove(0,1))].Map);
                };
                Grid.SetRow(butt, (int)(i/2));
                Grid.SetColumn(butt, i%2);
                grid.Children.Add(butt);
            }
            ButtonPanel = grid;
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

        private void clearChosen()
        {
            chosenLeds.ForEach(b => b.BorderThickness = new Thickness(0,0,0,0));
            chosenLeds.Clear();
        }

        private void SetColor()
        {
            chosenLeds.ForEach(b => b.Background = new SolidColorBrush(ColorSelected));
            var a = buttonToArray();
            panelLEDService.SetLEDs(buttonToArray());
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            var but = sender as Button;
            var ee = e as System.Windows.Input.KeyEventArgs;
            if ( ee!= null&&ee.Key ==System.Windows.Input.Key.LeftShift)
            {
                but.BorderThickness = new Thickness(5, 5, 5, 5);
                but.BorderBrush = new SolidColorBrush(Colors.LightSkyBlue);
                chosenLeds.Add(but);
            }
            else
            {
                if (chosenLeds.Count == 0 || chosenLeds.Last() != but)
                {
                    clearChosen();
                    but.BorderThickness = new Thickness(5, 5, 5, 5);
                    but.BorderBrush = new SolidColorBrush(Colors.LightSkyBlue);
                    chosenLeds.Add(but);
                }
            }
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
