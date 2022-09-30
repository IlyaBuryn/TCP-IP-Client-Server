using DataEditLib.Enums;
using DataEditLib.Models;

namespace DataEditLib.Interfaces
{
    public interface ILogger
    {
        void Log(string message);
        void Log(Exception exception);
        void Log(Message message, SenderType loggerDefine);
    }
}
