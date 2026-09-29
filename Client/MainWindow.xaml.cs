using System;
using System.Windows;
using System.ServiceModel;
using BusinessTier;

namespace Client
{
    public partial class MainWindow : Window
    {
        private BusinessServerInterface foob;

        public MainWindow()
        {
            InitializeComponent();
            var tcp = new NetTcpBinding();
            var URL = "net.tcp://localhost:8200/BusinessService";
            var chanFactory = new ChannelFactory<BusinessServerInterface>(tcp, URL);
            foob = chanFactory.CreateChannel();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameInput.Text.Trim();
            if (IsValidInput(username))
            {
                try
                {
                    bool loginSuccessful = foob.CreateUser(username);
                    if (loginSuccessful)
                    {
                        StatusText.Text = "Successfully logged in";
                        OpenLobbiesWindow(username);
                    }
                    else
                    {
                        StatusText.Text = "Username is already in use. Please choose a different username.";
                    }
                }
                catch (Exception ex)
                {
                    StatusText.Text = $"Error: {ex.Message}";
                }
            }
            else
            {
                StatusText.Text = "Please enter a valid username.";
            }
        }

        private bool IsValidInput(string input)
        {
            return !string.IsNullOrWhiteSpace(input);
        }

        private void OpenLobbiesWindow(string username)
        {
            LobbiesWindow lobbiesWindow = new LobbiesWindow(username, foob);
            lobbiesWindow.Show();
            this.Close();
        }
    }
}