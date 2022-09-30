namespace ManagementServer.Interfaces
{
    public interface IServer
    {
        void Start();
        byte[] FromStringToBytesIp(string ip);
        void Pause();
    }
}
