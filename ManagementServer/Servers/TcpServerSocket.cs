using DataEditLib.Data;
using DataEditLib.Interfaces;
using DataEditLib.Models.MessagesTypes;
using ManagementServer.Interfaces;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace ManagementServer.Servers
{
    public class TcpServerSocket : IServer
    {
        private TcpListener _server;
        private bool _isRunning;

        private readonly string _ip;
        private readonly int _port;
        private readonly IDataEdit _data;
        private readonly ILogger _log;

        private static ServerMessage? _messageToSend;
        private static ClientMessage? _messageToGet;

        public TcpServerSocket(IDataEdit dataEditService, ILogger log)
        {;
            _ip = ProjectProperties.TcpClientServerIp;
            _port = ProjectProperties.TcpClientServerPort;

            _data = dataEditService;
            _log = log;
        }

        public TcpServerSocket(string ip, int port, IDataEdit dataEditService, ILogger log)
        {
            _ip = ip;
            _port = port;

            _data = dataEditService;
            _log = log;
        }

        public void Start()
        {
            _server = new TcpListener(
                new IPAddress(FromStringToBytesIp(_ip)), _port);
            _isRunning = true;

            _server.Start();
            Thread th = new Thread(Listen);
            th.Start();
        }

        public byte[] FromStringToBytesIp(string ip) => ip.Split('.').Select(byte.Parse).ToArray();

        private void Listen()
        {
            while (_isRunning)
            {
                try
                {
                    // Waiting client
                    TcpClient newClient = _server.AcceptTcpClient();
                    if (newClient != null)
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(HandleClient));
                        t.Start(newClient);
                    }
                }
                catch (Exception ex)
                {
                    _log.Log(ex);
                }
            }
        }

        public async void HandleClient(object obj)
        {
            // [Need boxing for threads] 
            TcpClient client = (TcpClient)obj;


            // Making I/O streams
            StreamWriter _sWriter = new StreamWriter(client.GetStream(), Encoding.ASCII);
            StreamReader _sReader = new StreamReader(client.GetStream(), Encoding.ASCII);


            // Getting request from client (as json string)
            var jsonMessage = _sReader.ReadLine();
            _messageToGet = new ClientMessage();
            _messageToGet = JsonSerializer.Deserialize<ClientMessage>(jsonMessage);
            _log.Log(_messageToGet, DataEditLib.Enums.SenderType.Server);


            // Define operation and sending response
            _messageToSend = await _data.ParseMessageType(_messageToGet);
            _sWriter.WriteLine(JsonSerializer.Serialize(_messageToSend));
            _log.Log(_messageToSend, DataEditLib.Enums.SenderType.Server);


            _sWriter.Flush();
            Clear();
        }

        public void Pause() => _isRunning = false;

        private void Clear()
        {
            _messageToSend = null;
            _messageToGet = null;
        }
    }
}
