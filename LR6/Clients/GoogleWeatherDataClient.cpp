#include "GoogleWeatherDataClient.h"
#include <future>

namespace Forecast::Clients {
    GoogleWeatherDataClient::GoogleWeatherDataClient(const std::string& baseUrl, const std::string& apiKey)
        : baseUrl(baseUrl), apiKey(apiKey) {}

    std::future<double> GoogleWeatherDataClient::LocationCurrentTemperature(double latitude, double longitude) {
        // Заглушка: возвращаем фиксированную температуру
        return std::async(std::launch::async, []() {
            return 20.0;
        });
    }
}