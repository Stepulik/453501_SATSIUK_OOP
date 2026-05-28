using System;

namespace FactoryLib
{
    public class Worker : Person, IEmployable
    {
        public string Position { get; set; }
        public decimal Salary { get; set; }
        public string CurrentTask { get; private set; }
        public Department Department { get; set; }

        public Worker(int id, string fullName, string position, decimal salary)
            : base(id, fullName)
        {
            Position = position;
            Salary = salary;
            CurrentTask = string.Empty;
        }

        public void AssignTask(string task)
        {
            CurrentTask = task;
            Console.WriteLine($"Рабочему {FullName} назначена задача: {task}");
        }

        public void CompleteTask()
        {
            Console.WriteLine($"Рабочий {FullName} завершил задачу: {CurrentTask}");
            CurrentTask = string.Empty;
        }

        public void PerformDuties()
        {
            if (!string.IsNullOrEmpty(CurrentTask))
                Console.WriteLine($"{FullName} выполняет задачу: {CurrentTask}");
            else
                Console.WriteLine($"{FullName} ожидает задачи.");
        }

        public override void ShowInfo()
        {
            Console.WriteLine($"Рабочий: {FullName}, Должность: {Position}, Зарплата: {Salary}, Отдел: {Department?.Name}");
        }
    }
}