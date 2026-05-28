using System;
using System.Collections.Generic;

namespace FactoryLib
{
    public class Department : IEntity
    {
        public int Id { get; }
        public string Name { get; }
        public string Description { get; }

        private List<IEmployable> employees = new List<IEmployable>();
        private List<Machine> machines = new List<Machine>();

        public Warehouse Warehouse { get; set; }

        public Department(int id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
        }
        public void AddEmployee(Worker worker)
        {
            employees.Add(worker);
            worker.Department = this;
            Console.WriteLine($"Рабочий {worker.FullName} добавлен в отдел {Name}");
        }

        public void AddEmployee(Manager manager)
        {
            employees.Add(manager);
            Console.WriteLine($"Менеджер {manager.FullName} добавлен в отдел {Name}");
        }

        public void AddMachine(Machine machine)
        {
            machines.Add(machine);
            machine.Department = this;
            Console.WriteLine($"Станок {machine.Model} добавлен в отдел {Name}");
        }

        public void StartWorkDay()
        {
            Console.WriteLine($"Начинается рабочий день в отделе {Name}");
            foreach (var emp in employees)
                emp.PerformDuties();
        }

        public void CheckMachines()
        {
            Console.WriteLine($"Проверка станков в отделе {Name}:");
            foreach (var m in machines)
            {
                if (m.NeedsMaintenance)
                    Console.WriteLine($"  Станок {m.Model} требует обслуживания!");
                else
                    Console.WriteLine($"  Станок {m.Model} в порядке.");
            }
        }

        public void GetInfo()
        {
            Console.WriteLine($"Отдел: {Name} (ID: {Id}) - {Description}");
            Console.WriteLine($"  Сотрудников: {employees.Count}, Станков: {machines.Count}");
            if (Warehouse != null)
                Console.WriteLine($"  Привязан склад: {Warehouse.Name}");
        }
    }
}