#pragma once
#include "../Controllers/CurrentWeatherController.h"
#include <crow.h>
#include <memory>

namespace Forecast::Api {
    class WeatherApi {
    public:
        static void MapCurrentWeatherApi(crow::SimpleApp& app, std::shared_ptr<Controllers::CurrentWeatherController> controller);
        static void MapForecastApi(crow::SimpleApp& app, std::shared_ptr<Controllers::CurrentWeatherController> controller);
    private:
        static crow::response HandleGetCurrentWeather(
            std::shared_ptr<Controllers::CurrentWeatherController> controller,
            const crow::request& req
        );
        static crow::response HandleGetForecast(
            std::shared_ptr<Controllers::CurrentWeatherController> controller,
            const crow::request& req
        );
    };
}