using DataEditLib.Models.MessagesTypes;

namespace ManagementClient.Interfaces
{
    public interface IClient
    {
        void SendMessage(ClientMessage message);
    }
}
