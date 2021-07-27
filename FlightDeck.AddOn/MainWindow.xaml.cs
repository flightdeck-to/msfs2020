using FlightDeck.Logics;
using FlightDeck.SimConnectFSX;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

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
        private readonly ThrottlingLogic throttlingLogic;
        private IntPtr Handle;

        public MainWindow(DeckLogic deckLogic, IFlightConnector flightConnector, ILogger<MainWindow> logger, ThrottlingLogic throttlingLogic)
        {
            InitializeComponent();
            this.deckLogic = deckLogic;
            this.flightConnector = flightConnector;
            this.logger = logger;
            this.throttlingLogic = throttlingLogic;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Hide();

            if (Environment.GetCommandLineArgs().Length <= 1)
            {
                Application.Current.Shutdown();
                return;
            }

            deckLogic.Initialize();

            // Initialize SimConnect
            if (flightConnector is SimConnectFlightConnector simConnect)
            {
                simConnect.Closed += SimConnect_Closed;

                // Create an event handle for the WPF window to listen for SimConnect events
                Handle = new WindowInteropHelper(sender as Window).Handle; // Get handle of main WPF Window
                var HandleSource = HwndSource.FromHwnd(Handle); // Get source of handle in order to add event handlers to it
                HandleSource.AddHook(HandleSimConnectHook);

                //var viewModel = ServiceProvider.GetService<MainViewModel>();

                try
                {
                    logger.LogDebug("Start connecting...");
                    await InitializeSimConnectAsync(simConnect);
                }
                catch (BadImageFormatException ex)
                {
                    logger.LogError(ex, "Cannot find SimConnect!");

                    var result = MessageBox.Show(this, "Please contact support",
                        "SimConnect component is missing",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    App.Current.Shutdown(-1);
                }
            }
        }

        private IntPtr HandleSimConnectHook(IntPtr hWnd, int message, IntPtr wParam, IntPtr lParam, ref bool isHandled)
        {
            try
            {
                (flightConnector as SimConnectFlightConnector).HandleSimConnectEvents(message, ref isHandled);
                return IntPtr.Zero;
            }
            catch (BadImageFormatException)
            {
                return IntPtr.Zero;
            }
        }

        private async Task InitializeSimConnectAsync(SimConnectFlightConnector simConnect)
        {
            myNotifyIcon.Icon = new Icon("Images/button@2x.ico");
            while (true)
            {
                try
                {
                    simConnect.Initialize(Handle);
                    myNotifyIcon.Icon = new Icon("Images/button_active@2x.ico");
                    //simConnect.Send("Connected to Stream Deck plugin");
                    break;
                }
                catch (COMException ex)
                {
                    logger.LogDebug(ex, "SimConnect error.");
                    await Task.Delay(5000).ConfigureAwait(true);
                }
            }
        }

        private void SimConnect_Closed(object sender, EventArgs e)
        {
            logger.LogDebug("SimConnect is closed.");
            throttlingLogic.RunAsync(async () =>
            {
                logger.LogDebug("Start reconnecting...");
                var simConnect = sender as SimConnectFlightConnector;
                await InitializeSimConnectAsync(simConnect);
            }).Forget();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            myNotifyIcon.Dispose();
        }
    }
}
