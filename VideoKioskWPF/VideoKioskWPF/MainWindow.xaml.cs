using AxTrueConf_CallXLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace VideoKioskWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SettingsWindow settingsWindow;
        MainDataContext dataContext;
        Settings settings;
        XmlSerializer formatter = new XmlSerializer(typeof(Settings));
        AxTrueConfCallX tc;
        int appState = 0;
        Abook abook = new Abook();
        Timer timer;
        string logName = "log-" + DateTime.Now.ToString("dd-MM-yy") + ".txt";

        public MainWindow()
        {
            InitializeComponent();

            settingsWindow = new SettingsWindow(SettingsWindow_Closed);

            CheckCmdLineArgs();

            dataContext = new MainDataContext();
            DataContext = dataContext;
            InputBinding hotkeySettings = new InputBinding(new RelayCommand(c=>OpenSettings(),c=>true),
                new KeyGesture(Key.F12, ModifierKeys.Shift | ModifierKeys.Control));
            InputBindings.Add(hotkeySettings);

            LoadSettings();

            //set sizes
            mainWindow.Height = SystemParameters.VirtualScreenHeight;
            mainWindow.Width = SystemParameters.VirtualScreenWidth;
            mainGrid.RowDefinitions[0].Height = new GridLength(0.03 * mainWindow.Height);
            mainGrid.RowDefinitions[2].Height = new GridLength(0.15 * mainWindow.Height);
            mainGrid.RowDefinitions[1].Height = new GridLength(mainWindow.Height - mainGrid.RowDefinitions[0].Height.Value - mainGrid.RowDefinitions[2].Height.Value);
            btnCall.Width = mainWindow.Width / 3;
            chatPopup.MaxWidth = 400;
            chatPopup.MaxHeight = mainWindow.Height - 100;

            timer = new Timer(settings.chat * 1000);
            timer.Elapsed += Timer_Elapsed;
            timer.AutoReset = false;

            //TrueConf sdk init
            tc = new AxTrueConfCallX();

            tcHost.Focusable = false;
            tcHost.Child = tc;
            tc.GotFocus += Tc_GotFocus;

            //TrueCon sdk events
            tc.OnXAfterStart += Tc_OnXAfterStart;
            tc.OnXLogin += Tc_OnXLogin;
            tc.OnXLoginError += Tc_OnXLoginError;
            tc.OnServerConnected += Tc_OnServerConnected;
            tc.OnServerDisconnected += Tc_OnServerDisconnected;
            tc.OnIncomingChatMessage += Tc_OnIncomingChatMessage;
            tc.OnIncomingGroupChatMessage += Tc_OnIncomingGroupChatMessage;
            tc.OnInviteReceived += Tc_OnInviteReceived;
            tc.OnXNotify += Tc_OnXNotify;
            tc.OnAbookUpdate += Tc_OnAbookUpdate;
            tc.OnConferenceCreated += Tc_OnConferenceCreated;
            tc.OnConferenceDeleted += Tc_OnConferenceDeleted;
            tc.OnXChangeState += Tc_OnXChangeState;
            tc.OnLogout += Tc_OnLogout;

            Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            tc.shutdown2(true);
            Application.Current.Shutdown();
        }

        private void OpenSettings()
        {
            settingsWindow = new SettingsWindow(SettingsWindow_Closed);
            settingsWindow.LoadLastSettings();
            settingsWindow.ShowDialog();
        }

        private void Tc_GotFocus(object sender, EventArgs e)
        {
            mainWindow.Focus();
        }

        private void Tc_OnLogout(object sender, _ITrueConfCallXEvents_OnLogoutEvent e)
        {
            WriteLog(String.Format("Warning : {0} : The client logged out", DateTime.Now.ToString("g")));
            if (appState == 2)
            {
                tc.login(settings.login, settings.password);
                WriteLog(String.Format("Info : {0} : Try login as {1}", DateTime.Now.ToString("g"), settings.login));
            }
        }

        private void SettingsWindow_Closed(object sender, EventArgs e)
        {
            LoadSettings();
        }

        private void CheckCmdLineArgs()
        {
            string[] args = Environment.GetCommandLineArgs();
            if (args.Contains("-config") || args.Contains("/config"))
            {
                OpenSettings();
            }
        }

        private void LoadSettings()
        {
            if (settings == null)
                settings = new Settings();
            string oldServer = settings.server;
            if (File.Exists("settings.xml"))
                using (FileStream fs = new FileStream("settings.xml", FileMode.OpenOrCreate))
                {
                    settings = (Settings)formatter.Deserialize(fs);
                }

            logName = System.IO.Path.Combine(settings.logPath, logName);
            if (tc != null)
            {
                SetHardware();
                if (oldServer != settings.server)
                    tc.connectToServer(settings.server);
            }
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            timer.Stop();
            Dispatcher.Invoke(() =>
            {
                chatPopup.IsOpen = false;
            });
        }

        private DateTime GetDateTimeFromUlong(ulong time)
        {
            DateTime pointOfReference = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            long ticks = (long)(time / 100);
            return pointOfReference.AddTicks(ticks);
        }

        private void SetHardware()
        {
            tc.XSetCameraByIndex(settings.camera);
            tc.XSelectMicByIndex(settings.microphone);
            tc.XSelectSpeakerByIndex(settings.speaker);
            WriteLog(String.Format("Info : {0} : Setting hardware to: camera index - {1}, microphone index - {2}, speaker index - {3}", DateTime.Now.ToString("g"), settings.camera, settings.microphone, settings.speaker));
        }

        private void Tc_OnXChangeState(object sender, _ITrueConfCallXEvents_OnXChangeStateEvent e)
        {
            appState = e.newState;
            switch (e.newState)
            {
                case 0:
                case 1:
                case 2:
                    dataContext.ImageBG = new BitmapImage(new Uri("pack://application:,,,/Resources/Connecting.png"));
                    dataContext.ImageClickBG = new BitmapImage(new Uri("pack://application:,,,/Resources/Connecting-click.png"));
                    btnCall.IsEnabled = false;
                    break;
                case 3:
                    dataContext.ImageBG = new BitmapImage(new Uri("pack://application:,,,/Resources/Call.png"));
                    dataContext.ImageClickBG = new BitmapImage(new Uri("pack://application:,,,/Resources/Call-click.png"));
                    btnCall.IsEnabled = true;
                    break;
                case 4:
                    dataContext.ImageBG = new BitmapImage(new Uri("pack://application:,,,/Resources/Calling.png"));
                    dataContext.ImageClickBG = new BitmapImage(new Uri("pack://application:,,,/Resources/Calling-click.png"));
                    btnCall.IsEnabled = true;
                    break;
                case 5:
                    dataContext.ImageBG = new BitmapImage(new Uri("pack://application:,,,/Resources/Reject.png"));
                    dataContext.ImageClickBG = new BitmapImage(new Uri("pack://application:,,,/Resources/Reject-click.png"));
                    btnCall.IsEnabled = true;
                    break;
                case 6:
                    dataContext.ImageBG = new BitmapImage(new Uri("pack://application:,,,/Resources/Reject.png"));
                    dataContext.ImageClickBG = new BitmapImage(new Uri("pack://application:,,,/Resources/Reject-click.png"));
                    btnCall.IsEnabled = true;
                    break;
                default:
                    dataContext.ImageBG = new BitmapImage(new Uri("pack://application:,,,/Resources/Connecting.png"));
                    dataContext.ImageClickBG = new BitmapImage(new Uri("pack://application:,,,/Resources/Connecting-click.png"));
                    btnCall.IsEnabled = false;
                    break;

            }
            WriteLog(String.Format("Info : {0} : App state is changed. Previous state - {1}, new state - {2}", DateTime.Now.ToString("g"), e.prevState, e.newState));
        }

        public void WriteLog(string msg)
        {
            if (settings.isLogged)
            {
                using (StreamWriter sw = new StreamWriter(logName, true))
                {
                    sw.WriteLine(msg);
                    sw.WriteLine();
                }
            }
        }

        private void Tc_OnConferenceDeleted(object sender, _ITrueConfCallXEvents_OnConferenceDeletedEvent e)
        {
            WriteLog(String.Format("Info : {0} : Conference deleted. Event details - {1}", DateTime.Now.ToString("g"), e.eventDetails));
        }

        private void Tc_OnConferenceCreated(object sender, _ITrueConfCallXEvents_OnConferenceCreatedEvent e)
        {
            WriteLog(String.Format("Info : {0} : Conference created. Event details - {1}", DateTime.Now.ToString("g"), e.eventDetails));
        }

        private void Tc_OnAbookUpdate(object sender, _ITrueConfCallXEvents_OnAbookUpdateEvent e)
        {
            abook.UpdateAbook(e.eventDetails);
            WriteLog(String.Format("Info : {0} : Abook updated", DateTime.Now.ToString("g")));
        }

        private void Tc_OnXNotify(object sender, _ITrueConfCallXEvents_OnXNotifyEvent e)
        {
            if (e.data.Contains("getAbook"))
            {
                abook.UpdateAbook(e.data);
                WriteLog(String.Format("Info : {0} : Get abook", DateTime.Now.ToString("g")));
            }
        }

        private void Tc_OnInviteReceived(object sender, _ITrueConfCallXEvents_OnInviteReceivedEvent e)
        {
            if (appState != 5)
                tc.accept();
            WriteLog(String.Format("Info : {0} : Incoming call or invitation. Event details - {1}", DateTime.Now.ToString("g"), e.eventDetails));
        }

        private void Tc_OnIncomingGroupChatMessage(object sender, _ITrueConfCallXEvents_OnIncomingGroupChatMessageEvent e)
        {
            dataContext.MessageHeader = String.Format("New group message from {0}: \r\n", e.peerDn);
            dataContext.Message = e.message;
            timer.Stop();
            timer.Start();
            WriteLog(String.Format("Info : {0} : Incoming group message from {1} - [{2}]", DateTime.Now.ToString("g"), e.peerId, e.message));

            chatPopup.IsOpen = true;
        }

        private void Tc_OnIncomingChatMessage(object sender, _ITrueConfCallXEvents_OnIncomingChatMessageEvent e)
        {
            dataContext.MessageHeader = String.Format("New message from {0}: \r\n", e.peerDn);
            dataContext.Message = e.message;
            timer.Stop();
            timer.Start();
            WriteLog(String.Format("Info : {0} : Incoming message from {1} - [{2}]", DateTime.Now.ToString("g"), e.peerId, e.message));

            chatPopup.IsOpen = true;
        }

        private void Tc_OnServerDisconnected(object sender, _ITrueConfCallXEvents_OnServerDisconnectedEvent e)
        {
            WriteLog(String.Format("Warning : {0} : Server disconnected. Lost connection to - {1}", DateTime.Now.ToString("g"), settings.server));
        }

        private void Tc_OnServerConnected(object sender, _ITrueConfCallXEvents_OnServerConnectedEvent e)
        {
            tc.login(settings.login, settings.password);
            WriteLog(String.Format("Info : {0} : Connect to server success. Connect to - {1}", DateTime.Now.ToString("g"), settings.server));
            WriteLog(String.Format("Info : {0} : Try login as {1}", DateTime.Now.ToString("g"), settings.login));

        }

        private void Tc_OnXLoginError(object sender, _ITrueConfCallXEvents_OnXLoginErrorEvent e)
        {
            WriteLog(String.Format("Error : {0} : Login error. Error code - {1}", DateTime.Now.ToString("g"), e.errorCode));
        }

        private void Tc_OnXLogin(object sender, EventArgs e)
        {
            tc.getAbook();
            WriteLog(String.Format("Info : {0} : Login successed. Logged in as - {1}", DateTime.Now.ToString("g"), settings.login));
        }

        private void Tc_OnXAfterStart(object sender, EventArgs e)
        {
            SetHardware();
            tc.connectToServer(settings.server);
            mainWindow.Focus();
            WriteLog(String.Format("Info : {0} : Try connect to server {1}", DateTime.Now.ToString("g"), settings.server));
        }

        private void BtnCall_Click(object sender, RoutedEventArgs e)
        {
            switch (appState)
            {
                case 0:
                case 1:
                case 2:
                    break;
                case 3:
                    string callId = abook.GetRandomOnlineUser();
                    if (callId == null)
                    {
                        WriteLog(String.Format("Error : {0} : No available user in Abook. Client cannot make call.", DateTime.Now.ToString("g")));
                    }
                    else
                    {
                        tc.call(callId);
                        WriteLog(String.Format("Info : {0} : Calling user {1}", DateTime.Now.ToString("g"), callId));
                    }
                    break;
                case 4:
                case 5:
                    tc.hangUp();
                    break;
                case 6:
                    break;
                default:
                    break;
            }
            mainWindow.Focus();
        }
    }
}
