#pragma once
#include <iostream>
#include <vector>
#include <string>

#include <stdexcept> // <-- добавлено для std::runtime_error
// =========================================================
// Файл: staff.h
// Описание: Система управления персоналом склада.
// =========================================================

class IWorkable {
public:
    virtual ~IWorkable() = default;
    virtual void ProcessOrder() = 0;
};

class IBreakable {
public:
    virtual ~IBreakable() = default;
    virtual void GetRest() = 0;
};

class IMeetingParticipant {
public:
    virtual ~IMeetingParticipant() = default;
    virtual void AttendMeeting() = 0;
};

class ITimeWaster {
public:
    virtual ~ITimeWaster() = default;
    virtual void SwingingTheLead() = 0;
};

// HumanManager - Человек
class HumanManager : public IWorkable, public IBreakable,
                     public IMeetingParticipant, public ITimeWaster {
public:
    void ProcessOrder() override {
        std::cout << "Manager is processing logic..." << std::endl;
    }
    void GetRest() override {
        std::cout << "Manager is taking a break..." << std::endl;
    }
    void AttendMeeting() override {
        std::cout << "Manager is boring at the meeting..." << std::endl;
    }
    void SwingingTheLead() override {
        std::cout << "Manager is watching reels..." << std::endl;
    }
};

// RobotPacker - Робот
class RobotPacker : public IWorkable, public IBreakable {
public:
    std::string Model;
    explicit RobotPacker(std::string model) : Model(std::move(model)) {}

    void ProcessOrder() override {
        std::cout << "Robot " << Model << " is packing boxes..." << std::endl;
    }
    void GetRest() override {
        std::cout << "Robot was taken for maintenance" << std::endl;
    }
    //void AttendMeeting() override {
      //  std::cout << "ERROR: Robot cannot attend meetings" << std::endl;
    //}

    //void SwingingTheLead() override {
    //    throw std::runtime_error("CRITICAL ERROR: Robot cannot waste our money (we hope so)");
    //}
};

// ManageWarehouse - функция, которая работает со списком работников <<---
void ManageWarehouse(const std::vector<IWorkable*>& workers,
                     const std::vector<IMeetingParticipant*>& meetingers,
                     const std::vector<IBreakable*>& resters,
                     const std::vector<ITimeWaster*>& wasters) {
    std::cout << "\n--- Warehouse Shift Started ---\n";
    for (auto w : workers) w->ProcessOrder();
    for (auto m : meetingers) m->AttendMeeting();
    for (auto r : resters) r->GetRest();
    for (auto t : wasters) t->SwingingTheLead();
}