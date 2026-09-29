using BusinessTier;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System;
using System.ComponentModel;
using System.Windows.Controls;
using Microsoft.Win32;
using System.IO;
using System.Drawing;
using System.Collections.Generic;

namespace Client
{
    public partial class LobbyChatWindow : Window, INotifyPropertyChanged
    {
        private ObservableCollection<string> messages;
        private ObservableCollection<string> privateMessages;
        private ObservableCollection<string> users;
        private string username;
        private string lobbyName;
        private BusinessServerInterface foob;
        private bool isLoading = true;
        private List<string> hardcodedFiles = new List<string> { "welcome_message.txt" };
        private string selectedUser;


        public event PropertyChangedEventHandler PropertyChanged;

        public string Username
        {
            get { return username; }
            set
            {
                username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        public string LobbyName
        {
            get { return lobbyName; }
            set
            {
                lobbyName = value;
                OnPropertyChanged(nameof(LobbyName));
            }
        }

        public string SelectedUser
        {
            get { return selectedUser; }
            set
            {
                selectedUser = value;
                OnPropertyChanged(nameof(SelectedUser));
                _ = LoadPrivateMessages();
            }
        }

        public LobbyChatWindow(BusinessServerInterface inFoob, string username, string lobbyName)
        {
            InitializeComponent();
            DataContext = this;
            foob = inFoob;
            Username = username;
            LobbyName = lobbyName;

            messages = new ObservableCollection<string>();
            privateMessages = new ObservableCollection<string>();
            users = new ObservableCollection<string>();

            FileList.ItemsSource = hardcodedFiles;

            Messages.ItemsSource = messages;
            DMChat.ItemsSource = privateMessages;
            UserList.ItemsSource = users;

            this.Title = $"Chat Room: {LobbyName}";

            UserList.SelectionChanged += UserList_SelectionChanged;
            SendDMBtn.Click += SendDMBtn_Click;

            StartLoadingMessages();
            StartLoadingUsers();
        }

        private async void StartLoadingMessages()
        {
            try
            {
                while (isLoading)
                {
                    await LoadMessages();
                    await Task.Delay(1000);
                }
            }
            catch (Exception ex)
            {
                await Dispatcher.InvokeAsync(() =>
                    MessageBox.Show($"Error loading messages: {ex.Message}")
                );
            }
        }

        private async Task LoadMessages()
        {
            try
            {
                var lobbyChat = await Task.Run(() => foob.GetLobbyChat(LobbyName));
                await Dispatcher.InvokeAsync(() =>
                {
                    messages.Clear();
                    foreach (var message in lobbyChat)
                    {
                        messages.Add(message);
                    }
                });
            }
            catch (Exception ex)
            {
                await Dispatcher.InvokeAsync(() =>
                    MessageBox.Show($"Error refreshing messages: {ex.Message}")
                );
            }
        }

        private async void StartLoadingUsers()
        {
            try
            {
                while (isLoading)
                {
                    await LoadUsers();
                    await Task.Delay(5000); // Refresh user list every 5 seconds
                }
            }
            catch (Exception ex)
            {
                await Dispatcher.InvokeAsync(() =>
                    MessageBox.Show($"Error loading users: {ex.Message}")
                );
            }
        }

        private async Task LoadUsers()
        {
            try
            {
                var lobbyUsers = await Task.Run(() => foob.GetLobbyUsers(LobbyName));
                await Dispatcher.InvokeAsync(() =>
                {
                    users.Clear();
                    foreach (var user in lobbyUsers)
                    {
                        if (user != Username) // Don't add the current user to the list
                        {
                            users.Add(user);
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                await Dispatcher.InvokeAsync(() =>
                    MessageBox.Show($"Error refreshing user list: {ex.Message}")
                );
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void SendBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(MesageInput.Text))
            {
                try
                {
                    string messageToSend = $"{Username}: {MesageInput.Text}";
                    await Task.Run(() => foob.SendMsg(LobbyName, messageToSend));
                    await Dispatcher.InvokeAsync(() => MesageInput.Clear());
                    await LoadMessages(); // Refresh messages immediately after sending
                }
                catch (Exception ex)
                {
                    await Dispatcher.InvokeAsync(() =>
                        MessageBox.Show($"Error sending message: {ex.Message}")
                    );
                }
            }
        }

        private void UserList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UserList.SelectedItem is string selected)
            {
                SelectedUser = selected;
            }
        }

        private async Task LoadPrivateMessages()
        {
            if (!string.IsNullOrWhiteSpace(SelectedUser))
            {
                int count = 0;
                try
                {
                    var dms = await Task.Run(() => foob.GetLobbyDm(LobbyName, Username, SelectedUser));
                    await Dispatcher.InvokeAsync(() =>
                    {
                        privateMessages.Clear();
                        foreach (var message in dms)
                        {
                            count++;
                            if (count % 2 != 0)
                            {
                                privateMessages.Add(message);
                            }

                        }
                    });
                }
                catch (Exception ex)
                {
                    await Dispatcher.InvokeAsync(() =>
                        MessageBox.Show($"Error loading private messages: {ex.Message}")
                    );
                }
            }
        }

        //file upload and download
        private async void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Image Files (*.png, *.jpg, *.jpeg)|*.png;*.jpg;*.jpeg|Text Files (*.txt)|*.txt";
                if (openFileDialog.ShowDialog() == true)
                {
                    string fileName = Path.GetFileName(openFileDialog.FileName);
                    Console.WriteLine($"Attempting to upload file: {fileName}");

                    try
                    {
                        if (fileName.EndsWith(".txt"))
                        {
                            string fileContent = File.ReadAllText(openFileDialog.FileName);
                            Console.WriteLine($"Read text file content. Length: {fileContent.Length}");
                            await Task.Run(() => foob.SendFile(LobbyName, fileName, fileContent));
                        }
                        else
                        {
                            Bitmap bitmap = new Bitmap(openFileDialog.FileName);

                            await Task.Run(() => foob.SendFile(LobbyName, fileName, bitmap));
                        }

                        Console.WriteLine("File upload completed successfully.");
                        await RefreshFileList();
                    }
                    catch (OutOfMemoryException oom)
                    {
                        Console.WriteLine($"Out of memory error: {oom.Message}");
                        MessageBox.Show("The file is too large to upload. Please try a smaller file.", "Upload Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    catch (IOException io)
                    {
                        Console.WriteLine($"IO error: {io.Message}");
                        MessageBox.Show($"Error reading the file: {io.Message}", "Upload Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error during file processing: {ex.Message}");
                        MessageBox.Show($"An error occurred while processing the file: {ex.Message}", "Upload Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unhandled error in UploadButton_Click: {ex.Message}");
                MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedFile = FileList.SelectedItem as string;
            if (!string.IsNullOrEmpty(selectedFile))
            {
                if (selectedFile.EndsWith(".txt"))
                {
                    DownloadTextFile(selectedFile);
                }
                else
                {
                    MessageBox.Show("Only text files are supported for now.", "Unsupported File Type", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Please select a file to download.", "No File Selected", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void DownloadTextFile(string selectedFile)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.FileName = selectedFile;
                saveFileDialog.Filter = "Text Files (*.txt)|*.txt";
                if (saveFileDialog.ShowDialog() == true)
                {
                    // For the hardcoded file, we'll create a dummy content
                    string content = "Welcome to the Mortal Kombat X Lobby!\n\nEnjoy your stay and may the best fighter win!";

                    // In a real scenario, you would get the content from the server like this:
                    // string content = foob.GetLobbyFile(LobbyName, selectedFile) as string;

                    File.WriteAllText(saveFileDialog.FileName, content);
                    MessageBox.Show("File downloaded successfully!", "Download Complete", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error downloading file: {ex.Message}", "Download Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private void DownloadImageFile(string selectedFile)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = selectedFile;
            saveFileDialog.Filter = "PNG Files (.png)|.png";
            Bitmap imageData = (Bitmap)foob.GetLobbyFile(LobbyName, selectedFile);
            if (saveFileDialog.ShowDialog() == true)
            {
                imageData.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        private async Task RefreshFileList()
        {
            try
            {
                Console.WriteLine("Refreshing file list...");
                var files = await Task.Run(() => foob.GetLobbyFileNames(LobbyName));
                Console.WriteLine($"Retrieved {files.Count} files from server.");

                await Dispatcher.InvokeAsync(() =>
                {
                    FileList.ItemsSource = files;
                    Console.WriteLine("File list updated in UI.");
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error refreshing file list: {ex.Message}");
                await Dispatcher.InvokeAsync(() =>
                    MessageBox.Show($"Error refreshing file list: {ex.Message}", "Refresh Error", MessageBoxButton.OK, MessageBoxImage.Error)
                );
            }
        }

        private async void SendDMBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(DMInput.Text) && !string.IsNullOrWhiteSpace(SelectedUser) && users.Contains(SelectedUser))
            {
                try
                {
                    string messageToSend = $"{Username}: {DMInput.Text}";
                    await Task.Run(() => foob.SendDm(LobbyName, Username, SelectedUser, messageToSend));
                    await Dispatcher.InvokeAsync(() => DMInput.Clear());
                    await LoadPrivateMessages();
                }
                catch (Exception ex)
                {
                    await Dispatcher.InvokeAsync(() =>
                        MessageBox.Show($"Error sending private message: {ex.Message}")
                    );
                }
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            isLoading = false;
            try
            {
                foob.RemoveUserFromLobby(Username, LobbyName);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error leaving lobby: {ex.Message}");
            }
            base.OnClosed(e);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            foob.RemoveUserFromLobby(Username, LobbyName);
            LobbiesWindow lobby = new LobbiesWindow(username, foob);
            this.Close();
            lobby.Show();
        }
    }
}