using DataEditLib.Enums;
using DataEditLib.Interfaces;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text.Json.Serialization;

namespace DataEditLib.Models.MessagesTypes
{
    public class ServerMessage : Message
    {
        public ServerMessage() { }
        public ServerMessage(MessageType msgType)
            : base(msgType) { }
        public ServerMessage(string value, MessageType msgType)
            : base(value, msgType) { }
        [JsonConstructor]
        public ServerMessage(string Value, string IpAddress, int Port, DateTime Date,
            SenderType SenderType, MessageStatus MsgStatus, MessageType MsgType) 
            : base(Value, IpAddress, Port, Date, SenderType, MsgStatus, MsgType) { }


        public ServerMessage(Message oldMessage) : base()
        {
            Value = oldMessage.Value;
            IpAddress = oldMessage.IpAddress;
            Port = oldMessage.Port;
            SenderType = SenderType.Server;
        }

        public override string Info(SenderType loggerDefine)
        {
            if (loggerDefine == SenderType.Server)
                // server : to get
                return $"{SenderType}:  {IpAddress}:{Port} Send message [{Date}; Status: {MsgStatus}; Type: {MsgType}]:\n";
            else
                // client : to send
                return $"{SenderType}:  {IpAddress}:{Port} Send message [{Date}; Status: {MsgStatus}; Type: {MsgType}]:\n{Value}\n";
        }

        public static ServerMessage ServerResponse(IActionResult<MyEntity> actionResult, Message oldMessage)
        {
            var sMessage = new ServerMessage(oldMessage);
            sMessage.MsgStatus = actionResult.MessageStatus;

            sMessage.Value = actionResult.Value;
            return sMessage;
        }
    }
}
