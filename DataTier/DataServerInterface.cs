using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace DataTier
{
    [ServiceContract]
    public interface DataServerInterface
    {
        [OperationContract]
        List<string> GetUserNames();

        [OperationContract]
        List<string> GetLobbyNames();

        [OperationContract]
        void AddUser(string pUsername);

        [OperationContract]
        void RemoveUser(string pUsername);

        [OperationContract]
        void AddLobby(string pLobbyName);

        [OperationContract]
        void AddUserToLobby(string pUsername, string pLobbyName);

        [OperationContract]
        void RemoveUserFromLobby(string pUsername, string pLobbyName);

        [OperationContract]
        List<string> GetLobbyChat(string pLobbyName);

        [OperationContract]
        List<string> GetLobbyUsers(string pLobbyName);

        [OperationContract]
        List<string> GetLobbyFileNames(string pLobbyName);

        [OperationContract]
        List<string> GetLobbyDm(string pLobbyName, string pUser1, string pUser2);

        [OperationContract]
        Object GetLobbyFile(string pLobbyName, string pFileName);

        [OperationContract]
        void AddMsg(string pLobbyName, string pMessage);

        [OperationContract]
        void AddDm(string pLobbyName, string pUser1, string pUser2, string pMessage);

        [OperationContract]
        void AddFile(string pLobbyName, string pFileName, Object pFileObject);

        [OperationContract]
        void RemoveDms(string pUsername, string pLobbyName);

       
    }

}
