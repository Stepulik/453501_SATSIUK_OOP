using System;

namespace FactoryLib
{
    public class Manager : Person, IEmployable
    {
        public string DepartmentName { get; set; }
        public int TeamSize { get; set; }

        public Manager(int id, string fullName, string departmentName, int teamSize)
            : base(id, fullName)
        {
            DepartmentName = departmentName;
            TeamSize = teamSize;
        }

        public void PerformDuties()
        {
            Console.WriteLine($"{FullName} (менеджер) планирует работу отдела {DepartmentName}, руководит {TeamSize} сотрудниками.");
        }

        public override void ShowInfo()
        {
            Console.WriteLine($"Менеджер: {FullName}, Отдел: {DepartmentName}, Команда: {TeamSize} чел.");
        }
    }
}