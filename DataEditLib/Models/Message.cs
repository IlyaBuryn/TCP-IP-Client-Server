using DataEditLib.Enums;
using System.Net;
using System.Net.Sockets;
using System.Text.Json.Serialization;

namespace DataEditLib.Models
{
    public abstract class Message
    {
        public Message() 
        {
            Date = DateTime.Now;
        }

        [JsonConstructor]
        public Message(string Value, string IpAddress, int Port, DateTime Date, 
            SenderType SenderType, MessageStatus MsgStatus, MessageType MsgType)
        {
            this.Value = Value;
            this.IpAddress = IpAddress;
            this.Port = Port;
            this.Date = Date;
            this.SenderType = SenderType;
            this.MsgStatus = MsgStatus;
            this.MsgType = MsgType;
        }

        public Message(string value, MessageType msgType)
        {
            Value = value;
            MsgType = msgType;
            Date = DateTime.Now;
        }

        public Message(MessageType msgType)
        {
            MsgType = msgType;
            Date = DateTime.Now;
        }

        public string Value { get; set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public DateTime Date { get; set; }
        public SenderType SenderType { get; set; }
        public MessageStatus MsgStatus { get; set; }
        public MessageType MsgType { get; set; }

        public abstract string Info(SenderType loggerDefine);

        public static string CollectionToOneString<T>(IEnumerable<T> collection) where T : class
        {
            var dataToTransfer = string.Empty;
            foreach (var entity in collection)
                dataToTransfer += entity.ToString() + "\r\n";
            return dataToTransfer;
        }
    }
}
