using System;

namespace FactoryLib
{
    public class Product : IEntity
    {
        public int Id { get; }
        public string Name { get; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        public Product(int id, string name, decimal price, int quantity)
        {
            Id = id;
            Name = name;
            Price = price;
            Quantity = quantity;
        }

        public void DisplayInfo()
        {
            Console.WriteLine($"Продукт: {Name}, ID: {Id}, Цена: {Price:C}, Количество: {Quantity}");
        }
    }
}