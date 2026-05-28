using System;

namespace FactoryLib
{
    public class Machine : IEntity, IMaintainable
    {
        public int Id { get; }
        public string Model { get; }
        public bool IsOperational { get; private set; }
        public DateTime LastMaintenance { get; private set; }
        public Department Department { get; set; }

        public Machine(int id, string model, bool isOperational, DateTime lastMaintenance)
        {
            Id = id;
            Model = model;
            IsOperational = isOperational;
            LastMaintenance = lastMaintenance;
        }

        public bool NeedsMaintenance => (DateTime.Today - LastMaintenance).Days > 30;

        public void PerformMaintenance(string description, string technician)
        {
            LastMaintenance = DateTime.Today;
            IsOperational = true;
            Console.WriteLine($"Станок {Model} прошёл техобслуживание ({description}). Техник: {technician}.");
        }

        public int Produce(string productName, int rawQuantity, int consumptionPerUnit)
        {
            if (!IsOperational)
            {
                Console.WriteLine($"Станок {Model} неисправен, производство невозможно.");
                return 0;
            }
            int producedUnits = rawQuantity / consumptionPerUnit;
            if (producedUnits > 0)
                Console.WriteLine($"Станок {Model} произвёл {producedUnits} ед. продукта '{productName}'.");
            else
                Console.WriteLine($"Недостаточно сырья для производства '{productName}'.");
            return producedUnits;
        }
    }
}