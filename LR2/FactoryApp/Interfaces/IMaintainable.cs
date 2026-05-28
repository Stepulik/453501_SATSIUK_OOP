using System;

namespace FactoryLib
{
    public interface IMaintainable
    {
        bool NeedsMaintenance { get; }
        void PerformMaintenance(string description, string technician);
    }
}