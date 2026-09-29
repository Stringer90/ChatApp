using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace DataTier
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class DataServer : DataServerInterface
    {
        private static ConcurrentDictionary<string, Lobby> Lobbies = new ConcurrentDictionary<string, Lobby>();
        private List<string> Users;
        private readonly object _usersLock = new object();

        public DataServer()
        {
            Lobbies = new ConcurrentDictionary<string, Lobby>();
            Users = new List<string>();
        }

        public List<string> GetUserNames()
        {
            lock (_usersLock)
            {
                return new List<string>(Users);
            }
        }

        public List<string> GetLobbyNames()
        {
            var names = Lobbies.Keys.ToList();
            Console.WriteLine($"GetLobbyNames called. Returning {names.Count} lobbies: {string.Join(", ", names)}");
            return names;
        }

        public void AddUser(string pUsername)
        {
            lock (_usersLock)
            {
                if (!Users.Contains(pUsername))
                {
                    Users.Add(pUsername);
                }
            }
        }

        public void RemoveUser(string pUsername)
        {
            lock (_usersLock)
            {
                Users.Remove(pUsername);
            }
            // Remove user from all lobbies
            foreach (var lobby in Lobbies.Values)
            {
                lobby.RemoveUser(pUsername);
            }
        }

        public void AddLobby(string pLobbyName)
        {
            if (Lobbies.TryAdd(pLobbyName, new Lobby()))
            {
                Console.WriteLine($"Lobby '{pLobbyName}' added successfully.");
            }
            else
            {
                Console.WriteLine($"Failed to add lobby '{pLobbyName}'. It may already exist.");
            }
        }

        public void AddUserToLobby(string pUsername, string pLobbyName)
        {
            if (Lobbies.TryGetValue(pLobbyName, out Lobby lobby))
            {
                lobby.AddUser(pUsername);
            }
        }

        public void RemoveUserFromLobby(string pUsername, string pLobbyName)
        {
            if (Lobbies.TryGetValue(pLobbyName, out Lobby lobby))
            {
                lobby.RemoveUser(pUsername);
            }
        }

        public List<string> GetLobbyChat(string pLobbyName)
        {
            return Lobbies.TryGetValue(pLobbyName, out Lobby lobby) ? lobby.GetChat() : new List<string>();
        }

        public List<string> GetLobbyUsers(string pLobbyName)
        {
            return Lobbies.TryGetValue(pLobbyName, out Lobby lobby) ? lobby.GetUsers() : new List<string>();
        }

        public List<string> GetLobbyFileNames(string pLobbyName)
        {
            return Lobbies.TryGetValue(pLobbyName, out Lobby lobby) ? lobby.GetFileNames() : new List<string>();
        }

        public List<string> GetLobbyDm(string pLobbyName, string pUser1, string pUser2)
        {
            return Lobbies.TryGetValue(pLobbyName, out Lobby lobby) ? lobby.GetDm(pUser1, pUser2) : null;
        }

        public Object GetLobbyFile(string pLobbyName, string pFileName)
        {
            return Lobbies.TryGetValue(pLobbyName, out Lobby lobby) ? lobby.GetFile(pFileName) : null;
        }

        public void AddMsg(string pLobbyName, string pMessage)
        {
            if (Lobbies.TryGetValue(pLobbyName, out Lobby lobby))
            {
                lobby.AddMsg(pMessage);
            }
        }

        public void AddDm(string pLobbyName, string pUser1, string pUser2, string pMessage)
        {
            if (Lobbies.TryGetValue(pLobbyName, out Lobby lobby))
            {
                lobby.AddDm(pUser1, pUser2, pMessage);
            }
        }

        public void AddFile(string pLobbyName, string pFileName, Object pFileObject)
        {
            if (Lobbies.TryGetValue(pLobbyName, out Lobby lobby))
            {
                lobby.AddFile(pFileName, pFileObject);
            }
        }

        public void RemoveDms(string pUsername, string pLobbyName)
        {
            if (Lobbies.TryGetValue(pLobbyName, out Lobby lobby))
            {
                //lobby.RemoveDms(pUsername);
            }
        }

        
    }
}