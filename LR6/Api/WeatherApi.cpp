#include "WeatherApi.h"
#include "../Shared/Responses/Status.h"
#include "../Shared/Responses/Success.h"
#include "../Utils/ApiCallException.h"
#include <nlohmann/json.hpp>
#include <string>

namespace Forecast::Api {

	void WeatherApi::MapCurrentWeatherApi(
		crow::SimpleApp& app,
		std::shared_ptr<Controllers::CurrentWeatherController> controller
	) {
		CROW_ROUTE(app, "/api/v1/weather").methods(crow::HTTPMethod::Get)
			([controller](const crow::request& req) {
			return HandleGetCurrentWeather(controller, req);
				});
	}

	crow::response WeatherApi::HandleGetCurrentWeather(
		std::shared_ptr<Controllers::CurrentWeatherController> controller,
		const crow::request& req
	) {
		std::string lat_str = req.url_params.get("lat") != nullptr ? req.url_params.get("lat") : "18.300231990440128";
		std::string lon_str = req.url_params.get("lon") != nullptr ? req.url_params.get("lon") : "-64.8251590359234";

		try {
			double latitude = std::stod(lat_str);
			double longitude = std::stod(lon_str);
			auto weather = controller->GetCurrentWeather(latitude, longitude).get();
			nlohmann::json data;
			data["temperature"] = weather.temperature;
			nlohmann::json response;
			response["code"] = 200;
			response["message"] = "success";
			response["data"] = data;
			return crow::response(200,"application/json" ,response.dump());
		}
		catch (const std::invalid_argument&) {
			nlohmann::json error = { {"code", 400}, {"message", "invalid coordinates"} };
			return crow::response(400, error.dump());
		}
		catch (const Utils::ApiCallException& e) {
			std::cerr << "CRITICAL ERROR: " << e.what() << std::endl;
			nlohmann::json err = { {"code", 500}, {"message", e.what()} };
			return crow::response(500, err.dump());
		}
		catch (const std::exception& e) {
			nlohmann::json error = { {"code", 500}, {"message", "internal server error"} };
			return crow::response(500, error.dump());
		}
	}
}