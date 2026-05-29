#pragma once
#include <future>
#include <vector>

namespace Forecast::Clients {
    class IWeatherDataClient {
    public:
        virtual ~IWeatherDataClient() = default;
        virtual std::future<double> LocationCurrentTemperature(double latitude, double longitude) = 0;
        virtual std::future<std::vector<double>> GetForecast(double latitude, double longitude, int days) = 0;
    };
}