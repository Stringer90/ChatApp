using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTier
{
    public class Lobby
    {
        private List<string> Users = new List<string>();
        private List<string> Messages = new List<string>();
        private List<string> FileNames = new List<string>();
        public Dictionary<string, Dictionary<string, List<string>>> Dms = new Dictionary<string, Dictionary<string, List<string>>>();
        private Dictionary<string, Object> Files = new Dictionary<string, Object>();

        public Lobby() { }

        public void AddUser(string pUsername)
        {
            Users.Add(pUsername);
        }

        public void RemoveUser(string pUsername)
        {
            // Remove username from users list
            Users.Remove(pUsername);

            // Remove Dms entry for key = pUsername
            // (dms from user to others)
            Dms.Remove(pUsername);

            // Remove Dms sub-entry for key = pUsername
            // (dms from others to user being removed)
            foreach (var user1 in Dms.Keys)
            {
                Dms[user1].Remove(pUsername);
            }
        }

        public List<string> GetChat()
        {
            return Messages;
        }

        public List<string> GetUsers()
        {
            return Users;
        }

        public List<string> GetFileNames()
        {
            return FileNames;
        }

        public List<string> GetDm(string pUser1, string pUser2)
        {
            // If one or more users not in lobby, return null
            // null = grey out
            if (!Users.Contains(pUser1) || !Users.Contains(pUser2))
            {
                return null;
            }

            List<string> result = new List<string>();

            // If no dms sent between the 2 users, return empty list
            if (!Dms.ContainsKey(pUser1))
            {
                return result;
            }
            else if (!Dms[pUser1].ContainsKey(pUser2))
            {
                return result;
            }

            return Dms[pUser1][pUser2];
        }

        public Object GetFile(string pFileName)
        {
            return Files[pFileName];
        }

        public void AddMsg(string PMessage)
        {
            Messages.Add(PMessage);
        }

        public void AddDm(string pUser1, string pUser2, string pMessage)
        {
            // Add direct message from user1 to user2
            if (!Dms.ContainsKey(pUser1))
            {
                Dms[pUser1] = new Dictionary<string, List<string>>();
            }
            if (!Dms[pUser1].ContainsKey(pUser2))
            {
                Dms[pUser1][pUser2] = new List<string>();
            }
            Dms[pUser1][pUser2].Add(pMessage);

            // Add direct message from user2 to user1
            if (!Dms.ContainsKey(pUser2))
            {
                Dms[pUser2] = new Dictionary<string, List<string>>();
            }
            if (!Dms[pUser2].ContainsKey(pUser1))
            {
                Dms[pUser2][pUser1] = new List<string>();
            }
            Dms[pUser2][pUser1].Add(pMessage);

        }

        public void AddFile(string pFileName, Object pFileObject)
        {
            if (!Files.ContainsKey(pFileName))
            {
                Files[pFileName] = new Object();
            }
            Files[pFileName] = pFileObject;
        }

    }
}
