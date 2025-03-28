using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System;

class Program {
    static async Task Main()
    {
        int port = 8000;
        var udpServer = new UdpClient(port);
        Console.WriteLine($"UDP сервер працює на порту {port}");

        while (true)
        {
            try
            {
                var result = await udpServer.ReceiveAsync();
                _ = Task.Run(() => HandleClientAsync(result, udpServer));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Receive error: {ex.Message}");
            }
        }
    }

    static async Task HandleClientAsync(UdpReceiveResult result, UdpClient server)
    {
        try
        {
            var clientEP = result.RemoteEndPoint;
            var message = Encoding.UTF8.GetString(result.Buffer);
            Console.WriteLine($"[{clientEP}] => {message}");

            var response = new string('0', message.Length);
            byte[] responseBytes = Encoding.UTF8.GetBytes(response);

            await server.SendAsync(responseBytes, responseBytes.Length, clientEP);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Handle error: {ex.Message}");
        }
    }
}

