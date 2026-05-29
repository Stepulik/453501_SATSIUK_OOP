#pragma once
#include "WeatherDataClient.h"
#include <string>

namespace Forecast::Clients {
    class GoogleWeatherDataClient : public IWeatherDataClient {
    public:
        GoogleWeatherDataClient(const std::string& baseUrl, const std::string& apiKey);
        std::future<double> LocationCurrentTemperature(double latitude, double longitude) override;
        std::future<std::vector<double>> GetForecast(double latitude, double longitude, int days) override;
    private:
        std::string baseUrl;
        std::string apiKey;
    };
}