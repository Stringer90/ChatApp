using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using DataTier;

namespace BusinessTier
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class BusinessServer : BusinessServerInterface
    {
        private static readonly DataServerInterface DataServerService;

        static BusinessServer()
        {
            ChannelFactory<DataServerInterface> dataServerFactory;
            NetTcpBinding tcp = new NetTcpBinding();
            string URL = "net.tcp://0.0.0.0:8100/DataServerService";
            dataServerFactory = new ChannelFactory<DataServerInterface>(tcp, URL);
            DataServerService = dataServerFactory.CreateChannel();
        }

        public bool AddUser(string pUsername)
        {
            if (DataServerService.UserExists(pUsername))
            {
                return false;
            }
            DataServerService.AddUser(pUsername);
            return true;
        }

        public void RemoveUser(string pUsername)
        {
            DataServerService.RemoveUser(pUsername);
        }

        public void SendMessage(string pMessage)
        {
            DataServerService.AddMessage(pMessage);
        }

        public List<string> GetMessages()
        {
            return DataServerService.GetMessages();
        }

        public List<string> GetUsers()
        {
            return new List<string>(DataServerService.GetUsernames());
        }
    }
}
