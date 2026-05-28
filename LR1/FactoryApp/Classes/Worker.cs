using System;

namespace FactoryLib
{
    public class Worker
    {
        // Поля только для чтения (инкапсуляция: установлены один раз в конструкторе)
        public int Id { get; }
        public DateTime HireDate { get; }
        public int Experience { get; } // Рассчитывается один раз при создании

        // Свойства с открытыми геттерами и сеттерами (можно добавить валидацию при необходимости)
        public string FullName { get; set; }
        public string Passport { get; set; }
        public string Position { get; set; }
        public decimal Salary { get; set; }
        public string CurrentTask { get; private set; }
        private Queue<string> _taskQueue = new Queue<string>();
        public Worker(int id, string fullName, string passport, string position, DateTime hireDate, decimal salary)
        {
            Id = id;
            FullName = fullName;
            Passport = passport;
            Position = position;
            HireDate = hireDate;
            Experience = (DateTime.Today - hireDate).Days / 365;
            Salary = salary;
            CurrentTask = string.Empty; // Изначально задач нет
        }
        public void FinishCurrentTask()
        {
            if (!string.IsNullOrEmpty(CurrentTask))
            {
                Console.WriteLine($"Рабочий {FullName} завершил: {CurrentTask}.");
            }

            // Берём следующую задачу из очереди
            if (_taskQueue.Count > 0)
            {
                CurrentTask = _taskQueue.Dequeue();
                Console.WriteLine($"Рабочий {FullName} приступил к новой задаче: {CurrentTask}.");
            }
            else
            {
                CurrentTask = string.Empty;
                Console.WriteLine($"У рабочего {FullName} больше нет задач.");
            }
        }
        public void AddTask(string taskName)
        {
            if (string.IsNullOrWhiteSpace(taskName))
                throw new ArgumentException("Название задачи не может быть пустым.");

            _taskQueue.Enqueue(taskName);
            Console.WriteLine($"Задача \"{taskName}\" добавлена в список рабочего {FullName}.");

            // Если у рабочего нет текущей задачи – начинаем эту сразу
            if (string.IsNullOrEmpty(CurrentTask))
            {
                CurrentTask = _taskQueue.Dequeue();
                Console.WriteLine($"Рабочий {FullName} приступил к задаче: {CurrentTask}.");
            }
        }
        public string GetTaskList()
        {
            var allTasks = new List<string>();
            if (!string.IsNullOrEmpty(CurrentTask))
                allTasks.Add(CurrentTask);
            allTasks.AddRange(_taskQueue);

            return allTasks.Count > 0 ? string.Join(", ", allTasks) : "нет задач";
        }
    }
}