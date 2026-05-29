#include <gtest/gtest.h>
#include <future>
#include "../Clients/WeatherDataClient.h"
#include "../Controllers/CurrentWeatherController.h"
#include "../Api/WeatherApi.h"
#include "../Utils/ApiCallException.h"

//  Мок-клиент для тестов 
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

//  Фикстура для тестов CurrentWeatherController 
class CurrentWeatherControllerTest : public ::testing::Test {
protected:
    std::shared_ptr<MockWeatherClient> mockOpenWeather;
    std::shared_ptr<Forecast::Controllers::CurrentWeatherController> controller;

    void SetUp() override {
        mockOpenWeather = std::make_shared<MockWeatherClient>();
        controller = std::make_shared<Forecast::Controllers::CurrentWeatherController>(mockOpenWeather);
    }
};

//  СТАРЫЕ ТЕСТЫ 

// Проверяет, что контроллер возвращает температуру, переданную от клиента (без изменений)
TEST_F(CurrentWeatherControllerTest, ReturnsCorrectTemperature) {
    mockOpenWeather->returnValue = 22.5;
    auto result = controller->GetCurrentWeather(52.0, 21.0).get();
    EXPECT_DOUBLE_EQ(result.temperature, 22.5);
}

// Проверяет, что если клиент выбрасывает ApiCallException, контроллер пробрасывает его дальше
TEST_F(CurrentWeatherControllerTest, PropagatesApiException) {
    mockOpenWeather->shouldThrow = true;
    EXPECT_THROW(
        controller->GetCurrentWeather(0.0, 0.0).get(),
        Forecast::Utils::ApiCallException
    );
}

// Проверяет, что контроллер работает с отрицательными координатами
TEST_F(CurrentWeatherControllerTest, NegativeCoordinatesWork) {
    mockOpenWeather->returnValue = -10.0;
    auto result = controller->GetCurrentWeather(-90.0, -180.0).get();
    EXPECT_DOUBLE_EQ(result.temperature, -10.0);
}

// Проверяет, что контроллер корректно обрабатывает нулевую температуру
TEST_F(CurrentWeatherControllerTest, ZeroTemperature) {
    mockOpenWeather->returnValue = 0.0;
    auto result = controller->GetCurrentWeather(0.0, 0.0).get();
    EXPECT_DOUBLE_EQ(result.temperature, 0.0);
}

//  НОВЫЕ ТЕСТЫ (дополнение для пограничных случаев) 

// Проверяет, что метод GetCurrentWeather с явным указанием провайдера "openweather" работает
TEST_F(CurrentWeatherControllerTest, ExplicitProviderOpenWeather) {
    mockOpenWeather->returnValue = 25.0;
    auto result = controller->GetCurrentWeather(52.0, 21.0, "openweather").get();
    EXPECT_DOUBLE_EQ(result.temperature, 25.0);
}

// Проверяет, что пока не реализована логика выбора провайдера, любой строковый provider принимается
// (возвращается температура от единственного клиента). В будущем это изменится.
TEST_F(CurrentWeatherControllerTest, AnyProviderIsAccepted) {
    mockOpenWeather->returnValue = 18.0;
    auto result = controller->GetCurrentWeather(52.0, 21.0, "unknown_provider").get();
    EXPECT_DOUBLE_EQ(result.temperature, 18.0);
}

// Проверяет, что при передаче экстремальных значений координат (очень большие числа) контроллер не падает
TEST_F(CurrentWeatherControllerTest, ExtremeCoordinates) {
    mockOpenWeather->returnValue = -99.9;
    auto result = controller->GetCurrentWeather(1e6, -1e6, "openweather").get();
    EXPECT_DOUBLE_EQ(result.temperature, -99.9);
}


// MAIN 
int main(int argc, char **argv) {
    ::testing::InitGoogleTest(&argc, argv);
    return RUN_ALL_TESTS();
}