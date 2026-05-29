#include "OpenWeatherDataClient.h"
#include "../Utils/ApiCallException.h"
#include <cpr/cpr.h>
#include <nlohmann/json.hpp>
#include <stdexcept>

namespace Forecast::Clients {

    // Implementation of WeatherDataClient
    OpenWeatherDataClient::OpenWeatherDataClient(const std::string& baseUrl, const std::string& apiKey)
        : baseUrl(baseUrl), apiKey(apiKey) {
    }

    std::future<double> OpenWeatherDataClient::LocationCurrentTemperature(double latitude, double longitude) {
        return std::async(std::launch::async, [this, latitude, longitude]() {
            std::string url = baseUrl + "?lat=" + std::to_string(latitude) +
                "&lon=" + std::to_string(longitude) +
                "&appid=" + apiKey + "&units=metric";

            cpr::Response response = cpr::Get(cpr::Url{ url });

            if (response.status_code != 200) {
                throw Utils::ApiCallException("openweather returned bad status: " + std::to_string(response.status_code));
            }

            try {
                auto data = nlohmann::json::parse(response.text);
                return data["main"]["temp"].get<double>();
            }
            catch (const nlohmann::json::exception& e) {
                throw Utils::ApiCallException(std::string("failed to decode response: ") + e.what());
            }
            });
    }
}