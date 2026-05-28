namespace FactoryLib
{
    public class RawMaterial
    {
        public int Id { get; }
        public string Name { get; }
        public string Unit { get; } // "кг", "шт"
        public decimal CostPerUnit { get; set; }

        public RawMaterial(int id, string name, string unit, decimal costPerUnit)
        {
            Id = id;
            Name = name;
            Unit = unit;
            CostPerUnit = costPerUnit;
        }

        public void Display()
        {
            Console.WriteLine($"Сырьё: {Name}, Ед.изм: {Unit}, Цена: {CostPerUnit:C}");
        }
    }
}
