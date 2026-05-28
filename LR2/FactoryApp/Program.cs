using System;
using FactoryLib;

namespace FactoryApp
{
    class Program
    {
        static void Main()
        {
            // 1 НАСЛЕДОВАНИЕ + полиморфизм подтипов - виртуальные методы
            Person workerPerson = new Worker(1, "Иван Петров", "Сборщик", 50000);
            Person managerPerson = new Manager(2, "Ольга Смирнова", "Сборочный цех", 10);

            workerPerson.ShowInfo();
            managerPerson.ShowInfo();

            //2 КОМПОЗИЦИЯ
            var dept = new Department(1, "Сборочный цех", "Сборка готовой продукции");
            var warehouse = new Warehouse(1, "Основной склад", "Корпус А");
            dept.Warehouse = warehouse;

            Worker worker = workerPerson as Worker;
            Manager manager = managerPerson as Manager;
            dept.AddEmployee(worker);   // перегрузка 
            dept.AddEmployee(manager);   // перегрузка

            var machine1 = new Machine(1, "Токарный CNC", true, new DateTime(2025, 1, 20));
            var machine2 = new Machine(2, "Фрезерный FM-100", false, new DateTime(2024, 12, 1));
            dept.AddMachine(machine1);
            dept.AddMachine(machine2);

            //3 ПОЛИМОРФИЗМ ПОДТИПОВ через интерфейс IEmployable
            dept.StartWorkDay();

            // 4 Работа со складом
            warehouse.AddMaterial("Стальной лист", 1000);
            warehouse.AddMaterial("Краска", 50);
            warehouse.ShowStock();

            //5 Взаимодействие станка и склада
            if (warehouse.RemoveMaterial("Стальной лист", 50))
            {
                int produced = machine1.Produce("Металлическая балка", 50, 5);
                if (produced > 0)
                {
                    var product = new Product(1, "Металлическая балка", 2000m, produced);
                    product.DisplayInfo();

                    var qualityCheck = new QualityCheck(1, product.Id, DateTime.Today, true, "Ольга Смирнова");
                    qualityCheck.Display();
                }
            }

            //6 Проверка станков
            dept.CheckMachines();

            //7 ПАРАМЕТРИЧЕСКИЙ ПОЛИМОРФИЗМ T
            Repository<IEntity> repo = new Repository<IEntity>();
            repo.Add(worker);
            repo.Add(manager);
            repo.Add(machine1);
            repo.Add(warehouse);

            Console.WriteLine("\nВсе сущности в репозитории:");
            foreach (var entity in repo.GetAll())
                Console.WriteLine($"  {entity.GetType().Name} с ID {entity.Id}");

            //8 Worker
            worker.AssignTask("Собрать узел А");
            worker.PerformDuties();
            worker.CompleteTask();

            //9 Supplier
            var supplier = new Supplier(1, "ООО МеталлТорг", "Петров П.П.", "+375(44)456-78-90", "г. Минск, ул. Ленина, 1");
            Console.WriteLine(supplier.GetSupplierInfo());
            supplier.CorrectSupplierInformation("Иванов И.И.", "+7(098)765-43-21", "г. Москва, ул. Новая, 5");
            Console.WriteLine("После обновления: " + supplier.GetSupplierInfo());

            Console.WriteLine("\nИнформация о цехе:");
            dept.GetInfo();
        }
    }
}