#pragma once
#include <string>
#include <utility>
#include <stdexcept>

namespace Forecast::Services {
    class GeocodingService {
    public:
        std::pair<double, double> GetCoordinates(const std::string& city) const;
    };
}