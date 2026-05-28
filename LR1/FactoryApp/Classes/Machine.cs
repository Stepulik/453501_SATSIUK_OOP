using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryLib
{
    public class Machine
    {
        public int Id { get; }
        public string Model { get; }
        public bool IsOperational { get; private set; }
        public DateTime LastMaintenance { get; private set; }

        public Machine(int id, string model, bool isOperational, DateTime lastMaintenance)
        {
            Id = id;
            Model = model;
            IsOperational = isOperational;
            LastMaintenance = lastMaintenance;
        }
        public void SetOperational()
        {
            IsOperational = true;
            LastMaintenance = DateTime.Today;
        }

        public void SetNotOperational()
        {
            IsOperational = false;
        }
        public int AddProduce(string productName, int rawQuantity, int consumptionPerUnit)
        {
            if (!IsOperational)
            {
                Console.WriteLine($"Станок {Model} неисправен, производство невозможно.");
                return 0;
            }

            int producedUnits = rawQuantity / consumptionPerUnit;

            if (producedUnits > 0)
            {
                Console.WriteLine($"Станок {Model} произвёл {producedUnits} ед. продукта '{productName}'.");
            }
            else
            {
                Console.WriteLine($"Недостаточно сырья для производства хотя бы одной единицы '{productName}'.");
            }

            return producedUnits;
        }
    }
}
