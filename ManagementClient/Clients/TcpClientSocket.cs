using DataEditLib.Data;
using DataEditLib.Interfaces;
using DataEditLib.Models.MessagesTypes;
using ManagementClient.Interfaces;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace ManagementClient.Clients
{
    public class TcpClientSocket : IClient
    {
        private TcpClient _client;
        private StreamReader _sReader;
        private StreamWriter _sWriter;

        private readonly string _ip;
        private readonly int _port;
        private readonly ILogger _log;

        private static ServerMessage? _messageToGet;
        private static ClientMessage? _messageToSend;

        public TcpClientSocket(ILogger log)
        {
            _ip = ProjectProperties.TcpClientServerIp;
            _port = ProjectProperties.TcpClientServerPort;
            _log = log;
        }

        public TcpClientSocket(int port, string ip, ILogger log)
        {
            _ip = ip;
            _port = port;
            _log = log;
        }

        // Making new thread to connect to server
        private void StartConnection()
        {
            try
            {
                _client = new TcpClient();
                _client.Connect(_ip, _port);

                _messageToSend.IpAddress = GetLocalIPAddress();
                _messageToSend.Port = ((IPEndPoint)_client.Client.RemoteEndPoint).Port;

                HandleCommunication();
                //Thread t = new Thread(new ThreadStart(HandleCommunication));
                //t.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void HandleCommunication()
        {
            if (_messageToSend != null)
            {
                // Creation I/O streams
                Task.Delay(10);
                _sReader = new StreamReader(_client.GetStream(), Encoding.ASCII);
                Task.Delay(10);
                _sWriter = new StreamWriter(_client.GetStream(), Encoding.ASCII);


                // Sending message as CVS string and getting info about request; cloasing stream
                _sWriter.WriteLine(JsonSerializer.Serialize(_messageToSend));
                _log.Log(_messageToSend, DataEditLib.Enums.SenderType.Client);
                _sWriter.Flush();


                // Waiting server response and getting info about response
                var jsonMessage = _sReader.ReadLine();
                _messageToGet = new ServerMessage();
                _messageToGet = JsonSerializer.Deserialize<ServerMessage>(jsonMessage);
                _log.Log(_messageToGet, DataEditLib.Enums.SenderType.Client);


                // Cloasing all
                Console.WriteLine();
                _sWriter.Close();
                _client.Close();
                Clear();
            }
        }
        
        // Clearing messages variables
        private void Clear()
        {
            _messageToGet = null;
            _messageToSend = null;
        }


        // Send message method
        public void SendMessage(ClientMessage message)
        {
            _messageToSend = message;
            _messageToSend.SenderType = DataEditLib.Enums.SenderType.Client;
            StartConnection();
        }

        private string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
    }
}
