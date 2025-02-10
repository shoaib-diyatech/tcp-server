using System;
using System.Threading.Tasks;
using System.Configuration;

class Program
{
    static void Main()
    {
        // See https://aka.ms/new-console-template for more information
        Console.WriteLine("Hello, Tcp-Server World!");
        
        string port = ConfigurationManager.AppSettings["port"] ?? "8080";
        Console.WriteLine("Starting TCP-SERVER on Port: " + port);
        TcpServer server = new TcpServer(int.Parse(port), null);
        server.Start().Wait();
    }
}