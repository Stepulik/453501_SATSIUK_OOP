#pragma once
#include <memory>
#include <future>
#include "../Clients/WeatherDataClient.h"
#include "../Models/Weather/CurrentWeather.h"

namespace Forecast::Controllers {
	class CurrentWeatherController {
	private:
		std::shared_ptr<Clients::IWeatherDataClient> client;

	public:
		explicit CurrentWeatherController(std::shared_ptr<Clients::IWeatherDataClient> client)
			: client(std::move(client)) {
		}

		std::future<Models::Weather::CurrentWeather> GetCurrentWeather(double latitude, double longitude) {
			return std::async(std::launch::async, [this, latitude, longitude]() {
				double temperature = client->LocationCurrentTemperature(latitude, longitude).get();
				return Models::Weather::CurrentWeather{ temperature };
				});
		}
	};
}