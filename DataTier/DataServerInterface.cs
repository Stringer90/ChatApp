using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace DataTier
{
    [ServiceContract]
    public interface DataServerInterface
    {
        [OperationContract]
        HashSet<string> GetUsernames();

        [OperationContract]
        void AddUser(string pUsername);

        [OperationContract]
        void RemoveUser(string pUsername);

        [OperationContract]
        bool UserExists(string pUsername);

        [OperationContract]
        void AddMessage(string pMessage);

        [OperationContract]
        List<string> GetMessages();
    }
}
