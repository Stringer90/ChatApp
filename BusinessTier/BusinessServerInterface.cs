using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace BusinessTier
{
    [ServiceContract]
    public interface BusinessServerInterface
    {
        [OperationContract]
        bool CreateUser(string pUsername);

        [OperationContract]
        void RemoveUser(string pUsername);

        [OperationContract]
        bool CreateLobby(string pLobbyName);

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
        List<string> GetLobbyDm(string pLobbyName, string pUsername1, string pUsername2);

        [OperationContract]
        List<string> GetLobbyNames();

        [OperationContract]
        Object GetLobbyFile(string pLobbyName, string pFileName);

        [OperationContract]
        void SendMsg(string pLobbyName, string pMessage);

        [OperationContract]
        void SendDm(string pLobbyName, string pUsername1, string pUsername2, string pMessage);

        [OperationContract]
        void SendFile(string pLobbyName, string pFileName, Object pFileObject);

        [OperationContract]
        bool LoginUser(string username);

       

    }
}