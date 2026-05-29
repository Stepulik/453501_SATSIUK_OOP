#include "crow.h"
#include "Clients/OpenWeatherDataClient.h"
#include "Controllers/CurrentWeatherController.h"
#include "Api/WeatherApi.h"
#include <memory>
#include <sstream>
#include <fstream>
#include <string>
#define API_key "e40bc1b8df5f4df35925e33c6d825248"

int main() {
	crow::SimpleApp app;

	auto weatherClient = std::make_shared<Forecast::Clients::OpenWeatherDataClient>(
		"https://api.openweathermap.org/data/2.5/weather",
		API_key
	);
	auto weatherController = std::make_shared<Forecast::Controllers::CurrentWeatherController>(weatherClient);
	Forecast::Api::WeatherApi::MapCurrentWeatherApi(app, weatherController);
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