using DataEditLib.Models.MessagesTypes;

namespace DataEditLib.Interfaces.Crud
{
    public interface IUpdate<T>
    {
        Task<IActionResult<T>> Update(ClientMessage message);
    }
}
