#include "WeatherClientFactory.h"
#include "OpenWeatherDataClient.h"
#include "GoogleWeatherDataClient.h"

namespace Forecast::Clients {
    std::shared_ptr<WeatherClientFactory> WeatherClientFactory::defaultFactory() {
        static auto factory = []{
            auto f = std::make_shared<WeatherClientFactory>();
            f->registerClient("openweather", []{
                return std::make_shared<OpenWeatherDataClient>(
                    "https://api.openweathermap.org/data/2.5/weather",
                    "e40bc1b8df5f4df35925e33c6d825248"
                );
            });
            f->registerClient("google", []{
                return std::make_shared<GoogleWeatherDataClient>("", "");
            });
            return f;
        }();
        return factory;
    }
}