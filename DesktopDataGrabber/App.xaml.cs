using DesktopDataGrabber.View;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using DesktopDataGrabber.Service;
using DesktopDataGrabber.Model;

namespace DesktopDataGrabber
{
    /** 
     * @brief  Interaction logic for App.xaml
     */
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; private set; }
        public IConfiguration Configuration { get; private set; }
        protected override void OnStartup(StartupEventArgs e)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            Configuration = builder.Build();

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            ServiceProvider = serviceCollection.BuildServiceProvider();

            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IConfig, Config>();
            services.AddTransient<IPanelLED, PanelLED>();
            services.AddTransient(typeof(MainWindow));
        }
    }
}
