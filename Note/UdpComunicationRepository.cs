using Newtonsoft.Json;
using Note.Models;
using Speaker.leison.Sistem.layer.Repositories;
using System.Net.Sockets;
using System.Net;
using System.Text;

public class UdpComunicationRepository : IUdpComunicationRepository
{
    private readonly int _port = 1192;

    public async Task<List<ExcellDataMode3l>> ReceiveAsync()
    {
        using (var client = new UdpClient())
        {
            client.ExclusiveAddressUse = false; // Allow multiple clients to bind to the same address/port
            client.Client.Bind(new IPEndPoint(IPAddress.Any, _port));

            try
            {
                await Console.Out.WriteLineAsync("Start");
                var clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                var receivedInfo = await client.ReceiveAsync(); // Use async receive
                string message = Encoding.UTF8.GetString(receivedInfo.Buffer);
                client.Dispose();
                client.Close();
                return JsonConvert.DeserializeObject<List<ExcellDataMode3l>>(message);
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"SocketException: {ex.Message}"); // Log socket exception details
                throw;
            }
            catch (Exception exp)
            {
                Console.WriteLine($"Exception: {exp.Message}"); // Log other exceptions
                throw;
            }
        }
    }
}
