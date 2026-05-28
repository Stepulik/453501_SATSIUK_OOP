#pragma once
#include <vector>
#include <string>

// =========================================================
// Файл: models.h
// Описание: Модели данных системы.
// =========================================================

// Item - товар в заказе
struct Item {
  std::string ID;
  std::string Name;
  double Price;
};

// Address - адрес доставки
struct Address {
  std::string City;
  std::string Street;
  std::string Zip;
};

// Order - заказ
struct Order {
  std::string ID;
  std::vector<Item> Items;
  std::string Type; // "Standard", "Premium", "Budget", "International"
  std::string ClientEmail;
  std::string ManagerTelegramID;
  std::string DiscountCard; // <<---
  Address Destination;
};