#include "OpenWeatherDataClient.h"
#include "../Utils/ApiCallException.h"
#include <cpr/cpr.h>
#include <nlohmann/json.hpp>
#include <stdexcept>
#include <vector>

namespace Forecast::Clients {

    OpenWeatherDataClient::OpenWeatherDataClient(const std::string& baseUrl, const std::string& apiKey)
        : baseUrl(baseUrl), apiKey(apiKey) {}

    std::future<double> OpenWeatherDataClient::LocationCurrentTemperature(double latitude, double longitude) {
        return std::async(std::launch::async, [this, latitude, longitude]() {
            std::string url = baseUrl + "?lat=" + std::to_string(latitude) +
                "&lon=" + std::to_string(longitude) +
                "&appid=" + apiKey + "&units=metric";
            cpr::Response response = cpr::Get(cpr::Url{ url });
            if (response.status_code != 200) {
                throw Utils::ApiCallException("OpenWeather current weather error: " + std::to_string(response.status_code));
            }
            auto data = nlohmann::json::parse(response.text);
            return data["main"]["temp"].get<double>();
        });
    }

    std::future<std::vector<double>> OpenWeatherDataClient::GetForecast(double latitude, double longitude, int days) {
        return std::async(std::launch::async, [this, latitude, longitude, days]() {
            std::string url = "https://api.openweathermap.org/data/2.5/forecast?lat=" + std::to_string(latitude) +
                "&lon=" + std::to_string(longitude) + "&appid=" + apiKey + "&units=metric";
            cpr::Response response = cpr::Get(cpr::Url{ url });
            if (response.status_code != 200) {
                throw Utils::ApiCallException("OpenWeather forecast error: " + std::to_string(response.status_code));
            }
            auto json = nlohmann::json::parse(response.text);
            std::vector<double> temps;
            // OpenWeather возвращает 40 записей (каждые 3 часа). Ограничим days * 8 (8 записей в день)
            int max_items = std::min(days * 8, 40);
            for (int i = 0; i < max_items; ++i) {
                temps.push_back(json["list"][i]["main"]["temp"].get<double>());
            }
            return temps;
        });
    }
}