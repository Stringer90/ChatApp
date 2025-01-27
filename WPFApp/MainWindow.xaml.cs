using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ServiceModel;
using BusinessTier;
using DataTier;
using System.Runtime.CompilerServices;
using System.ServiceModel.Channels;

namespace WPFApp
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<string> Messages { get; set; }
        public ObservableCollection<string> Users { get; set; }
        public string Username;
        private CancellationTokenSource _messagesCancellationTokenSource;
        private CancellationTokenSource _usersCancellationTokenSource;

        private readonly BusinessServerInterface BusinessServerService;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            ChannelFactory<BusinessServerInterface> businessServerFactory;
            NetTcpBinding tcp = new NetTcpBinding();
            string URL = "net.tcp://0.0.0.0:8200/BusinessServerService";
            EndpointAddress endpointAddress = new EndpointAddress(URL);
            businessServerFactory = new ChannelFactory<BusinessServerInterface>(tcp, endpointAddress);
            BusinessServerService = businessServerFactory.CreateChannel();

            Messages = new ObservableCollection<string>();
            Users = new ObservableCollection<string>();
            _messagesCancellationTokenSource = new CancellationTokenSource();
            _usersCancellationTokenSource = new CancellationTokenSource();

            Username = "";
        }

        private async void Login_OnClick(object sender, RoutedEventArgs e)
        {
            string username = TextInput_Username.Text;

            await Task.Run(() => Login(username));
        }

        public async void Logout_OnClick(object sender, RoutedEventArgs e)
        {
            await Task.Run(() => Logout());
        }

        public async void Send_OnClick(object sender, RoutedEventArgs e)
        {
            string message = TextInput_Message.Text;

            await Task.Run(() => SendMessage(message));
        }

        public void Login(string username)
        {
            Text_LoginError.Dispatcher.Invoke(new Action(() => Text_LoginError.Text = ""));

            if (string.IsNullOrWhiteSpace(username))
            {
                Text_LoginError.Dispatcher.Invoke(new Action(() => Text_LoginError.Text = "Please enter a username before logging in."));
                return;
            }
            else if (username.Length > 10)
            {
                Text_LoginError.Dispatcher.Invoke(new Action(() => Text_LoginError.Text = "Please enter a shorter username."));
                return;
            }
            else if (!BusinessServerService.AddUser(username))
            {
                Text_LoginError.Dispatcher.Invoke(new Action(() => Text_LoginError.Text = "Username is already taken."));
                return;
            }
            else
            {
                EnableUI();
                Username = username;
                StartLoadingMessages();
                StartLoadingUsers();
            }
        }

        public void Logout()
        {
            BusinessServerService.RemoveUser(Username);
            Username = "";
            StopLoadingMessages();
            StopLoadingUsers();
            DisableUI();
        }

        public void SendMessage(string message)
        {
            BusinessServerService.SendMessage(message);
        }

        // For logging out
        public void DisableUI()
        {
            Messages.Clear();
            Users.Clear();
            TextInput_Message.Dispatcher.Invoke(new Action(() => TextInput_Message.Text = ""));
            TextInput_Username.Dispatcher.Invoke(new Action(() => TextInput_Username.IsEnabled = true));
            Button_Login.Dispatcher.Invoke(new Action(() => Button_Login.IsEnabled = true));
            Button_Logout.Dispatcher.Invoke(new Action(() => Button_Logout.IsEnabled = false));
            Button_Send.Dispatcher.Invoke(new Action(() => Button_Send.IsEnabled = false));
            List_Users.Dispatcher.Invoke(new Action(() => List_Users.IsEnabled = false));
            List_Messages.Dispatcher.Invoke(new Action(() => List_Messages.IsEnabled = false));
        }

        // For logging in
        public void EnableUI()
        {
            TextInput_Username.Dispatcher.Invoke(new Action(() => TextInput_Username.IsEnabled = false));
            Button_Login.Dispatcher.Invoke(new Action(() => Button_Login.IsEnabled = false));
            Button_Logout.Dispatcher.Invoke(new Action(() => Button_Logout.IsEnabled = true));
            Button_Send.Dispatcher.Invoke(new Action(() => Button_Send.IsEnabled = true));
            List_Users.Dispatcher.Invoke(new Action(() => List_Users.IsEnabled = true));
            List_Messages.Dispatcher.Invoke(new Action(() => List_Messages.IsEnabled = true));
        }

        public void StartLoadingMessages()
        {
            _messagesCancellationTokenSource = new CancellationTokenSource();

            Task.Run(async () =>
            {
                try
                {
                    while (!_messagesCancellationTokenSource.Token.IsCancellationRequested)
                    {
                        await LoadMessages();
                        await Task.Delay(1000, _messagesCancellationTokenSource.Token); // Supports cooperative cancellation
                    }
                }
                catch (TaskCanceledException) { }
                catch (Exception)
                {
                    await Dispatcher.InvokeAsync(() =>
                        Text_SendError.Text = "Error loading messages."
                    );
                }
            });
        }

        public void StartLoadingUsers()
        {
            _usersCancellationTokenSource = new CancellationTokenSource();

            Task.Run(async () =>
            {
                try
                {
                    while (!_usersCancellationTokenSource.Token.IsCancellationRequested)
                    {
                        await LoadUsers();
                        await Task.Delay(1000, _usersCancellationTokenSource.Token); // Supports cooperative cancellation
                    }
                }
                catch (TaskCanceledException) { }
                catch (Exception)
                {
                    await Dispatcher.InvokeAsync(() =>
                        Text_SendError.Text = "Error loading user list."
                    );
                }
            });
        }

        private async Task LoadMessages()
        {
            try
            {
                List<string> messages = await Task.Run(() => BusinessServerService.GetMessages());

                await Dispatcher.InvokeAsync(() =>
                {
                    Messages.Clear();
                    foreach (string message in messages)
                    {
                        Messages.Add(message);
                    }
                });
            }
            catch (Exception)
            {
                await Dispatcher.InvokeAsync(() =>
                    Text_SendError.Text = "Error loading messages."
                );
            }
        }

        private async Task LoadUsers()
        {
            try
            {
                List<string> users = await Task.Run(() => BusinessServerService.GetUsers());

                await Dispatcher.InvokeAsync(() =>
                {
                    Users.Clear();
                    foreach (string user in users)
                    {
                        Users.Add(user);
                    }
                });
            }
            catch (Exception)
            {
                await Dispatcher.InvokeAsync(() =>
                    Text_SendError.Text = "Error loading user list."
                );
            }
        }

        public void StopLoadingMessages()
        {
            _messagesCancellationTokenSource?.Cancel();
        }

        public void StopLoadingUsers()
        {
            _usersCancellationTokenSource?.Cancel();
        }
    }
}