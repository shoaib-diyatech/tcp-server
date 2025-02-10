using System;
using System.Threading.Tasks;
using System.Configuration;
using log4net;
using log4net.Config;

class Program
{
    public static readonly ILog log = LogManager.GetLogger(typeof(TcpServer));
    static void Main()
    {
        ConfigureLogging();
        // See https://aka.ms/new-console-template for more information
        Console.WriteLine("Hello, Tcp-Server World!");

        string port = ConfigurationManager.AppSettings["port"] ?? "8080";
        Console.WriteLine("Starting TCP-SERVER on Port: " + port);
        log.Info($"Server is starting on port: [{port}]...");
        TcpServer server = new TcpServer(int.Parse(port), null);
        server.Start().Wait();
    }
    private static void ConfigureLogging()
    {
        var logRepository = LogManager.GetRepository(System.Reflection.Assembly.GetEntryAssembly());
        XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
    }
}