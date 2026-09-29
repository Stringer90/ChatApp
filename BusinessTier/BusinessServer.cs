using System;
using System.Collections.Generic;
using System.ServiceModel;
using DataTier;

namespace BusinessTier
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class BusinessServer : BusinessServerInterface
    {
        private static readonly DataServerInterface DataServer;
        private HashSet<string> loggedInUsers = new HashSet<string>();

        static BusinessServer()
        {
            ChannelFactory<DataServerInterface> dataFactory;
            NetTcpBinding tcp = new NetTcpBinding();
            string URL = "net.tcp://localhost:8100/DataService";
            dataFactory = new ChannelFactory<DataServerInterface>(tcp, URL);
            DataServer = dataFactory.CreateChannel();
        }

        public bool DoesUserExist(string pUsername)
        {
            List<string> users = DataServer.GetUserNames();
            return users.Contains(pUsername);
        }

        public bool DoesLobbyExist(string pLobbyName)
        {
            List<string> lobbies = DataServer.GetLobbyNames();
            return lobbies.Contains(pLobbyName);
        }

        public bool CreateUser(string pUsername)
        {
            if (!DoesUserExist(pUsername))
            {
                DataServer.AddUser(pUsername);
                return true;
            }
            return false;
        }

        public void RemoveUser(string pUsername)
        {
            DataServer.RemoveUser(pUsername);
        }

        public bool CreateLobby(string pLobbyName)
        {
            Console.WriteLine($"CreateLobby called with name: {pLobbyName}");
            if (!DoesLobbyExist(pLobbyName))
            {
                DataServer.AddLobby(pLobbyName);
                Console.WriteLine($"Lobby {pLobbyName} created successfully.");
                return true;
            }
            Console.WriteLine($"Lobby {pLobbyName} already exists.");
            return false;
        }

        public void AddUserToLobby(string pUsername, string pLobbyName)
        {
            DataServer.AddUserToLobby(pUsername, pLobbyName);
        }

        public void RemoveUserFromLobby(string pUsername, string pLobbyName)
        {
            DataServer.RemoveUserFromLobby(pUsername, pLobbyName);
            DataServer.RemoveDms(pUsername, pLobbyName);
        }

        public List<string> GetLobbyChat(string pLobbyName)
        {
            return DataServer.GetLobbyChat(pLobbyName);
        }

        public List<string> GetLobbyUsers(string pLobbyName)
        {
            return DataServer.GetLobbyUsers(pLobbyName);
        }

        public List<string> GetLobbyFileNames(string pLobbyName)
        {
            return DataServer.GetLobbyFileNames(pLobbyName);
        }

        public List<string> GetLobbyDm(string pLobbyName, string pUsername1, string pUsername2)
        {
            return DataServer.GetLobbyDm(pLobbyName, pUsername1, pUsername2);
        }

        public Object GetLobbyFile(string pLobbyName, string pFileName)
        {
            return DataServer.GetLobbyFile(pLobbyName, pFileName);
        }

        public void SendMsg(string pLobbyName, string pMessage)
        {
            DataServer.AddMsg(pLobbyName, pMessage);
        }

        public void SendDm(string pLobbyName, string pUsername1, string pUsername2, string pMessage)
        {
            DataServer.AddDm(pLobbyName, pUsername1, pUsername2, pMessage);
        }

        public void SendFile(string pLobbyName, string pFileName, Object pFileObject)
        {
            DataServer.AddFile(pLobbyName, pFileName, pFileObject);
        }

        public List<string> GetLobbyNames()
        {
            var names = DataServer.GetLobbyNames();
            Console.WriteLine($"GetLobbyNames called. Returning {names.Count} lobbies: {string.Join(", ", names)}");
            return names;
        }


        //loggining method
        public bool LoginUser(string username)
        {
            lock (loggedInUsers)
            {
                if (!loggedInUsers.Contains(username))
                {
                    loggedInUsers.Add(username);
                    return true;
                }
                return false;
            }
        }

        //remove users when they log out or disconnect
        public void LogoutUser(string username)
        {
            lock (loggedInUsers)
            {
                loggedInUsers.Remove(username);
            }
        }

    }
}