#pragma once
#include <iostream>
#include <stdexcept>
#include <string>
#include <format>
#include "infrastructure.h"

// =========================================================
// Файл: processor.h
// Описание: Основная бизнес-логика.
// =========================================================

class OrderProcessor {
private:
    RandomSQLDatabase db;
    CachedDatabaseRepository proxy;
    IDatabaseClient* database;
    IMessageSender* mailer;

public:
    OrderProcessor()
        : db()
        , proxy(db)          // прокси ссылается на realDb
        , database(&proxy)       // работаем через прокси
        , mailer(new SmtpMailer("smtp.google.com")) // конкретная реализация
    {}
    ~OrderProcessor() {
        //delete database;
        delete mailer;
    }

    void Process(Order order)
    {
        std::cout << "--- Processing Order " << order.ID << " ---\n";

        //1. Логика валидации
        if (order.Items.size() == 0)
        {
            throw std::runtime_error("order must have at leat one item");
        }
        if (order.Destination.City == "")
        {
            throw std::runtime_error("destination city is required");
        }

        //2. Логика расчёта суммы
        double total = 0;
        for (auto item : order.Items)
        {
            total += item.Price;
        }

        // 2.5. Добавлен расчет скидок по дисконтной карте <<---
        double current_discount;
        if (order.DiscountCard == "Gold"){
            current_discount = 0.15;
        }
        else if (order.DiscountCard == "Silver") {
            current_discount = 0.1;
        }
        else if (order.DiscountCard == "Newbie") {
            current_discount = 0;
        }
        else 
        {
            throw std::runtime_error("unknown discount card type");
        }


        //3. логика скидок и налогов
        if (order.Type == "Standard")
        {
            // Стандартный налог
            total -= total * current_discount; // <<---
            total *= 1.2;
        }
        else if (order.Type == "Premium")
        {
            // Скидка 10% + налог
            total -= total * (current_discount + 0.1); // <<--- суть изменения в том чтобы скидка 10% + 10% давала 20%
            total *= 1.2;                              // а не 19% если применить сразу одну а потом вторую
            //total = (total * current_price * 0.9) * 1.2;
        }
        else if (order.Type == "Budget")
        {
            if (order.Items.size() > 3)
            {
                std::cout << "Budget orders cannot have more than 3 items. Skipping.\n";
                return;
            }
        }
        else if (order.Type == "International")
        {
            total -= total * current_discount; // <<---
            total *= 1.5; // Таможенный сбор
            if (order.Destination.City == "Nowhere")
            {
                throw std::runtime_error("cannot ship to Nowhere");
            }
        }
        else 
        {
            throw std::runtime_error("unknown order type");
        }


        //4. Логика сохранения
        try
        {
            database->SaveOrder(order, total);
        }
        catch (const std::runtime_error& e)
        {
            std::cout << "database error: " << e.what() << std::endl;
        }

        //5. Логика уведомлений
        std::string emailBody = std::format("Your order {} is confirmed!\nTotal: {:.2f}.\n", order.ID, total);
        this->mailer->SendMessage(order.ClientEmail, "Order Confirmation", emailBody);
        this->mailer->SendMessage(order.ManagerTelegramID, "Order Confirmation", emailBody);
    }
};