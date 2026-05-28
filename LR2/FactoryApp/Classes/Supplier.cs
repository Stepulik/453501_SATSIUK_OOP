using System;

namespace FactoryLib
{
    public class Supplier : IEntity
    {
        public int Id { get; }
        public string Name { get; }
        public string ContactPerson { get; private set; }
        public string Phone { get; private set; }
        public string Address { get; private set; }

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

        public string GetSupplierInfo()
        {
            return $"Поставщик: {Name} (ID: {Id}), контакт: {ContactPerson}, тел: {Phone}, адрес: {Address}";
        }
    }
}