using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryLib
{
    public class MaintenanceRecord
    {
        public int Id { get; }
        public int MachineId { get; }
        public DateTime Date { get; }
        public string Description { get; }
        public string Technician { get; }

        public MaintenanceRecord(int id, int machineId, DateTime date, string description, string technician)
        {
            Id = id;
            MachineId = machineId;
            Date = date;
            Description = description;
            Technician = technician;
        }

        public void Record()
        {
            Console.WriteLine($"Техобслуживание станка #{MachineId}: {Date:d} — {Description}. Техник: {Technician}");
        }
    }
}
