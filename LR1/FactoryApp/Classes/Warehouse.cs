namespace FactoryLib
{
    public class Warehouse
    {
        public int Id { get; }
        public string Name { get; }
        public string Location { get; }
        private Dictionary<string, int> stock = new Dictionary<string, int>(); // название сырья и его количество

        public Warehouse(int id, string name, string location)
        {
            Id = id;
            Name = name;
            Location = location;
        }

        public void AddMaterial(string materialName, int quantity)
        {
            if (stock.ContainsKey(materialName))
                stock[materialName] += quantity;
            else
                stock[materialName] = quantity;
            Console.WriteLine($"Склад '{Name}': добавлено {quantity} ед. {materialName}. Текущий запас: {stock[materialName]}");
        }

        public bool RemoveMaterial(string materialName, int quantity)
        {
            if (!stock.ContainsKey(materialName) || stock[materialName] < quantity)
                return false;
            stock[materialName] -= quantity;
            Console.WriteLine($"Склад '{Name}': списано {quantity} ед. {materialName}. Остаток: {stock[materialName]}");
            return true;
        }

        public int GetQuantity(string materialName)
        {
            return stock.ContainsKey(materialName) ? stock[materialName] : 0;
        }

        public void ShowStock()
        {
            Console.WriteLine($"Состояние склада '{Name}':");
            foreach (var item in stock)
                Console.WriteLine($"  {item.Key}: {item.Value}");
        }
    }
}
