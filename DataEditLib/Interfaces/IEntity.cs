namespace DataEditLib.Interfaces
{
    public interface IEntity
    {
        int Id { get; set; }
        IEntity ToObjectFromText(string info);
        string ToString();
        void UpdateCurrent(IEntity newEntity);
    }
}
