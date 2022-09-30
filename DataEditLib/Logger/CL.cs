using DataEditLib.Enums;
using DataEditLib.Interfaces;
using DataEditLib.Models;

namespace DataEditLib.Logger
{
    public class CL : ILogger
    {
        public void Log(string message)
        {
            Reset();
            Console.WriteLine(message);
        }

        public void Log(Exception exception)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(exception.Message);
            Reset();
        }

        public void Log(Message message, SenderType loggerDefine)
        {
            if (message.MsgStatus == Enums.MessageStatus.Error)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(message.Info(loggerDefine));
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(message.Info(loggerDefine));
            }
            Reset();
        }

        private void Reset()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
