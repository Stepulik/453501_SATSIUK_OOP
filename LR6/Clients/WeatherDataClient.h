#pragma once
#include <future>

namespace Forecast::Clients {
    class IWeatherDataClient {
    public:
        virtual ~IWeatherDataClient() = default;
        virtual std::future<double> LocationCurrentTemperature(double latitude, double longitude) = 0;
    };
}