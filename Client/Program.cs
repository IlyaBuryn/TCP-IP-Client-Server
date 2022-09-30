// Client Console
using DataEditLib.Enums;
using DataEditLib.Interfaces;
using DataEditLib.Logger;
using DataEditLib.Models.MessagesTypes;
using ManagementClient.Clients;


ILogger _logger = new CL();
var client = new TcpClientSocket(_logger);


using (StreamReader f = new StreamReader("../../../../TestData/Script_1.txt"))
{
    string line = string.Empty;
    ClientMessage message = new ClientMessage();
    while ((line = f.ReadLine()) != null)
    {
        
        var objs = line.Split('|');
        var msgType = (MessageType)Enum.Parse(typeof(MessageType), objs[0]);
        message = new ClientMessage(objs[1], msgType);   
        Console.WriteLine(msgType + ": " + objs[1]);

        client.SendMessage(message);

        Task.Delay(500).Wait();
    }
}


Console.ReadLine();