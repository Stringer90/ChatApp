using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace BusinessTier
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Business server is starting up.");
            ServiceHost host;
            NetTcpBinding tcp = new NetTcpBinding();
            host = new ServiceHost(typeof(BusinessServer));
            host.AddServiceEndpoint(typeof(BusinessServerInterface), tcp, "net.tcp://localhost:8200/BusinessServerService");

            host.Open();

            Console.WriteLine("Business server is online.");
            Console.ReadLine();

            host.Close();
        }
    }
}
