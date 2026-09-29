using BusinessTier;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Linq;
using System.Collections.Generic;

namespace Client
{
    public partial class LobbiesWindow : Window
    {
        private string Username;
        private ObservableCollection<string> LobbiesList;
        private BusinessServerInterface foob;
        private CancellationTokenSource cts;

        public LobbiesWindow(string InUsername, BusinessServerInterface InFoob)
        {
            InitializeComponent();
            Username = InUsername;
            foob = InFoob;
            LobbiesList = new ObservableCollection<string>();
            LobbyList.ItemsSource = LobbiesList;
            StartLoadingLobbies();
        }

        private async void StartLoadingLobbies()
        {
            cts = new CancellationTokenSource();

            try
            {
                while (!cts.Token.IsCancellationRequested)
                {
                    await LoadLobbies();
                    await Task.Delay(1000, cts.Token);
                }
            }
            catch (OperationCanceledException)
            {
                // This is expected when cancelling, no need to handle
            }
            catch (Exception ex)
            {
                await Dispatcher.InvokeAsync(() =>
                    MessageBox.Show($"Error loading lobbies: {ex.Message}")
                );
            }
        }

        private async Task LoadLobbies()
        {
            try
            {
                Console.WriteLine($"LoadLobbies called on thread {Thread.CurrentThread.ManagedThreadId}.");
                var lobbyNames = await Task.Run(() => foob.GetLobbyNames());
                Console.WriteLine($"Received {lobbyNames.Count} lobbies from server: {string.Join(", ", lobbyNames)}");

                await Dispatcher.InvokeAsync(() =>
                {
                    Console.WriteLine($"Updating UI on thread {Thread.CurrentThread.ManagedThreadId}.");
                    var currentLobbies = new HashSet<string>(LobbiesList);
                    var serverLobbies = new HashSet<string>(lobbyNames);

                    var newLobbies = serverLobbies.Except(currentLobbies).ToList();
                    var removedLobbies = currentLobbies.Except(serverLobbies).ToList();

                    foreach (var lobby in newLobbies)
                    {
                        LobbiesList.Add(lobby);
                        Console.WriteLine($"Added new lobby: {lobby}");
                    }

                    foreach (var lobby in removedLobbies)
                    {
                        LobbiesList.Remove(lobby);
                        Console.WriteLine($"Removed lobby: {lobby}");
                    }

                    Console.WriteLine($"Updated LobbiesList now contains {LobbiesList.Count} lobbies: {string.Join(", ", LobbiesList)}");
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in LoadLobbies: {ex.Message}");
                await Dispatcher.InvokeAsync(() =>
                    MessageBox.Show($"Error refreshing lobby list: {ex.Message}")
                        );
            }
        }

        private async void CreateLobbyBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(LobbyName.Text))
            {
                try
                {
                    string lobbyNameToCreate = LobbyName.Text;
                    Console.WriteLine($"Attempting to create lobby: {lobbyNameToCreate}");
                    bool created = await Task.Run(() => foob.CreateLobby(lobbyNameToCreate));
                    await Dispatcher.InvokeAsync(() =>
                    {
                        if (created)
                        {
                            LobbyStatusText.Text = "Lobby Created!";
                            LobbyName.Clear();
                            Console.WriteLine($"Lobby {lobbyNameToCreate} created successfully.");
                        }
                        else
                        {
                            LobbyStatusText.Text = "Lobby already exists";
                            Console.WriteLine($"Failed to create lobby {lobbyNameToCreate}. It may already exist.");
                        }
                    });
                    await LoadLobbies(); // Refresh the lobby list immediately
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in CreateLobbyBtn_Click: {ex.Message}");
                    await Dispatcher.InvokeAsync(() =>
                        MessageBox.Show($"Error creating lobby: {ex.Message}")
                    );
                }
            }
        }

        private async void JoinLobbyBtn_Click(object sender, RoutedEventArgs e)
        {
            if (LobbyList.SelectedItem != null)
            {
                string selectedLobby = LobbyList.SelectedItem.ToString();
                try
                {
                    await Task.Run(() => foob.AddUserToLobby(Username, selectedLobby));
                    cts.Cancel(); // Stop loading lobbies
                    await Dispatcher.InvokeAsync(() =>
                    {
                        LobbyChatWindow lobbyChatWindow = new LobbyChatWindow(foob, Username, selectedLobby);
                        this.Close();
                        lobbyChatWindow.Show();
                    });
                }
                catch (Exception ex)
                {
                    await Dispatcher.InvokeAsync(() =>
                        MessageBox.Show($"Error joining lobby: {ex.Message}")
                    );
                }
            }
        }

        private void LogOutBtn_Click(object sender, RoutedEventArgs e)
        {
            foob.RemoveUser(Username);
            MainWindow mainWindow = new MainWindow();
            this.Close();
            mainWindow.Show();
        }

        protected override void OnClosed(EventArgs e)
        {
            cts?.Cancel();
            base.OnClosed(e);
        }
    }
}