using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

class TcpServer
{
    private static TcpListener? _listener;
    private int Port;
    private int? MaxConnections;
    private SemaphoreSlim _connectionLimiter;

    /**
    * Create a TcpServer instance , defaults to 10 number of connections if null is provided
    */
    public TcpServer(int port, int? maxConnections)
    {
        Port = port;
        MaxConnections = maxConnections ?? 10;
        _connectionLimiter = new(MaxConnections.Value);
    }

    public async Task Start()
    {
        _listener = new TcpListener(IPAddress.Any, Port);
        _listener.Start();
        Console.WriteLine($"Server started on port {Port}");

        while (true)
        {
            TcpClient client = await _listener.AcceptTcpClientAsync();
            _ = HandleClientAsync(client); // Fire-and-forget to handle multiple clients
        }
    }

    private async Task HandleClientAsync(TcpClient client)
    {
        await _connectionLimiter.WaitAsync(); // Limit concurrent clients
        Console.WriteLine($"Client connected: {client.Client.RemoteEndPoint}");

        NetworkStream stream = client.GetStream();
        byte[] buffer = new byte[1024];

        try
        {
            while (true)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead == 0) break; // Client disconnected

                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"Received: {message}");

                // Echo back the received message
                byte[] response = Encoding.UTF8.GetBytes($"Echo: {message}");
                await stream.WriteAsync(response, 0, response.Length);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        finally
        {
            client.Close();
            _connectionLimiter.Release(); // Free up a connection slot
            Console.WriteLine("Client disconnected.");
        }
    }
}