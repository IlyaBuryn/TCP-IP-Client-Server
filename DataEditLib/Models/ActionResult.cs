using DataEditLib.Enums;
using DataEditLib.Interfaces;
using DataEditLib.Models.MessagesTypes;

namespace DataEditLib.Models
{
    public class ActionResult<T> : IActionResult<T>
    {
        public ActionResult() {}
        public ActionResult(ClientMessage client, MessageStatus messageStatus, Exception exception)
        {
            ClientMsg = client;
            MessageStatus = messageStatus;
            Exception = exception;
            Value = exception.Message;
        }

        public ActionResult(ClientMessage client, MessageStatus messageStatus, string value)
        {
            ClientMsg = client;
            MessageStatus = messageStatus;
            Value = value;
        }

        public ClientMessage ClientMsg { get; }
        public MessageStatus MessageStatus { get; }
        public Exception Exception { get; }
        public string Value { get; }
    }
}
