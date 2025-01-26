using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace BusinessTier
{
    [ServiceContract]
    public interface BusinessServerInterface
    {
        [OperationContract]
        bool AddUser(string pUsername);

        [OperationContract]
        void RemoveUser(string pUsername);

        [OperationContract]
        void SendMessage(string pMessage);

        [OperationContract]
        List<string> GetMessages();

        [OperationContract]
        List<string> GetUsers();
    }
}
