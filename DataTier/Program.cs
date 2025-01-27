using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace DataTier
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Data server is starting up.");
            ServiceHost host;
            NetTcpBinding tcp = new NetTcpBinding();
            host = new ServiceHost(typeof(DataServer));
            host.AddServiceEndpoint(typeof(DataServerInterface), tcp, "net.tcp://localhost:8100/DataServerService");

            host.Open();

            Console.WriteLine("Data server is online.");
            Console.ReadLine();

            host.Close();
        }
    }
}
