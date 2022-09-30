using DataEditLib.Enums;
using DataEditLib.Models.MessagesTypes;

namespace DataEditLib.Interfaces
{
    public interface IActionResult<T>
    {
        ClientMessage ClientMsg { get; }
        MessageStatus MessageStatus { get; }
        Exception Exception { get; }
        string Value { get; }
    }
}
