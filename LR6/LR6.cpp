#include "crow.h"
#include "Clients/WeatherClientFactory.h"
#include "Controllers/CurrentWeatherController.h"
#include "Api/WeatherApi.h"
#include <memory>
#include <sstream>
#include <fstream>

int main() {
    crow::SimpleApp app;

    auto factory = Forecast::Clients::WeatherClientFactory::defaultFactory();
    auto weatherController = std::make_shared<Forecast::Controllers::CurrentWeatherController>(factory);

    Forecast::Api::WeatherApi::MapCurrentWeatherApi(app, weatherController);
    Forecast::Api::WeatherApi::MapForecastApi(app, weatherController);

    CROW_ROUTE(app, "/swagger/v1/swagger.json")
    ([] {
        std::ifstream ifs("openApi.json");
        std::stringstream buffer;
        buffer << ifs.rdbuf();
        return crow::response(200, "application/json", buffer.str());
    });

    CROW_ROUTE(app, "/swagger")
    ([] {
        std::ifstream ifs("index.html");
        std::stringstream buffer;
        buffer << ifs.rdbuf();
        return crow::response(200, "text/html", buffer.str());
    });

    app.port(18080).multithreaded().run();
    return 0;
}