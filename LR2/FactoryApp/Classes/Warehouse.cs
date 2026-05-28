using System;
using System.Collections.Generic;

namespace FactoryLib
{
    public class Warehouse : IEntity
    {
        public int Id { get; }
        public string Name { get; }
        public string Location { get; }
        private List<Material> materials = new List<Material>();

        public Warehouse(int id, string name, string location)
        {
            Id = id;
            Name = name;
            Location = location;
        }

        public void AddMaterial(string name, int quantity)
        {
            var material = materials.Find(m => m.Name == name);
            if (material != null)
                material.Quantity += quantity;
            else
                materials.Add(new Material(name, quantity));
            Console.WriteLine($"Склад '{Name}': добавлено {quantity} ед. {name}. Текущий запас: {GetQuantity(name)}");
        }

        public bool RemoveMaterial(string name, int quantity)
        {
            var material = materials.Find(m => m.Name == name);
            if (material == null || material.Quantity < quantity)
                return false;
            material.Quantity -= quantity;
            Console.WriteLine($"Склад '{Name}': списано {quantity} ед. {name}. Остаток: {material.Quantity}");
            return true;
        }

        public int GetQuantity(string name)
        {
            var material = materials.Find(m => m.Name == name);
            return material?.Quantity ?? 0;
        }

        public void ShowStock()
        {
            Console.WriteLine($"Склад '{Name}':");
            foreach (var m in materials)
                Console.WriteLine($"  {m.Name}: {m.Quantity}");
        }
    }
}