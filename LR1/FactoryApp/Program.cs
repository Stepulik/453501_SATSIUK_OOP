using FactoryLib;
using System;
using System.Reflection.PortableExecutable;

namespace FactoryApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("\n");

            //Worker
            Console.WriteLine("Рабочие:");
            var worker1 = new Worker(1, "Иванов И.И.", "AB123456", "Сборщик", new DateTime(2020, 5, 10), 45000m);
            worker1.AddTask("Собрать узел А");
            worker1.AddTask("Проверить конвейер");
            worker1.FinishCurrentTask();
            worker1.AddTask("Упаковать детали");
            Console.WriteLine($"Список задач: {worker1.GetTaskList()}");
            worker1.FinishCurrentTask();
            worker1.FinishCurrentTask();
            Console.WriteLine();

            // Warehouse
            Console.WriteLine("--- Склад ---");
            var warehouse = new Warehouse(101, "Основной склад", "Цех №1");
            warehouse.AddMaterial("Сталь", 1000);
            warehouse.AddMaterial("Пластик", 500);
            warehouse.RemoveMaterial("Сталь", 200);
            warehouse.ShowStock();
            Console.WriteLine($"Количество пластика: {warehouse.GetQuantity("Пластик")}");
            Console.WriteLine();

            // Supplier
            Console.WriteLine("--- Поставщик ---");
            var supplier = new Supplier(201, "ООО 'МеталлСнаб'", "Петров П.П.", "+7(123)456-78-90", "г. Москва, ул. Ленина, 10");
            supplier.AddMaterial("Сталь", 120.50m);
            supplier.AddMaterial("Алюминий", 200.00m);
            supplier.SetPrice("Пластик", 80.30m);
            Console.WriteLine(supplier.GetSupplierInfo());
            Console.WriteLine("Материалы и цены:");
            Console.WriteLine(supplier.GetMaterialsList());
            supplier.CorrectSupplierInformation("Сидоров С.С.", "+375(44)654-32-10", "г. Минск, ул. Мира, 5");
            Console.WriteLine("После обновления контактов:");
            Console.WriteLine(supplier.GetSupplierInfo());
            Console.WriteLine();

            // RawMaterial
            Console.WriteLine("--- Сырьё ---");
            var steel = new RawMaterial(301, "Сталь листовая", "кг", 100.0m);
            steel.Display();
            Console.WriteLine();

            //Product
            Console.WriteLine("--- Продукт ---");
            var product = new Product(401, "Корпус прибора", 1500.0m, 10);
            product.DisplayInfo();
            product.Price = 1600.0m;
            product.Quantity = 15;
            product.DisplayInfo();
            Console.WriteLine();

            // QualityCheck
            Console.WriteLine("--- Контроль качества ---");
            var check = new QualityCheck(501, 401, DateTime.Today, true, "Иванов И.И.");
            check.Display();
            Console.WriteLine();

            // Order
            Console.WriteLine("--- Заказ ---");
            var order = new Order(601, "ООО 'Покупатель'", "Корпус прибора", 5, 2.5, DateTime.Now.AddDays(7));
            order.DisplayOrderInfo();
            order.FinishOrder();
            Console.WriteLine($"Заказ выполнен: {order.IsDue}");
            Console.WriteLine();

            // Machine
            Console.WriteLine("--- Станок ---");
            var machine = new FactoryLib.Machine(701, "Станок-1", true, DateTime.Today.AddDays(-30));
            int produced = machine.AddProduce("Корпус прибора", 100, 5); // 100 сырья, расход 5 на ед. -> 20 ед.
            Console.WriteLine($"Произведено единиц: {produced}");
            machine.SetNotOperational();
            machine.AddProduce("Корпус прибора", 50, 5); // не должен работать
            machine.SetOperational();
            Console.WriteLine($"Станок исправен: {machine.IsOperational}, дата последнего ТО: {machine.LastMaintenance:d}");
            Console.WriteLine();

            //  MaintenanceRecord
            Console.WriteLine("--- Техобслуживание ---");
            var maintenance = new MaintenanceRecord(801, 701, DateTime.Today, "Замена масла", "Сергеев А.А.");
            maintenance.Record();
            Console.WriteLine();

            //Department
            Console.WriteLine("--- Цех ---");
            var department = new Department(901, "Сборочный цех", "Сборка корпусов и узлов");
            department.GetInfo();
            Console.WriteLine();

            Console.WriteLine("=== Демонстрация завершена ===");
        }
    }
}