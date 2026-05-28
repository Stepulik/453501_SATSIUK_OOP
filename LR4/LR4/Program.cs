using System;
using System.Collections.Generic;

// Класс шруза, у него есть все о грузе -> information Expert, считает totalWeight
class CargoItem
{
    public string Name { get; set; }
    public double WeightPerUnit { get; set; }
    public double CostPerKg { get; set; }
    public int Quantity { get; set; }
    public double TotalWeight => WeightPerUnit * Quantity;
}

// Класс транспорта
class Transport
{
    public string Name { get; set; }
    public string Medium { get; set; }
    public double CostPerKm { get; set; }
    public double SpeedKmH { get; set; }
}

// Статическая фабрика для создания грузов по их имени.
// Pure Fabrication исскуственный класс которого нет в предметной области
// но он помогает создавать объекты и поддерживает high cohesion(только создание грузов)
static class CargoFactory
{
    private static Dictionary<string, (double weight, double costPerKg)> _catalog = new()
    {
        { "Электроника",             (1.5,   50.0)  },
        { "Одежда",                  (0.8,   20.0)  },
        { "Оборудование",            (120.0, 15.0)  },
        { "Скоропортящиеся продукты",(10.0,  100.0) }
    };

    // Creator он имеет данные для инициализации груза поэтому отвечает за создание объекта CargoItem
    public static CargoItem Create(string name, int quantity)
    {
        if (!_catalog.ContainsKey(name))
            throw new ArgumentException($"Груз '{name}' не найден.");
        var (weight, costPerKg) = _catalog[name];
        return new CargoItem { Name = name, WeightPerUnit = weight, CostPerKg = costPerKg, Quantity = quantity };
    }
}


// Интерфейс абстрактной фабрики, полиморфизм, т.к. разные реализации фабрик могут вести себя по-разному.
// Protected Variations интерфейс дает защиту от изменений в конкретных фаббриках
interface ILogisticsFactory
{
    Transport CreateTransport(string name);
    CargoItem CreateCargo(string name, int quantity);
}


// Конкретная фабрика для наземной логистики, реализует интерфейс абстрактной фабрики + Creator low Coupuling + High Cohesion
class LandLogisticsFactory : ILogisticsFactory
{
    private static Dictionary<string, (double cost, double speed)> _transports = new()
    {
        { "Грузовик", (15.0, 80.0) },
        { "Поезд",    (5.0,  60.0) }
    };

    public Transport CreateTransport(string name)
    {
        if (!_transports.ContainsKey(name))
            throw new ArgumentException($"Наземный транспорт '{name}' не найден.");
        var (cost, speed) = _transports[name];
        return new Transport { Name = name, Medium = "Земля", CostPerKm = cost, SpeedKmH = speed };
    }

    public CargoItem CreateCargo(string name, int quantity) => CargoFactory.Create(name, quantity);
}

class WaterLogisticsFactory : ILogisticsFactory
{
    private static Dictionary<string, (double cost, double speed)> _transports = new()
    {
        { "Танкер", (2.0, 35.0) }
    };

    public Transport CreateTransport(string name)
    {
        if (!_transports.ContainsKey(name))
            throw new ArgumentException($"Водный транспорт '{name}' не найден.");
        var (cost, speed) = _transports[name];
        return new Transport { Name = name, Medium = "Вода", CostPerKm = cost, SpeedKmH = speed };
    }

    public CargoItem CreateCargo(string name, int quantity) => CargoFactory.Create(name, quantity);
}

class AirLogisticsFactory : ILogisticsFactory
{
    private static Dictionary<string, (double cost, double speed)> _transports = new()
    {
        { "Самолет",  (150.0, 850.0) },
        { "Вертолет", (200.0, 250.0) }
    };

    public Transport CreateTransport(string name)
    {
        if (!_transports.ContainsKey(name))
            throw new ArgumentException($"Воздушный транспорт '{name}' не найден.");
        var (cost, speed) = _transports[name];
        return new Transport { Name = name, Medium = "Воздух", CostPerKm = cost, SpeedKmH = speed };
    }

    public CargoItem CreateCargo(string name, int quantity) => CargoFactory.Create(name, quantity);
}

// Pure Fabrication
class DeliveryCalculator
{
    //P = sum(ci * mi) + r * p_delivery
    public double CalculateCost(List<CargoItem> cargo, Transport transport, double distanceKm)
    {
        double cargoCost = 0;
        foreach (var item in cargo)
            cargoCost += item.CostPerKg * item.TotalWeight;
        return cargoCost + distanceKm * transport.CostPerKm;
    }

    public double CalculateTimeHours(Transport transport, double distanceKm)
        => distanceKm / transport.SpeedKmH;
}

//Controller
class LogisticsController
{
    private readonly DeliveryCalculator _calculator = new();

    public void ProcessDelivery(ILogisticsFactory factory, string transportName, List<(string cargoName, int qty)> cargoList, double distanceKm)
    {
        Transport transport = factory.CreateTransport(transportName);

        List<CargoItem> cargo = new();
        foreach (var (name, qty) in cargoList)
            cargo.Add(factory.CreateCargo(name, qty));

        double cost = _calculator.CalculateCost(cargo, transport, distanceKm);
        double hours = _calculator.CalculateTimeHours(transport, distanceKm);

        Console.WriteLine($"Транспорт  : {transport.Name} ({transport.Medium})");
        Console.WriteLine($"Расстояние : {distanceKm} км");
        Console.WriteLine("Состав партии:");
        foreach (var item in cargo)
            Console.WriteLine($"  - {item.Name}: {item.Quantity} шт. × {item.WeightPerUnit} кг = {item.TotalWeight} кг");
        Console.WriteLine($"Стоимость  : {cost:F2} у.е.");
        Console.WriteLine($"Время пути : {hours:F1} ч");
    }
}

class Program
{
    static void Main()
    {
        var controller = new LogisticsController();

        // Пример 1: Земля — Грузовик
        controller.ProcessDelivery(new LandLogisticsFactory(), "Грузовик", new() { ("Электроника", 10), ("Одежда", 50) }, 300);

        // Пример 2: Вода — Танкер
        controller.ProcessDelivery(new WaterLogisticsFactory(), "Танкер", new() { ("Оборудование", 5), ("Скоропортящиеся продукты", 20) }, 1500);

        // Пример 3: Воздух — Самолет
        controller.ProcessDelivery(new AirLogisticsFactory(), "Самолет", new() { ("Скоропортящиеся продукты", 3), ("Электроника", 5) }, 2000);
    }
}
