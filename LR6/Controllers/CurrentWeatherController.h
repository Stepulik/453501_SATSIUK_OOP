#pragma once
#include <memory>
#include <future>
#include <string>
#include <stdexcept>
#include "../Clients/WeatherDataClient.h"
#include "../Clients/WeatherClientFactory.h"
#include "../Models/Weather/CurrentWeather.h"

namespace Forecast::Controllers {
    class CurrentWeatherController {
    private:
        std::shared_ptr<Clients::WeatherClientFactory> factory;
    public:
        // Новый конструктор (принимает фабрику)
        explicit CurrentWeatherController(std::shared_ptr<Clients::WeatherClientFactory> factory_)
            : factory(std::move(factory_)) {}

        // Старый конструктор (принимает одного клиента) – для обратной совместимости
        explicit CurrentWeatherController(std::shared_ptr<Clients::IWeatherDataClient> client) {
            factory = std::make_shared<Clients::WeatherClientFactory>();
            factory->registerClient("openweather", [client]() { return client; });
        }

        std::future<Models::Weather::CurrentWeather> GetCurrentWeather(double latitude, double longitude, const std::string& provider) {
            auto client = factory->create(provider);
            if (!client) {
                throw std::runtime_error("Unknown provider: " + provider);
            }
            return std::async(std::launch::async, [client, latitude, longitude]() {
                double temperature = client->LocationCurrentTemperature(latitude, longitude).get();
                return Models::Weather::CurrentWeather{ temperature };
            });
        }

        std::future<Models::Weather::CurrentWeather> GetCurrentWeather(double latitude, double longitude) {
            return GetCurrentWeather(latitude, longitude, "openweather");
        }
    };
}