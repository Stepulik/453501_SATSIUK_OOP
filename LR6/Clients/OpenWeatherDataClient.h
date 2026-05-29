#pragma once
#include "WeatherDataClient.h"
#include <string>

namespace Forecast::Clients {
    class OpenWeatherDataClient : public IWeatherDataClient {
    private:
        std::string baseUrl;
        std::string apiKey;

    public:
        OpenWeatherDataClient(const std::string& baseUrl, const std::string& apiKey);
        std::future<double> LocationCurrentTemperature(double latitude, double longitude) override;
    };
}