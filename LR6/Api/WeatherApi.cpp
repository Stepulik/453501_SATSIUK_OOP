#include "WeatherApi.h"
#include "../Shared/Responses/Status.h"
#include "../Shared/Responses/Success.h"
#include "../Utils/ApiCallException.h"
#include <nlohmann/json.hpp>
#include <string>
#include <iostream>

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

    void WeatherApi::MapForecastApi(
        crow::SimpleApp& app,
        std::shared_ptr<Controllers::CurrentWeatherController> controller
    ) {
        CROW_ROUTE(app, "/api/v1/forecast").methods(crow::HTTPMethod::Get)
            ([controller](const crow::request& req) {
                return HandleGetForecast(controller, req);
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
            std::string provider = req.url_params.get("provider") != nullptr ? req.url_params.get("provider") : "openweather";

            auto weather = controller->GetCurrentWeather(latitude, longitude, provider).get();
            nlohmann::json data;
            data["temperature"] = weather.temperature;
            nlohmann::json response;
            response["code"] = 200;
            response["message"] = "success";
            response["data"] = data;
            return crow::response(200, "application/json", response.dump());
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

    crow::response WeatherApi::HandleGetForecast(
        std::shared_ptr<Controllers::CurrentWeatherController> controller,
        const crow::request& req
    ) {
        std::string lat_str = req.url_params.get("lat") != nullptr ? req.url_params.get("lat") : "18.300231990440128";
        std::string lon_str = req.url_params.get("lon") != nullptr ? req.url_params.get("lon") : "-64.8251590359234";
        std::string days_str = req.url_params.get("days") != nullptr ? req.url_params.get("days") : "5";
        std::string provider = req.url_params.get("provider") != nullptr ? req.url_params.get("provider") : "openweather";

        try {
            double latitude = std::stod(lat_str);
            double longitude = std::stod(lon_str);
            int days = std::stoi(days_str);
            if (days < 1) days = 1;
            if (days > 5) days = 5;  // ограничим
			auto forecast = controller->GetForecast(latitude, longitude, days, provider).get();
            nlohmann::json response;
            response["code"] = 200;
            response["message"] = "success";
            response["data"] = forecast;
            return crow::response(200, "application/json", response.dump());
        }
        catch (const std::invalid_argument&) {
            nlohmann::json error = { {"code", 400}, {"message", "invalid parameters"} };
            return crow::response(400, error.dump());
        }
        catch (const Utils::ApiCallException& e) {
            nlohmann::json err = { {"code", 500}, {"message", e.what()} };
            return crow::response(500, err.dump());
        }
        catch (const std::exception& e) {
            nlohmann::json error = { {"code", 500}, {"message", "internal server error"} };
            return crow::response(500, error.dump());
        }
    }

	void WeatherApi::MapBatchWeatherApi(crow::SimpleApp& app, std::shared_ptr<Controllers::CurrentWeatherController> controller) {
    CROW_ROUTE(app, "/api/v1/weather/batch").methods(crow::HTTPMethod::Post)
        ([controller](const crow::request& req) {
            return HandleBatchWeather(controller, req);
        });
}

	crow::response WeatherApi::HandleBatchWeather(
		std::shared_ptr<Controllers::CurrentWeatherController> controller,
		const crow::request& req
	) {
		try {
			auto json = nlohmann::json::parse(req.body);
			if (!json.contains("locations") || !json["locations"].is_array()) {
				return crow::response(400, R"({"code":400,"message":"Invalid request: 'locations' array expected"})");
			}
			std::vector<std::tuple<double, double, std::string>> requests;
			for (auto& item : json["locations"]) {
				double lat = item.value("lat", 0.0);
				double lon = item.value("lon", 0.0);
				std::string provider = item.value("provider", "openweather");
				requests.emplace_back(lat, lon, provider);
			}
			auto results = controller->GetMultipleCurrentWeather(requests).get();
			nlohmann::json response;
			response["code"] = 200;
			response["message"] = "success";
			nlohmann::json data = nlohmann::json::array();
			for (const auto& w : results) {
				data.push_back({{"temperature", w.temperature}});
			}
			response["data"] = data;
			return crow::response(200, response.dump());
		}
		catch (const nlohmann::json::parse_error&) {
			return crow::response(400, R"({"code":400,"message":"Invalid JSON"})");
		}
		catch (const std::exception& e) {
			return crow::response(500, R"({"code":500,"message":"Internal server error"})");
		}
	}
}