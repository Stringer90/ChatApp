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
            //Start the server
            Console.WriteLine("Welcome to my Data Server! :)");
            var tcp = new NetTcpBinding();
            //Bind the interface
            //Create the host
            var host = new ServiceHost(typeof(DataServer));
            host.AddServiceEndpoint(typeof(DataTier.DataServerInterface), tcp, "net.tcp://0.0.0.0:8100/DataService");
            host.Open();
            //Hold the server open until someone does something
            Console.WriteLine("System Online");
            Console.ReadLine();
            //Close the host
            host.Close();
        }
    }
}
