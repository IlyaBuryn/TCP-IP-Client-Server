using DataEditLib.Models.MessagesTypes;

namespace DataEditLib.Interfaces.Crud
{
    public interface IDelete<T>
    {
        Task<IActionResult<T>> Delete(ClientMessage message);
    }
}
