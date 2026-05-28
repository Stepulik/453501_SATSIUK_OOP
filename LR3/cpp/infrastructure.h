#pragma once
#include "models.h"
#include <unordered_set>
#include <iostream>
#include <fstream>
#include <string>
#include <vector>
#include <thread>
#include <ctime>
#include <chrono>
#include <format>

// =========================================================
// Файл: infrastructure.h
// Описание: Имитация работы с БД и внешними сервисами.
// =========================================================

// Добавим интерфейс для базы данных чтобы следовать принципу DIP <<--
class IDatabaseClient {
public:
    virtual ~IDatabaseClient() = default;
    virtual void SaveOrder(const Order& order, double total) = 0;
};

// Добавим интерфейс для отправки уведомлений <<---
class IMessageSender {
public:
    virtual ~IMessageSender() = default;
    virtual void SendMessage(
        const std::string& recipient, 
        const std::string& subject, 
        const std::string& body) = 0;
};

// Реализация логов <<---
class FileLogger {
private:
    std::string LogFileName;

public:
    explicit FileLogger(const std::string& fileName = "app_log.txt")
        : LogFileName(fileName){}

    void Log(const std::string& message)  {
        std::ofstream file(LogFileName, std::ios::app);

        if (!file.is_open()) {
            std::cerr << "Failed to open log file: " << LogFileName << "\n";
            return;
        }
        else {
            auto now = std::chrono::system_clock::now();
            auto local_now = std::chrono::zoned_time{std::chrono::current_zone(), now};

            std::string time_str = std::format("{:%Y-%m-%dT%H:%M:%S}", local_now);
            std::string time = "[" + time_str + "]";

            file << time << " INFO: " << message << "\n";
        }
        file.close();
    }
};

// RandomSQLDatabase - имитация тяжелой базы данных
class RandomSQLDatabase : public IDatabaseClient { // <<---
private:
    std::string ConnectionString;
    FileLogger Logger; 

public:
    RandomSQLDatabase() : 
            ConnectionString("random://root:password@localhost:228/shop"), 
            Logger() {}

    // Сохранение заказа в "базу данных"
    void SaveOrder(const Order& order, double total) override {
        std::cout << "Connecting to RandomSQL at " << ConnectionString << "...\n";
        std::this_thread::sleep_for(std::chrono::milliseconds(500)); // Имитация задержки сети

        std::ofstream file("orders_db.txt", std::ios::app);
        if (!file.is_open())
        {
            throw std::runtime_error("Could not open orders_db.txt !");
        }

        auto now = std::chrono::system_clock::now();
        auto local_now = std::chrono::zoned_time{std::chrono::current_zone(), now};
        std::string time_str = std::format("{:%Y-%m-%dT%H:%M:%S}", local_now);

        std::string time = "[" + time_str + "]";
        std::string msg = "ID: " + order.ID + " | Type: " + order.Type + " | Total: " + std::to_string(total) + "\n";
        file << time << ' ' << msg; // <<---

        if (!file)
        {
            throw std::runtime_error("Could not write to a file !");
        }

        file.close();
        std::cout << "Order saved successfully." << std::endl;
        
        Logger.Log(msg); // <<---
        return;
    }
};

class CachedDatabaseRepository : public IDatabaseClient {
private:
    IDatabaseClient& realRepository;
    std::unordered_set<std::string> cache;   // храним ID заказов

public:
    explicit CachedDatabaseRepository(IDatabaseClient& repo) : realRepository(repo) {}

    void SaveOrder(const Order& order, double total) override {
        if (cache.find(order.ID) != cache.end()) {
            std::cout << "Заказ уже есть в кэше\n";
            return;
        }
        // Сохраняем в условную БД
        realRepository.SaveOrder(order, total);
        cache.insert(order.ID);
        std::cout << "Заказ добавлен в кэш\n";
    }
};


// SmtpMailer - имитация почтового сервиса
class SmtpMailer : public IMessageSender {
private: // <<---
    std::string Server;   
public:
    explicit SmtpMailer(const std::string &server) : Server(server) {}

    void SendMessage(const std::string& to, const std::string& subject, const std::string& body) override {
        std::cout << ">> Connecting to SMTP server " << Server << " ...\n";
        std::cout << ">> Sending EMAIL to "<< to 
                  << "\nSubject : " << subject 
                  << "\nBody : " << body << "\n";
    }
};

// Добавим уведомления для менеджера по телеграмму ---
class TelegramNotifier : public IMessageSender {
private:
    std::string BotToken;      // токен бота 
public:
    explicit TelegramNotifier(const std::string& token) : BotToken(token) {}

    void SendMessage(const std::string& chatId, const std::string& subject, const std::string& body) override {
        std::cout << ">> Using bot token: " << BotToken.substr(0, 6) << "***" << "...\n"
                  << ">> Sending Telegram message to chat ID: " << chatId
                  << "\nSubject: " << subject
                  << "\nBody: " << body << "\n";
    }
};

