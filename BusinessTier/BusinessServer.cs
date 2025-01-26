using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

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

        }

        [OperationContract]
        public void RemoveUser(string pUsername)
        {

        }

        [OperationContract]
        public void SendMessage(string pMessage)
        {

        }

        [OperationContract]
        public List<string> GetMessages()
        {

        }

        [OperationContract]
        public List<string> GetUsers()
        {

        }




    }
}
