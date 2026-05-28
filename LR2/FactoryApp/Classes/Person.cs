using System;

namespace FactoryLib
{
    public abstract class Person : IEntity
    {
        public int Id { get; }
        public string FullName { get; set; }

        protected Person(int id, string fullName)
        {
            Id = id;
            FullName = fullName;
        }

        public virtual void ShowInfo()
        {
            Console.WriteLine($"Person: {FullName} (ID: {Id})");
        }
    }
}