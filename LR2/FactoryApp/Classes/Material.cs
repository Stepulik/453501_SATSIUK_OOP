namespace FactoryLib
{
    public class Material
    {
        public string Name { get; set; }
        public int Quantity { get; set; }

        public Material(string name, int quantity)
        {
            Name = name;
            Quantity = quantity;
        }
    }
}