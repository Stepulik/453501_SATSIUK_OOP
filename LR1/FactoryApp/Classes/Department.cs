using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryLib
{
    public class Department
    {
        public int Id { get; }
        public string Name { get; }
        public string Description { get; }

        public Department(int id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
        }
        public void GetInfo()
        {
            Console.WriteLine($"Цех: {Name}, ID: {Id}, Описание: {Description}.");
        }
    }
}
