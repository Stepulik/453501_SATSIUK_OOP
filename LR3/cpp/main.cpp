#include <iostream>
#include <vector>
#include "models.h"
#include "infrastructure.h"
#include "processor.h"
#include "staff.h"

// =========================================================
// Файл: main.cpp
// Описание: Точка входа в приложение.
// =========================================================

int main ()
{
    // 1. Создание заказа <<---
    Order order{
        "ORD-256-X",                                 // ID
        {{"1", "Thermal Clips", 1500},               // Items
         {"2", "UNATCO Pass Card", 50}},
        "Premium",                                   // Type
        "jeevacation@gmail.com",                     // ClientEmail
        "111111111",                                 // ManagerTelegramID
        "Gold",                                      // DiscountCard
        {"Agartha", "33 Thomas Street", "[REDACTED]"} // Destination
    };
    
    // 2. Инициализация процессора
    OrderProcessor processor;

    // 3. Обработка заказа
    try 
    {
        processor.Process(order);
        processor.Process(order);
    }
    catch(const std::runtime_error& e)
    {
        std::cout << "Failed to process order: " << e.what() << std::endl;
    }

    // 4. Работа с обслуживанием
    std::cout << "\nTesting Warehouse Stuff:";

    auto hm = new HumanManager{};
    auto rp = new RobotPacker{"George Droid"};
    // Собираем разные списки
    std::vector<IWorkable*> workers = {hm, rp};
    std::vector<IMeetingParticipant*> meetingers = {hm}; // только менеджер
    std::vector<IBreakable*> resters = {hm, rp};
    std::vector<ITimeWaster*> wasters = {hm};

    ManageWarehouse(workers, meetingers, resters, wasters);

    //5*. Корректное завершение программы 
    delete hm;
    delete rp;
    
    return 0;
}