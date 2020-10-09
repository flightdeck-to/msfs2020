using System.IO;
using System.Windows;
using FlightDeck.Core;
using FlightDeck.Logics;
using FlightDeck.Logics.Evaluators;
using FlightDeck.SimConnectFSX;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace FlightDeck.AddOn
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public ServiceProvider ServiceProvider { get; private set; }
        public IConfigurationRoot Configuration { get; private set; }

        private MainWindow mainWindow = null;

        protected override void OnStartup(StartupEventArgs e)
        {

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            Configuration = builder.Build();

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            ServiceProvider = serviceCollection.BuildServiceProvider();

            mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private void ConfigureServices(ServiceCollection services)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Debug()
                .MinimumLevel.Information()
                .WriteTo.File("flightdeck.log", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 3)
                .CreateLogger();

            //services.AddOptions<AppSettings>().Bind(Configuration).ValidateDataAnnotations();

            services.AddLogging(configure =>
            {
                configure.AddSerilog();
            });

            services.AddSingleton<DeckLogic>();
            services.AddSingleton<IFlightConnector, SimConnectFlightConnector>();
            //services.AddSingleton(new UserPreferencesLoader("preferences.json"));

            services.AddTransient(typeof(MainWindow));
            services.AddSingleton<IImageLogic, ImageLogic>();
            services.AddTransient<IEvaluator, ComparisonEvaluator>();
            services.AddTransient<EnumConverter>();
        }
    }
}
