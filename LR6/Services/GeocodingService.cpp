#include "GeocodingService.h"
#include <unordered_map>
#include <algorithm>

namespace Forecast::Services {
    std::pair<double, double> GeocodingService::GetCoordinates(const std::string& city) const {
        static const std::unordered_map<std::string, std::pair<double, double>> cityMap = {
            {"minsk",    {53.9045,  27.5615}},
            {"london",   {51.5074,  -0.1278}},
            {"tokyo",    {35.6895, 139.6917}},
            {"shanghai", {31.2304, 121.4737}},
            {"warsaw",   {52.2297,  21.0122}}
        };
        std::string key = city;
        std::transform(key.begin(), key.end(), key.begin(), ::tolower);
        auto it = cityMap.find(key);
        if (it == cityMap.end())
            throw std::runtime_error("City not found");
        return it->second;
    }
}