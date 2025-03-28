using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class Program
{
    static void Main()
    {
        IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, 8000);
        Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        serverSocket.Bind(ipEndPoint);
        serverSocket.Listen(100);

        Console.WriteLine("Waiting for TCP connections...");

        while (true)
        {
            Socket clientSocket = serverSocket.Accept();
            Console.WriteLine("Client connected!");
            Task.Run(() => HandleClient(clientSocket));
        }
    }

    static void HandleClient(Socket clientSocket)
    {
        try
        {
            byte[] buffer = new byte[1024];

            while (true)
            {
                int bytesReceived = clientSocket.Receive(buffer);
                if (bytesReceived == 0) break;

                string message = Encoding.UTF8.GetString(buffer, 0, bytesReceived);
                Console.WriteLine($"Received message: {message}");

                string response = new string('0', message.Length) + "\n";
                byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                clientSocket.Send(responseBytes);
                Console.WriteLine($"Sent message: {response}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        finally
        {
            clientSocket.Close();
        }
    }
}
