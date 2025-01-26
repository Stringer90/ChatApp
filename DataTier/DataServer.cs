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
        private readonly object _messagesLockLock = new object();

        static DataServer()
        {
            Usernames = new HashSet<string>();
            Messages = new List<string>();
        }

        List<string> GetUsernames()
        {

        }

        void AddUser(string pUsername)
        {

        }

        void RemoveUser(string pUsername)
        {

        }

        void AddMessage(string pMessage)
        {

        }

        List<string> GetMessages()
        {

        }
    }
}
