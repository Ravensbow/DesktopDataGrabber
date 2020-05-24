using DesktopDataGrabber.Service;
using DesktopDataGrabber.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DesktopDataGrabber.View.Partial
{
    /// <summary>
    /// Logika interakcji dla klasy PrzebiegiPage.xaml
    /// </summary>
    public partial class PrzebiegiPage : Page
    {
        public PrzebiegiPage(IConfig c)
        {
            DataContext = new PrzebiegiViewModel(c);
            InitializeComponent();
        }
    }
}
