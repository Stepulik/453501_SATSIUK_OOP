using System;

namespace FactoryLib
{
    public class Order : IEntity
    {
        public int Id { get; }
        public string CustomerName { get; }
        public string ProductName { get; }
        public int Quantity { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsDue { get; private set; }

        public Order(int id, string customerName, string productName, int quantity, DateTime dueDate)
        {
            Id = id;
            CustomerName = customerName;
            ProductName = productName;
            Quantity = quantity;
            DueDate = dueDate;
            IsDue = false;
        }

        public void DisplayOrderInfo()
        {
            Console.WriteLine($"Заказ №{Id}: {CustomerName} — {ProductName} x {Quantity} до {DueDate:d}");
        }

        public void FinishOrder()
        {
            IsDue = true;
        }
    }
}