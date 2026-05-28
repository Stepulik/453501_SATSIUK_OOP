using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryLib
{
    public class QualityCheck
    {
        public int Id { get; }
        public int ProductId { get; }
        public DateTime Date { get; }
        public bool Result { get; }
        public string Inspector { get; }

        public QualityCheck(int id, int productId, DateTime date, bool result, string inspector)
        {
            Id = id;
            ProductId = productId;
            Date = date;
            Result = result;
            Inspector = inspector;
        }

        public void Display()
        {
            Console.WriteLine($"Проверка продукта #{ProductId}: {Date:d} — {(Result ? "Годен" : "Брак")}. Инспектор: {Inspector}");
        }
    }
}
