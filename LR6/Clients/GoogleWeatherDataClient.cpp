#include "GoogleWeatherDataClient.h"
#include "../Utils/ApiCallException.h"
#include <cpr/cpr.h>
#include <nlohmann/json.hpp>
#include <future>

namespace Forecast::Clients {
    GoogleWeatherDataClient::GoogleWeatherDataClient(const std::string& baseUrl, const std::string& apiKey)
        : baseUrl(baseUrl), apiKey(apiKey) {}

    std::future<double> GoogleWeatherDataClient::LocationCurrentTemperature(double latitude, double longitude) {
        return std::async(std::launch::async, [latitude, longitude]() {
            std::string url = "https://api.open-meteo.com/v1/forecast?latitude=" + std::to_string(latitude) +
                "&longitude=" + std::to_string(longitude) + "&current_weather=true";
            cpr::Response response = cpr::Get(cpr::Url{ url });
            if (response.status_code != 200) {
                throw Utils::ApiCallException("Google/Open-Meteo current weather error: " + std::to_string(response.status_code));
            }
            auto json = nlohmann::json::parse(response.text);
            return json["current_weather"]["temperature"].get<double>();
        });
    }

    std::future<std::vector<double>> GoogleWeatherDataClient::GetForecast(double latitude, double longitude, int days) {
        return std::async(std::launch::async, [latitude, longitude, days]() {
            std::string url = "https://api.open-meteo.com/v1/forecast?latitude=" + std::to_string(latitude) +
                "&longitude=" + std::to_string(longitude) +
                "&hourly=temperature_2m&forecast_days=" + std::to_string(days);
            cpr::Response response = cpr::Get(cpr::Url{ url });
            if (response.status_code != 200) {
                throw Utils::ApiCallException("Google/Open-Meteo forecast error: " + std::to_string(response.status_code));
            }
            auto json = nlohmann::json::parse(response.text);
            std::vector<double> temps = json["hourly"]["temperature_2m"].get<std::vector<double>>();
            return temps;
        });
    }
}