using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using FlightDeck.Logics;
using FlightDeck.SimConnectFSX;
using Microsoft.Extensions.Logging;

namespace FlightDeck.AddOn
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly DeckLogic deckLogic;
        private readonly IFlightConnector flightConnector;
        private readonly ILogger<MainWindow> logger;
        private IntPtr _handle;

        public MainWindow(DeckLogic deckLogic, IFlightConnector flightConnector, ILogger<MainWindow> logger)
        {
            InitializeComponent();
            this.deckLogic = deckLogic;
            this.flightConnector = flightConnector;
            this.logger = logger;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Hide();
            deckLogic.Initialize();

            // Initialize SimConnect
            if (flightConnector is SimConnectFlightConnector simConnect)
            {
                simConnect.Closed += SimConnect_Closed;

                // Create an event _handle for the WPF window to listen for SimConnect events
                _handle = new WindowInteropHelper((sender as Window)!).Handle; // Get _handle of main WPF Window
                var handleSource = HwndSource.FromHwnd(_handle); // Get source of _handle in order to add event handlers to it
                handleSource?.AddHook(simConnect.HandleSimConnectEvents);

                //var viewModel = ServiceProvider.GetService<MainViewModel>();

                try
                {
                    await InitializeSimConnectAsync(simConnect);
                }
                catch (BadImageFormatException ex)
                {
                    logger.LogError(ex, "Cannot find SimConnect!");

                    var result = MessageBox.Show(this, "SimConnect not found. Please contact support",
                        "Needed component is missing",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    Application.Current.Shutdown(-1);
                }
            }
        }

        private async Task InitializeSimConnectAsync(SimConnectFlightConnector simConnect)
        {
            myNotifyIcon.Icon = new Icon("Images/button@2x.ico");
            while (true)
            {
                try
                {
                    simConnect.Initialize(_handle);
                    myNotifyIcon.Icon = new Icon("Images/button_active@2x.ico");
                    //simConnect.Send("Connected to FlightDeck");
                    break;
                }
                catch (COMException)
                {
                    await Task.Delay(5000).ConfigureAwait(true);
                }
            }
        }

        private async void SimConnect_Closed(object sender, EventArgs e)
        {
            var simConnect = sender as SimConnectFlightConnector;
            await InitializeSimConnectAsync(simConnect);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            myNotifyIcon.Dispose();
        }
    }
}
