using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace DataTier
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class DataServer
    {
        private HashSet<string> Usernames;
        private List<string> Messages;
        private readonly object _usersLock = new object();
        private readonly object _messagesLock = new object();

        public DataServer()
        {
            Usernames = new HashSet<string>();
            Messages = new List<string>();
        }

        public HashSet<string> GetUsernames()
        {
            lock (_usersLock)
            {
                return new HashSet<string>(Usernames);
            }
        }

        public void AddUser(string pUsername)
        {
            lock (_usersLock)
            {
                Usernames.Add(pUsername);
            }
        }

        public void RemoveUser(string pUsername)
        {
            lock (_usersLock)
            {
                Usernames.Remove(pUsername);
            }
        }

        public bool UserExists(string pUsername)
        {
            lock (_usersLock)
            {
                return Usernames.Contains(pUsername);
            }
        }

        public void AddMessage(string pMessage)
        {
            lock (_messagesLock)
            {
                Messages.Add(pMessage);
            }
        }

        public List<string> GetMessages()
        {
            lock (_messagesLock)
            {
                return new List<string>(Messages);
            }
        }
    }
}
