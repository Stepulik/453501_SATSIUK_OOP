using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryLib
{
    public class Supplier
    {
        public int Id { get; }
        public string Name { get; }
        public string ContactPerson { get; private set; }
        public string Phone { get; private set; }
        public string Address { get; private set; }
        private Dictionary<string, decimal> _materialPrices = new Dictionary<string, decimal>();

        public Supplier(int id, string name, string contactPerson, string phone, string address)
        {
            Id = id;
            Name = name;
            ContactPerson = contactPerson;
            Phone = phone;
            Address = address;
        }
        public void CorrectSupplierInformation(string contactPerson, string phone, string address)
        {
            ContactPerson = contactPerson;
            Phone = phone;
            Address = address;
        }
        public void AddMaterial(string materialName, decimal price)
        {
            _materialPrices.Add(materialName, price);
        }

        public void SetPrice(string materialName, decimal price)
        {
            _materialPrices[materialName] = price;
        }
        public bool RemoveMaterial(string materialName)
        {
            return _materialPrices.Remove(materialName);
        }
        public bool HasMaterial(string materialName)
        {
            return _materialPrices.ContainsKey(materialName);
        }
        public string GetMaterialsList()
        {
            if (_materialPrices.Count == 0)
                return "У поставщика нет материалов.";

            var lines = new List<string>();
            foreach (var kvp in _materialPrices)
            {
                lines.Add($"{kvp.Key}: {kvp.Value:C}");
            }
            return string.Join(Environment.NewLine, lines);
        }

        public string GetSupplierInfo()
        {
            return $"Поставщик: {Name} (ID: {Id}), контакт: {ContactPerson}, тел: {Phone}, адрес: {Address}";
        }
    }
}
