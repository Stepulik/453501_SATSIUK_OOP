using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryLib
{
    public class Order
    {
        public int Id { get; }
        public string CustomerName { get; }
        public string ProductName { get; }
        public int Quantity { get; set; }
        public double ConsumptionPerUnit { get; set; } //коэффициент расхода сырья на ед. готового товара
        public DateTime DueDate { get; set; }
        public bool IsDue { get; private set; }

        public Order(int id, string customerName, string productName, int quantity, double consumptionPerUnit, DateTime dueDate)
        {
            Id = id;
            CustomerName = customerName;
            ProductName = productName;
            Quantity = quantity;
            ConsumptionPerUnit = consumptionPerUnit;
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
