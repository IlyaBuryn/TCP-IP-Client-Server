using DataEditLib.Models.MessagesTypes;

namespace DataEditLib.Interfaces.Crud
{
    public interface ICreate<T>
    {
        Task<IActionResult<T>> Create(ClientMessage message);
    }
}
