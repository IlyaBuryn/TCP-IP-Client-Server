using DataEditLib.Models.MessagesTypes;

namespace DataEditLib.Interfaces.Crud
{
    public interface IRead<T>
    {
        Task<IActionResult<T>> ReadOne(ClientMessage message);
        Task<IActionResult<T>> ReadAll(ClientMessage message);
    }
}
