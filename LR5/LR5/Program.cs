using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Xml.Serialization;

//  Базовые классы предметной области 

class CargoItem
{
    public string Name { get; set; }
    public double WeightPerUnit { get; set; }
    public double CostPerKg { get; set; }
    public int Quantity { get; set; }
    public double TotalWeight => WeightPerUnit * Quantity;
}

class Transport
{
    public string Name { get; set; }
    public string Medium { get; set; }
    public double CostPerKm { get; set; }
    public double SpeedKmH { get; set; }
}

// Фабрика грузов (GRASP: Pure Fabrication, Creator)
static class CargoFactory
{
    private static Dictionary<string, (double weight, double costPerKg)> _catalog = new();

    static CargoFactory()
    {
        // начальное наполнение
        _catalog["Электроника"] = (1.5, 50.0);
        _catalog["Одежда"] = (0.8, 20.0);
        _catalog["Оборудование"] = (120.0, 15.0);
        _catalog["Скоропортящиеся продукты"] = (10.0, 100.0);
    }

    public static CargoItem Create(string name, int quantity)
    {
        if (!_catalog.ContainsKey(name))
            throw new ArgumentException($"Груз '{name}' не найден.");
        var (weight, cost) = _catalog[name];
        return new CargoItem { Name = name, WeightPerUnit = weight, CostPerKg = cost, Quantity = quantity };
    }

    // Для импорта из внешних файлов
    public static void AddOrUpdate(string name, double weightPerUnit, double costPerKg)
    {
        _catalog[name] = (weightPerUnit, costPerKg);
    }
}

// Абстрактная фабрика для транспорта и грузов (GRASP: Polymorphism, Protected Variations)
interface ILogisticsFactory
{
    Transport CreateTransport(string name);
    IEnumerable<Transport> GetAllTransports();  // для получения всех доступных видов транспорта
}

class LandLogisticsFactory : ILogisticsFactory
{
    private static Dictionary<string, (double cost, double speed)> _transports = new()
    {
        ["Грузовик"] = (15.0, 80.0),
        ["Поезд"] = (5.0, 60.0)
    };

    public Transport CreateTransport(string name)
    {
        if (!_transports.ContainsKey(name))
            throw new ArgumentException($"Наземный транспорт '{name}' не найден.");
        var (cost, speed) = _transports[name];
        return new Transport { Name = name, Medium = "Земля", CostPerKm = cost, SpeedKmH = speed };
    }

    public IEnumerable<Transport> GetAllTransports()
    {
        foreach (var kv in _transports)
            yield return new Transport { Name = kv.Key, Medium = "Земля", CostPerKm = kv.Value.cost, SpeedKmH = kv.Value.speed };
    }

    public static void AddOrUpdateTransport(string name, double costPerKm, double speedKmH)
        => _transports[name] = (costPerKm, speedKmH);
}

class WaterLogisticsFactory : ILogisticsFactory
{
    private static Dictionary<string, (double cost, double speed)> _transports = new()
    {
        ["Танкер"] = (2.0, 35.0)
    };

    public Transport CreateTransport(string name)
    {
        if (!_transports.ContainsKey(name))
            throw new ArgumentException($"Водный транспорт '{name}' не найден.");
        var (cost, speed) = _transports[name];
        return new Transport { Name = name, Medium = "Вода", CostPerKm = cost, SpeedKmH = speed };
    }

    public IEnumerable<Transport> GetAllTransports()
    {
        foreach (var kv in _transports)
            yield return new Transport { Name = kv.Key, Medium = "Вода", CostPerKm = kv.Value.cost, SpeedKmH = kv.Value.speed };
    }

    public static void AddOrUpdateTransport(string name, double costPerKm, double speedKmH)
        => _transports[name] = (costPerKm, speedKmH);
}

class AirLogisticsFactory : ILogisticsFactory
{
    private static Dictionary<string, (double cost, double speed)> _transports = new()
    {
        ["Самолет"] = (150.0, 850.0),
        ["Вертолет"] = (200.0, 250.0)
    };

    public Transport CreateTransport(string name)
    {
        if (!_transports.ContainsKey(name))
            throw new ArgumentException($"Воздушный транспорт '{name}' не найден.");
        var (cost, speed) = _transports[name];
        return new Transport { Name = name, Medium = "Воздух", CostPerKm = cost, SpeedKmH = speed };
    }

    public IEnumerable<Transport> GetAllTransports()
    {
        foreach (var kv in _transports)
            yield return new Transport { Name = kv.Key, Medium = "Воздух", CostPerKm = kv.Value.cost, SpeedKmH = kv.Value.speed };
    }

    public static void AddOrUpdateTransport(string name, double costPerKm, double speedKmH)
        => _transports[name] = (costPerKm, speedKmH);
}

// Калькулятор доставки (GRASP: Information Expert, Low Coupling)
class DeliveryCalculator
{
    public double CalculateCost(List<CargoItem> cargo, Transport transport, double distanceKm)
    {
        double cargoCost = cargo.Sum(c => c.CostPerKg * c.TotalWeight);
        return cargoCost + distanceKm * transport.CostPerKm;
    }

    public double CalculateTimeHours(Transport transport, double distanceKm)
        => distanceKm / transport.SpeedKmH;
}

// Результат доставки для одного вида транспорта
class DeliveryResult
{
    public Transport Transport { get; set; }
    public List<CargoItem> Cargo { get; set; }
    public double DistanceKm { get; set; }
    public double TotalCost { get; set; }
    public double TotalHours { get; set; }

    public override string ToString() =>
        $"{Transport.Name} ({Transport.Medium}) | Цена: {TotalCost:F2} у.е. | Время: {TotalHours:F1} ч";
}

//  Поведенческие паттерны 

//  Стратегия для фильтрации 
interface IFilterStrategy
{
    bool IsMatch(DeliveryResult result);
}

class MaxCostFilter : IFilterStrategy
{
    private double _maxCost;
    public MaxCostFilter(double max) => _maxCost = max;
    public bool IsMatch(DeliveryResult r) => r.TotalCost <= _maxCost;
}

class MediumFilter : IFilterStrategy
{
    private string _medium;
    public MediumFilter(string medium) => _medium = medium;
    public bool IsMatch(DeliveryResult r) => r.Transport.Medium == _medium;
}

//  Стратегия для сортировки 
interface ISortStrategy
{
    IComparer<DeliveryResult> GetComparer();
}

class SortByNameStrategy : ISortStrategy
{
    public IComparer<DeliveryResult> GetComparer() => new NameComparer();
    private class NameComparer : IComparer<DeliveryResult>
    {
        public int Compare(DeliveryResult x, DeliveryResult y) => string.Compare(x.Transport.Name, y.Transport.Name);
    }
}

class SortByCostStrategy : ISortStrategy
{
    public IComparer<DeliveryResult> GetComparer() => new CostComparer();
    private class CostComparer : IComparer<DeliveryResult>
    {
        public int Compare(DeliveryResult x, DeliveryResult y) => x.TotalCost.CompareTo(y.TotalCost);
    }
}

class SortBySpeedStrategy : ISortStrategy
{
    public IComparer<DeliveryResult> GetComparer() => new SpeedComparer();
    private class SpeedComparer : IComparer<DeliveryResult>
    {
        public int Compare(DeliveryResult x, DeliveryResult y) => x.Transport.SpeedKmH.CompareTo(y.Transport.SpeedKmH);
    }
}

// Комбинированная сортировка (композиция стратегий)
class CompositeSortStrategy : ISortStrategy
{
    private List<ISortStrategy> _strategies = new();
    public void Add(ISortStrategy s) => _strategies.Add(s);
    public IComparer<DeliveryResult> GetComparer() => new CompositeComparer(_strategies);

    private class CompositeComparer : IComparer<DeliveryResult>
    {
        private List<IComparer<DeliveryResult>> _comparers;
        public CompositeComparer(List<ISortStrategy> strategies)
        {
            _comparers = strategies.Select(s => s.GetComparer()).ToList();
        }
        public int Compare(DeliveryResult x, DeliveryResult y)
        {
            foreach (var cmp in _comparers)
            {
                int res = cmp.Compare(x, y);
                if (res != 0) return res;
            }
            return 0;
        }
    }
}

//  Итератор - собственный итератор для списка результатов (паттерн Iterator) 
class DeliveryResultIterator : IEnumerator<DeliveryResult>
{
    private List<DeliveryResult> _list;
    private int _pos = -1;
    public DeliveryResultIterator(List<DeliveryResult> list) => _list = list;
    public DeliveryResult Current => _list[_pos];
    object IEnumerator.Current => Current;
    public bool MoveNext() { _pos++; return _pos < _list.Count; }
    public void Reset() => _pos = -1;
    public void Dispose() { }
}

//  Наблюдатель для событий экспорта 
interface IExportObserver
{
    void OnExportStarted(string fileName);
    void OnExportFinished(string fileName);
    void OnExportError(string fileName, Exception ex);
}

class ConsoleExportObserver : IExportObserver
{
    public void OnExportStarted(string fn) => Console.WriteLine($"[Наблюдатель] Экспорт в {fn} начат.");
    public void OnExportFinished(string fn) => Console.WriteLine($"[Наблюдатель] Экспорт в {fn} завершён.");
    public void OnExportError(string fn, Exception ex) => Console.WriteLine($"[Наблюдатель] Ошибка экспорта в {fn}: {ex.Message}");
}

//  Структурные паттерны: Декоратор, Адаптер, Фасад 

//  Декораторы для экспорта (IFileExporter) 
interface IFileExporter
{
    void Export(string filePath, List<DeliveryResult> results);
}

// Конкретный экспортер в JSON
class JsonExporter : IFileExporter
{
    private static readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public void Export(string path, List<DeliveryResult> results)
    {
        string json = JsonSerializer.Serialize(results, _options);
        File.WriteAllText(path, json, Encoding.UTF8);
    }
}

// Конкретный экспортер в CSV
class CsvExporter : IFileExporter
{
    public void Export(string path, List<DeliveryResult> results)
    {
        var sb = new StringBuilder();
        sb.AppendLine("TransportName,Medium,DistanceKm,TotalCost,TotalHours,CargoDetails");
        foreach (var r in results)
        {
            string cargoStr = string.Join(";", r.Cargo.Select(c => $"{c.Name}({c.Quantity})"));
            sb.AppendLine($"{r.Transport.Name},{r.Transport.Medium},{r.DistanceKm},{r.TotalCost:F2},{r.TotalHours:F1},{cargoStr}");
        }
        File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
    }
}

// Базовый декоратор
abstract class ExporterDecorator : IFileExporter
{
    protected IFileExporter _wrapped;
    public ExporterDecorator(IFileExporter wrapped) => _wrapped = wrapped;
    public abstract void Export(string path, List<DeliveryResult> results);
}

// Декоратор шифрования (простое XOR)
class EncryptionDecorator : ExporterDecorator
{
    private string _password;
    public EncryptionDecorator(IFileExporter wrapped, string pwd) : base(wrapped) => _password = pwd;
    public override void Export(string path, List<DeliveryResult> results)
    {
        string temp = Path.GetTempFileName();
        _wrapped.Export(temp, results);
        byte[] data = File.ReadAllBytes(temp);
        byte[] key = Encoding.UTF8.GetBytes(_password);
        for (int i = 0; i < data.Length; i++)
            data[i] ^= key[i % key.Length];
        File.WriteAllBytes(path, data);
        File.Delete(temp);
    }
}

// Декоратор сжатия (ZIP). Удаляет старый архив, если существует, чтобы избежать ошибок.
class ZipDecorator : ExporterDecorator
{
    public ZipDecorator(IFileExporter wrapped) : base(wrapped) { }
    public override void Export(string path, List<DeliveryResult> results)
    {
        string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        string tempFile = Path.Combine(tempDir, Path.GetFileName(path));
        _wrapped.Export(tempFile, results);
        string zipPath = path;
        if (!zipPath.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
            zipPath += ".zip";
        if (File.Exists(zipPath)) File.Delete(zipPath);  // перезапись
        ZipFile.CreateFromDirectory(tempDir, zipPath);
        Directory.Delete(tempDir, true);
        // Если ожидалось имя без .zip, переименовывать не будем – результат в .zip
    }
}

//  Адаптеры для импорта данных из разных форматов (JSON, CSV, XML) 
// Интерфейс адаптера
interface IDataImporter
{
    void ImportCargo(string filePath);
    void ImportTransports(string filePath);
}

// DTO для десериализации
[Serializable]
public class CargoDto
{
    public string Name { get; set; }
    public double WeightPerUnit { get; set; }
    public double CostPerKg { get; set; }
}

[Serializable]
public class TransportDto
{
    public string Name { get; set; }
    public string Medium { get; set; }
    public double CostPerKm { get; set; }
    public double SpeedKmH { get; set; }
}

class JsonImporter : IDataImporter
{
    public void ImportCargo(string path)
    {
        string json = File.ReadAllText(path, Encoding.UTF8);
        var items = JsonSerializer.Deserialize<List<CargoDto>>(json);
        foreach (var i in items)
            CargoFactory.AddOrUpdate(i.Name, i.WeightPerUnit, i.CostPerKg);
    }

    public void ImportTransports(string path)
    {
        string json = File.ReadAllText(path, Encoding.UTF8);
        var items = JsonSerializer.Deserialize<List<TransportDto>>(json);
        foreach (var t in items)
            AddTransportToFactory(t);
    }

    private void AddTransportToFactory(TransportDto t)
    {
        switch (t.Medium)
        {
            case "Земля": LandLogisticsFactory.AddOrUpdateTransport(t.Name, t.CostPerKm, t.SpeedKmH); break;
            case "Вода": WaterLogisticsFactory.AddOrUpdateTransport(t.Name, t.CostPerKm, t.SpeedKmH); break;
            case "Воздух": AirLogisticsFactory.AddOrUpdateTransport(t.Name, t.CostPerKm, t.SpeedKmH); break;
            default: throw new Exception($"Неизвестная среда: {t.Medium}");
        }
    }
}

class CsvImporter : IDataImporter
{
    public void ImportCargo(string path)
    {
        var lines = File.ReadAllLines(path, Encoding.UTF8);
        foreach (var line in lines.Skip(1)) // заголовок
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            var parts = line.Split(',');
            if (parts.Length >= 3)
            {
                string name = parts[0].Trim();
                double w = double.Parse(parts[1]);
                double c = double.Parse(parts[2]);
                CargoFactory.AddOrUpdate(name, w, c);
            }
        }
    }

    public void ImportTransports(string path)
    {
        var lines = File.ReadAllLines(path, Encoding.UTF8);
        foreach (var line in lines.Skip(1))
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            var parts = line.Split(',');
            if (parts.Length >= 4)
            {
                string name = parts[0].Trim();
                string medium = parts[1].Trim();
                double cost = double.Parse(parts[2]);
                double speed = double.Parse(parts[3]);
                switch (medium)
                {
                    case "Земля": LandLogisticsFactory.AddOrUpdateTransport(name, cost, speed); break;
                    case "Вода": WaterLogisticsFactory.AddOrUpdateTransport(name, cost, speed); break;
                    case "Воздух": AirLogisticsFactory.AddOrUpdateTransport(name, cost, speed); break;
                    default: throw new Exception($"Неизвестная среда: {medium}");
                }
            }
        }
    }
}

class XmlImporter : IDataImporter
{
    public void ImportCargo(string path)
    {
        XmlSerializer ser = new(typeof(List<CargoDto>));
        using FileStream fs = new(path, FileMode.Open);
        var items = (List<CargoDto>)ser.Deserialize(fs);
        foreach (var i in items)
            CargoFactory.AddOrUpdate(i.Name, i.WeightPerUnit, i.CostPerKg);
    }

    public void ImportTransports(string path)
    {
        XmlSerializer ser = new(typeof(List<TransportDto>));
        using FileStream fs = new(path, FileMode.Open);
        var items = (List<TransportDto>)ser.Deserialize(fs);
        foreach (var t in items)
        {
            switch (t.Medium)
            {
                case "Земля": LandLogisticsFactory.AddOrUpdateTransport(t.Name, t.CostPerKm, t.SpeedKmH); break;
                case "Вода": WaterLogisticsFactory.AddOrUpdateTransport(t.Name, t.CostPerKm, t.SpeedKmH); break;
                case "Воздух": AirLogisticsFactory.AddOrUpdateTransport(t.Name, t.CostPerKm, t.SpeedKmH); break;
                default: throw new Exception($"Неизвестная среда: {t.Medium}");
            }
        }
    }
}

//  Фасад для работы с внешними данными (импорт/экспорт с наблюдателями) 
class ExternalDataFacade
{
    private List<IExportObserver> _observers = new();

    public void Subscribe(IExportObserver o) => _observers.Add(o);
    public void Unsubscribe(IExportObserver o) => _observers.Remove(o);

    private void NotifyStarted(string fn) { foreach (var o in _observers) o.OnExportStarted(fn); }
    private void NotifyFinished(string fn) { foreach (var o in _observers) o.OnExportFinished(fn); }
    private void NotifyError(string fn, Exception ex) { foreach (var o in _observers) o.OnExportError(fn, ex); }

    // Экспорт с комбинацией декораторов
    public void ExportResults(List<DeliveryResult> results, string filePath, string format,
                              bool encrypt = false, bool zip = false, string password = null)
    {
        NotifyStarted(filePath);
        try
        {
            IFileExporter exporter = format.ToLower() switch
            {
                "json" => new JsonExporter(),
                "csv" => new CsvExporter(),
                _ => throw new ArgumentException("Формат должен быть json или csv")
            };
            if (encrypt)
            {
                if (string.IsNullOrEmpty(password))
                    throw new ArgumentException("Для шифрования нужен пароль");
                exporter = new EncryptionDecorator(exporter, password);
            }
            if (zip)
                exporter = new ZipDecorator(exporter);
            exporter.Export(filePath, results);
            NotifyFinished(filePath);
        }
        catch (Exception ex)
        {
            NotifyError(filePath, ex);
            throw;
        }
    }

    // Импорт данных через адаптер по типу файла
    public void ImportData(string filePath, string fileType, bool isCargo)
    {
        IDataImporter importer = fileType.ToLower() switch
        {
            "json" => new JsonImporter(),
            "csv" => new CsvImporter(),
            "xml" => new XmlImporter(),
            _ => throw new ArgumentException("Тип файла должен быть json, csv или xml")
        };
        if (isCargo)
            importer.ImportCargo(filePath);
        else
            importer.ImportTransports(filePath);
    }
}

//  Контроллер (GRASP: Controller) 
class LogisticsController
{
    private DeliveryCalculator _calculator = new();
    private List<ILogisticsFactory> _factories = new List<ILogisticsFactory>
    {
        new LandLogisticsFactory(),
        new WaterLogisticsFactory(),
        new AirLogisticsFactory()
    };

    // Если transportName == null, возвращаем все возможные варианты транспорта
    public List<DeliveryResult> ProcessDelivery(string transportName, List<(string cargoName, int qty)> cargoList, double distanceKm)
    {
        List<CargoItem> cargo = new();
        foreach (var (name, qty) in cargoList)
            cargo.Add(CargoFactory.Create(name, qty));

        if (!string.IsNullOrEmpty(transportName))
        {
            foreach (var factory in _factories)
            {
                try
                {
                    var t = factory.CreateTransport(transportName);
                    double cost = _calculator.CalculateCost(cargo, t, distanceKm);
                    double hours = _calculator.CalculateTimeHours(t, distanceKm);
                    return new List<DeliveryResult> {
                        new DeliveryResult { Transport = t, Cargo = cargo, DistanceKm = distanceKm, TotalCost = cost, TotalHours = hours }
                    };
                }
                catch { } // не нашлось в этой фабрике
            }
            throw new ArgumentException($"Транспорт '{transportName}' не найден");
        }
        else
        {
            List<DeliveryResult> results = new();
            foreach (var factory in _factories)
            {
                foreach (var t in factory.GetAllTransports())
                {
                    double cost = _calculator.CalculateCost(cargo, t, distanceKm);
                    double hours = _calculator.CalculateTimeHours(t, distanceKm);
                    results.Add(new DeliveryResult { Transport = t, Cargo = cargo, DistanceKm = distanceKm, TotalCost = cost, TotalHours = hours });
                }
            }
            return results;
        }
    }

    // Применение фильтра (стратегия)
    public List<DeliveryResult> Filter(List<DeliveryResult> results, IFilterStrategy filter)
        => results.Where(r => filter.IsMatch(r)).ToList();

    // Применение сортировки (стратегия)
    public List<DeliveryResult> Sort(List<DeliveryResult> results, ISortStrategy strategy)
    {
        var list = new List<DeliveryResult>(results);
        list.Sort(strategy.GetComparer());
        return list;
    }

    // Получить итератор для обхода результатов
    public DeliveryResultIterator GetIterator(List<DeliveryResult> results)
        => new DeliveryResultIterator(results);
}

//  Программа (демонстрация) 
class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.WriteLine(" Логистическая система: расширенная демонстрация \n");

        // 1. Настраиваем фасад и наблюдателей
        var facade = new ExternalDataFacade();
        var consoleObserver = new ConsoleExportObserver();
        facade.Subscribe(consoleObserver);
        // Можно подписать и второго наблюдателя, например, логирующего в файл (не реализован, но для идеи)
        // facade.Subscribe(new FileExportObserver("export.log"));

        // 2. Генерируем тестовые файлы, если их нет (перезаписываем для чистоты)
        GenerateSampleFiles();

        // 3. Импортируем данные из разных форматов
        Console.WriteLine("---> Импорт данных ");

        Console.WriteLine("Импорт грузов из JSON (cargo.json)");
        facade.ImportData("cargo.json", "json", isCargo: true);

        Console.WriteLine("Импорт транспорта из CSV (transports.csv)");
        facade.ImportData("transports.csv", "csv", isCargo: false);

        Console.WriteLine("Импорт транспорта из XML (transports.xml) - добавим ещё один вид (дирижабль)");
        // Для демонстрации импорта XML сначала создадим дополнительный файл с новым транспортом
        GenerateExtraXmlTransport();
        facade.ImportData("extra_transport.xml", "xml", isCargo: false);

        // 4. Формируем партию грузов и расстояние
        var cargoList = new List<(string, int)>
        {
            ("Электроника", 10),
            ("Скоропортящиеся продукты", 5),
            ("Оборудование", 1)   // добавили оборудование для наглядности
        };
        double distance = 500;

        var controller = new LogisticsController();

        // 5. Сценарий A: пользователь выбрал конкретный транспорт
        Console.WriteLine("\n---> Сценарий A: указан транспорт 'Поезд'");
        try
        {
            var trainResult = controller.ProcessDelivery("Поезд", cargoList, distance);
            foreach (var res in trainResult)
                Console.WriteLine(res);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }

        // 6. Сценарий B: транспорт не указан -> все возможные варианты
        Console.WriteLine("\n---> Сценарий B: все варианты транспорта (без фильтрации)");
        var allResults = controller.ProcessDelivery(null, cargoList, distance);
        foreach (var res in allResults)
            Console.WriteLine(res);

        // 7. Применяем фильтр по максимальной стоимости (оставляем только дешёвые варианты)
        Console.WriteLine("\n---> Фильтр: стоимость <= 12000 у.е.");
        var costFiltered = controller.Filter(allResults, new MaxCostFilter(12000));
        foreach (var res in costFiltered)
            Console.WriteLine(res);

        // 8. Фильтр по среде передвижения (только наземный)
        Console.WriteLine("\n---> Фильтр: только наземный транспорт");
        var landFiltered = controller.Filter(allResults, new MediumFilter("Земля"));
        foreach (var res in landFiltered)
            Console.WriteLine(res);

        // 9. Сортировка по стоимости (от дешёвых к дорогим)
        Console.WriteLine("\n---> Сортировка всех вариантов по стоимости");
        var sortedByCost = controller.Sort(allResults, new SortByCostStrategy());
        foreach (var res in sortedByCost)
            Console.WriteLine(res);

        // 10. Комбинированная сортировка: сначала по среде (Земля, Вода, Воздух), затем по скорости
        // Для этого нужно создать компаратор или стратегию, сортирующую по среде. Сделаем простую стратегию сортировки по среде.
        Console.WriteLine("\n---> Комбинированная сортировка: по среде, затем по скорости");
        var compositeSort = new CompositeSortStrategy();
        compositeSort.Add(new SortByMediumStrategy());   // нужно добавить этот класс (см. ниже)
        compositeSort.Add(new SortBySpeedStrategy());
        var sortedByMediumThenSpeed = controller.Sort(allResults, compositeSort);
        foreach (var res in sortedByMediumThenSpeed)
            Console.WriteLine(res);

        // 11. Демонстрация итератора (обходим отфильтрованный и отсортированный список)
        Console.WriteLine("\n---> Обход результатов через итератор (только наземные, отсортированные по стоимости)");
        var filteredAndSorted = controller.Sort(controller.Filter(allResults, new MediumFilter("Земля")), new SortByCostStrategy());
        var iterator = controller.GetIterator(filteredAndSorted);
        int idx = 1;
        while (iterator.MoveNext())
            Console.WriteLine($"{idx++}. {iterator.Current}");

        // 12. Экспорт результатов в разных форматах и с разными декораторами
        Console.WriteLine("\n---> Экспорт результатов");

        // 12.1 Простой экспорт в JSON
        facade.ExportResults(allResults, "output_all.json", "json", encrypt: false, zip: false);
        Console.WriteLine("Создан файл: output_all.json");

        // 12.2 Экспорт в CSV с шифрованием
        facade.ExportResults(sortedByCost, "output_encrypted.csv", "csv", encrypt: true, zip: false, password: "123");
        Console.WriteLine("Создан файл: output_encrypted.csv (зашифрован XOR с паролем '123')");

        // 12.3 Экспорт в JSON со сжатием (ZIP)
        facade.ExportResults(landFiltered, "output_land.zip", "json", encrypt: false, zip: true);
        Console.WriteLine("Создан архив: output_land.zip (внутри JSON)");

        // 12.4 Экспорт в CSV с шифрованием + сжатие (комбинация)
        facade.ExportResults(sortedByMediumThenSpeed, "output_combo.zip", "csv", encrypt: true, zip: true, password: "secret");
        Console.WriteLine("Создан архив: output_combo.zip (внутри зашифрованный CSV)");

        // 13. Дополнительно: попробуем экспортировать пустой список (должен создаться пустой файл)
        Console.WriteLine("\n---> Экспорт пустого списка (проверка граничного случая)");
        var emptyList = new List<DeliveryResult>();
        facade.ExportResults(emptyList, "empty.json", "json", encrypt: false, zip: false);

        Console.WriteLine("\n Демонстрация завершена ");
    }

    // Вспомогательная стратегия сортировки по среде (для комбинированной сортировки)
    class SortByMediumStrategy : ISortStrategy
    {
        public IComparer<DeliveryResult> GetComparer() => new MediumComparer();
        private class MediumComparer : IComparer<DeliveryResult>
        {
            // Порядок: Земля, Вода, Воздух
            private static Dictionary<string, int> order = new()
            {
                ["Земля"] = 1,
                ["Вода"] = 2,
                ["Воздух"] = 3
            };
            public int Compare(DeliveryResult x, DeliveryResult y)
            {
                int xOrder = order[x.Transport.Medium];
                int yOrder = order[y.Transport.Medium];
                return xOrder.CompareTo(yOrder);
            }
        }
    }

    static void GenerateSampleFiles()
    {
        // cargo.json (как было)
        if (!File.Exists("cargo.json"))
        {
            var list = new List<CargoDto>
            {
                new CargoDto { Name = "Электроника", WeightPerUnit = 1.5, CostPerKg = 50 },
                new CargoDto { Name = "Одежда", WeightPerUnit = 0.8, CostPerKg = 20 },
                new CargoDto { Name = "Оборудование", WeightPerUnit = 120, CostPerKg = 15 },
                new CargoDto { Name = "Скоропортящиеся продукты", WeightPerUnit = 10, CostPerKg = 100 }
            };
            string json = JsonSerializer.Serialize(list, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText("cargo.json", json, Encoding.UTF8);
        }

        // transports.csv (как было)
        if (!File.Exists("transports.csv"))
        {
            var csvLines = new List<string> { "Name,Medium,CostPerKm,SpeedKmH" };
            csvLines.Add("Грузовик,Земля,15,80");
            csvLines.Add("Поезд,Земля,5,60");
            csvLines.Add("Танкер,Вода,2,35");
            csvLines.Add("Самолет,Воздух,150,850");
            csvLines.Add("Вертолет,Воздух,200,250");
            File.WriteAllLines("transports.csv", csvLines, Encoding.UTF8);
        }

        // transports.xml (основной)
        if (!File.Exists("transports.xml"))
        {
            var transportList = new List<TransportDto>
            {
                new TransportDto { Name = "Грузовик", Medium = "Земля", CostPerKm = 15, SpeedKmH = 80 },
                new TransportDto { Name = "Поезд", Medium = "Земля", CostPerKm = 5, SpeedKmH = 60 },
                new TransportDto { Name = "Танкер", Medium = "Вода", CostPerKm = 2, SpeedKmH = 35 },
                new TransportDto { Name = "Самолет", Medium = "Воздух", CostPerKm = 150, SpeedKmH = 850 },
                new TransportDto { Name = "Вертолет", Medium = "Воздух", CostPerKm = 200, SpeedKmH = 250 }
            };
            XmlSerializer ser = new(typeof(List<TransportDto>));
            using var fs = new FileStream("transports.xml", FileMode.Create);
            ser.Serialize(fs, transportList);
        }
    }

    static void GenerateExtraXmlTransport()
    {
        // Создаём дополнительный XML файл с новым видом транспорта (дирижабль)
        var extraTransport = new List<TransportDto>
        {
            new TransportDto { Name = "Дирижабль", Medium = "Воздух", CostPerKm = 80, SpeedKmH = 120 }
        };
        XmlSerializer ser = new(typeof(List<TransportDto>));
        using var fs = new FileStream("extra_transport.xml", FileMode.Create);
        ser.Serialize(fs, extraTransport);
    }
}