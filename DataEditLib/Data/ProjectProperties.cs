namespace DataEditLib.Data
{
    public static class ProjectProperties
    {
        public const int TcpClientServerPort = 59267;
        public const string TcpClientServerIp = "192.168.56.1";

        public const string TextFilePath = "../../../../TestData/Data.txt";
        public const string SqliteFilePath = "../../../../TestData/SqliteData.db";
        public const string SqliteDataString = "Data Source=" + SqliteFilePath;

        public const string MessageDefineError = "Message type definition error";
        public const string MessageTypeNotDefine = "Wrong message type. Can't find [Create/ReadOne/ReadAll/Update/Delete] or [CreateDatabase/DeleteDatabase]";

        public const string OkCreatedInfo = "Object was created";
        public const string OkUpdatedInfo = "Object was updated";
        public const string OkDeletedInfo = "Object was deleted";
    }
}
