using DataEditLib.Models.MessagesTypes;

namespace DataEditLib.Interfaces
{
    public interface IDataEdit
    {
        Task<ServerMessage> ParseMessageType(ClientMessage cMsg);

    }
}
