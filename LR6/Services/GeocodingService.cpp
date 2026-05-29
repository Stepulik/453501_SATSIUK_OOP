#include "GeocodingService.h"
#include <unordered_map>

namespace Forecast::Services {
    std::pair<double, double> GeocodingService::GetCoordinates(const std::string& city) const {
        static const std::unordered_map<std::string, std::pair<double, double>> cityMap = {
            {"Minsk", {53.9045, 27.5615}},
            {"London", {51.5074, -0.1278}},
            {"Tokyo", {35.6895, 139.6917}},
            {"Shanghai", {31.2304, 121.4737}},
            {"Warsaw", {52.2297, 21.0122}}
        };
        auto it = cityMap.find(city);
        if (it == cityMap.end())
            throw std::runtime_error("City not found");
        return it->second;
    }
}