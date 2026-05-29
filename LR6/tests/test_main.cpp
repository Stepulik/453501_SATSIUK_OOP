#include <gtest/gtest.h>
#include <future>
#include "../Clients/WeatherDataClient.h"
#include "../Controllers/CurrentWeatherController.h"
#include "../Utils/ApiCallException.h"

class MockWeatherClient : public Forecast::Clients::IWeatherDataClient {
public:
    double returnValue = 25.0;
    bool shouldThrow = false;
    std::string throwMessage = "mock error";

    std::future<double> LocationCurrentTemperature(double, double) override {
        return std::async(std::launch::async, [this]() -> double {
            if (shouldThrow)
                throw Forecast::Utils::ApiCallException(throwMessage);
            return returnValue;
        });
    }
};

TEST(CurrentWeatherControllerTest, ReturnsCorrectTemperature) {
    auto mock = std::make_shared<MockWeatherClient>();
    mock->returnValue = 22.5;
    Forecast::Controllers::CurrentWeatherController ctrl(mock);
    auto result = ctrl.GetCurrentWeather(52.0, 21.0).get();
    EXPECT_DOUBLE_EQ(result.temperature, 22.5);
}

TEST(CurrentWeatherControllerTest, PropagatesApiException) {
    auto mock = std::make_shared<MockWeatherClient>();
    mock->shouldThrow = true;
    Forecast::Controllers::CurrentWeatherController ctrl(mock);
    EXPECT_THROW(
        ctrl.GetCurrentWeather(0.0, 0.0).get(),
        Forecast::Utils::ApiCallException
    );
}

TEST(CurrentWeatherControllerTest, NegativeCoordinatesWork) {
    auto mock = std::make_shared<MockWeatherClient>();
    mock->returnValue = -10.0;
    Forecast::Controllers::CurrentWeatherController ctrl(mock);
    auto result = ctrl.GetCurrentWeather(-90.0, -180.0).get();
    EXPECT_DOUBLE_EQ(result.temperature, -10.0);
}

TEST(CurrentWeatherControllerTest, ZeroTemperature) {
    auto mock = std::make_shared<MockWeatherClient>();
    mock->returnValue = 0.0;
    Forecast::Controllers::CurrentWeatherController ctrl(mock);
    auto result = ctrl.GetCurrentWeather(0.0, 0.0).get();
    EXPECT_DOUBLE_EQ(result.temperature, 0.0);
}

int main(int argc, char **argv) {
    ::testing::InitGoogleTest(&argc, argv);
    return RUN_ALL_TESTS();
}