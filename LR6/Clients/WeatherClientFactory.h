#pragma once
#include <memory>
#include <string>
#include <unordered_map>
#include <functional>
#include "WeatherDataClient.h"

namespace Forecast::Clients {
    class WeatherClientFactory {
    public:
        using Creator = std::function<std::shared_ptr<IWeatherDataClient>()>;

        void registerClient(const std::string& name, Creator creator) {
            creators[name] = creator;
        }

        std::shared_ptr<IWeatherDataClient> create(const std::string& name) const {
            auto it = creators.find(name);
            if (it != creators.end())
                return it->second();
            return nullptr;
        }

        static std::shared_ptr<WeatherClientFactory> defaultFactory();

    private:
        std::unordered_map<std::string, Creator> creators;
    };
}