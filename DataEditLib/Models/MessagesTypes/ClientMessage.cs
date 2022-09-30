using DataEditLib.Enums;
using System.Net;
using System.Net.Sockets;
using System.Text.Json.Serialization;

namespace DataEditLib.Models.MessagesTypes
{
    public class ClientMessage : Message
    {
        public ClientMessage() { }
        public ClientMessage(MessageType msgType)
            : base(msgType) { }
        public ClientMessage(string value, MessageType msgType)
            : base(value, msgType) { }
        [JsonConstructor]
        public ClientMessage(string Value, string IpAddress, int Port, DateTime Date,
            SenderType SenderType, MessageStatus MsgStatus, MessageType MsgType)
            : base(Value, IpAddress, Port, Date, SenderType, MsgStatus, MsgType) { }

        public override string Info(SenderType loggerDefine)
        {
            if (loggerDefine == SenderType.Server)
                // server : to send
                return $"{SenderType}:  {IpAddress}:{Port} Send message [{Date}; Status: {MsgStatus}; Type: {MsgType}]:\n{Value}\n";
            else
                // client : to get
                return $"{SenderType}:  Send message [Status: {MsgStatus}; Type: {MsgType}]:\n{Value}\n";
        }
    }

}
