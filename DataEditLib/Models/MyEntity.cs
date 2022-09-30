using DataEditLib.Interfaces;

namespace DataEditLib.Models
{
    public class MyEntity : IEntity
    {
        public MyEntity() { }
        public MyEntity(int id, string name, double scopeOfWork, double unitPrice, double accruedEarnings)
        {
            Id = id;
            Name = name;
            ScopeOfWork = scopeOfWork;
            UnitPrice = unitPrice;
            AccruedEarnings = accruedEarnings;
        }
        public int Id { get; set; }
        public string? Name { get; set; }
        public double? ScopeOfWork { get; set; }
        public double? UnitPrice { get; set; }
        public double? AccruedEarnings { get; set; }

        public override string ToString()
        {
            return Id.ToString() + ',' +
                Name.ToString() + ',' +
                ScopeOfWork.ToString() + ',' +
                UnitPrice.ToString() + ',' +
                AccruedEarnings.ToString() + ';';
        }

        public IEntity ToObjectFromText(string info)
        {
            info = info.Replace("\r\n", string.Empty).Trim(';');
            var props = info.Split(',');

            Id = int.Parse(props[0]);
            Name = props[1];
            ScopeOfWork = double.Parse(props[2]);
            UnitPrice = double.Parse(props[3]);
            AccruedEarnings = double.Parse(props[4]);

            return this;
        }

        public void UpdateCurrent(IEntity newEntity)
        {
            var tmp = newEntity as MyEntity;
            this.Name = tmp.Name;
            this.AccruedEarnings = tmp.AccruedEarnings;
            this.ScopeOfWork = tmp.ScopeOfWork;
            this.UnitPrice = tmp.UnitPrice;
        }
    }
}
