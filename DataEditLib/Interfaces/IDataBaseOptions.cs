using DataEditLib.Models.MessagesTypes;

namespace DataEditLib.Interfaces
{
    public interface IDataBaseOptions<T>
    {
        Task<IActionResult<T>> CreateDataBase(ClientMessage message);
        Task<IActionResult<T>> DeleteDataBase(ClientMessage message);
    }
}
