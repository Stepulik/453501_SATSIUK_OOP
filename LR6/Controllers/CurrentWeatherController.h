#pragma once
#include <memory>
#include <future>
#include <string>
#include <vector>
#include <stdexcept>
#include "../Clients/WeatherDataClient.h"
#include "../Clients/WeatherClientFactory.h"
#include "../Models/Weather/CurrentWeather.h"

namespace Forecast::Controllers {
    class CurrentWeatherController {
    private:
        std::shared_ptr<Clients::WeatherClientFactory> factory;
    public:
        explicit CurrentWeatherController(std::shared_ptr<Clients::WeatherClientFactory> factory_)
            : factory(std::move(factory_)) {}

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

        // Новый метод для прогноза
        std::future<std::vector<double>> GetForecast(double latitude, double longitude, int days, const std::string& provider) {
            auto client = factory->create(provider);
            if (!client) {
                throw std::runtime_error("Unknown provider: " + provider);
            }
            return client->GetForecast(latitude, longitude, days);
        }

		std::future<std::vector<Models::Weather::CurrentWeather>> GetMultipleCurrentWeather(
    	const std::vector<std::tuple<double, double, std::string>>& requests
		) {
    		return std::async(std::launch::async, [this, requests]() {
        		std::vector<Models::Weather::CurrentWeather> results;
        		for (const auto& req : requests) {
        		    double lat = std::get<0>(req);
        		    double lon = std::get<1>(req);
        		    std::string provider = std::get<2>(req);
        		    results.push_back(GetCurrentWeather(lat, lon, provider).get());
        		}
        		return results;
    		});
		}
    };
}