#include <gtest/gtest.h>
#include <future>
#include <tuple>
#include "../Clients/WeatherDataClient.h"
#include "../Clients/WeatherClientFactory.h"
#include "../Controllers/CurrentWeatherController.h"
#include "../Utils/ApiCallException.h"

//  Мок-клиент
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

    // Новый метод для прогноза (пока нет в интерфейсе)
    // Пока закомментирован, чтобы тест упал на этапе компиляции
    /*
    std::future<std::vector<double>> GetForecast(double, double, int days) override {
        return std::async(std::launch::async, [this, days]() {
            return std::vector<double>(days, returnValue);
        });
    }
    */
};

//  Фикстура для тестов контроллера (без фабрики, пока старый стиль) 
class CurrentWeatherControllerTest : public ::testing::Test {
protected:
    std::shared_ptr<MockWeatherClient> mock;

    void SetUp() override {
        mock = std::make_shared<MockWeatherClient>();
    }
};

// СТАРЫЕ ТЕСТЫ (7 штук, зелёные)

TEST_F(CurrentWeatherControllerTest, ReturnsCorrectTemperature) {
    mock->returnValue = 22.5;
    Forecast::Controllers::CurrentWeatherController ctrl(mock);
    auto result = ctrl.GetCurrentWeather(52.0, 21.0).get();
    EXPECT_DOUBLE_EQ(result.temperature, 22.5);
}

TEST_F(CurrentWeatherControllerTest, PropagatesApiException) {
    mock->shouldThrow = true;
    Forecast::Controllers::CurrentWeatherController ctrl(mock);
    EXPECT_THROW(
        ctrl.GetCurrentWeather(0.0, 0.0).get(),
        Forecast::Utils::ApiCallException
    );
}

TEST_F(CurrentWeatherControllerTest, NegativeCoordinatesWork) {
    mock->returnValue = -10.0;
    Forecast::Controllers::CurrentWeatherController ctrl(mock);
    auto result = ctrl.GetCurrentWeather(-90.0, -180.0).get();
    EXPECT_DOUBLE_EQ(result.temperature, -10.0);
}

TEST_F(CurrentWeatherControllerTest, ZeroTemperature) {
    mock->returnValue = 0.0;
    Forecast::Controllers::CurrentWeatherController ctrl(mock);
    auto result = ctrl.GetCurrentWeather(0.0, 0.0).get();
    EXPECT_DOUBLE_EQ(result.temperature, 0.0);
}

TEST_F(CurrentWeatherControllerTest, ExplicitProviderOpenWeather) {
    mock->returnValue = 25.0;
    Forecast::Controllers::CurrentWeatherController ctrl(mock);
    auto result = ctrl.GetCurrentWeather(52.0, 21.0, "openweather").get();
    EXPECT_DOUBLE_EQ(result.temperature, 25.0);
}

TEST_F(CurrentWeatherControllerTest, UnknownProviderThrowsException) {
    mock->returnValue = 18.0;
    Forecast::Controllers::CurrentWeatherController ctrl(mock);
    EXPECT_THROW(
        ctrl.GetCurrentWeather(52.0, 21.0, "unknown_provider").get(),
        std::runtime_error
    );
}

TEST_F(CurrentWeatherControllerTest, ExtremeCoordinates) {
    mock->returnValue = -99.9;
    Forecast::Controllers::CurrentWeatherController ctrl(mock);
    auto result = ctrl.GetCurrentWeather(1e6, -1e6, "openweather").get();
    EXPECT_DOUBLE_EQ(result.temperature, -99.9);
}

// НОВЫЕ КРАСНЫЕ ТЕСТЫ (пока не компилируются/падают) 

// 1 Тест на фабрику и выбор провайдера (требует WeatherClientFactory)
TEST(FactoryProviderSelectionTest, ControllerUsesFactoryToSelectClient) {
    auto factory = std::make_shared<Forecast::Clients::WeatherClientFactory>();
    auto mockOpen = std::make_shared<MockWeatherClient>();
    auto mockGoogle = std::make_shared<MockWeatherClient>();
    mockOpen->returnValue = 15.0;
    mockGoogle->returnValue = 25.0;
    factory->registerClient("openweather", [mockOpen]() { return mockOpen; });
    factory->registerClient("google", [mockGoogle]() { return mockGoogle; });

    Forecast::Controllers::CurrentWeatherController ctrl(factory);
    auto tempOpen = ctrl.GetCurrentWeather(0, 0, "openweather").get().temperature;
    auto tempGoogle = ctrl.GetCurrentWeather(0, 0, "google").get().temperature;

    EXPECT_DOUBLE_EQ(tempOpen, 15.0);
    EXPECT_DOUBLE_EQ(tempGoogle, 25.0);
}

// 2 Тест для Google клиента (реальный вызов, потом заменим моком)
TEST(GoogleWeatherClientTest, DISABLED_ReturnsTemperatureForValidCoordinates) {
    SUCCEED();
}
// 3 Тест для прогноза погоды (требует ForecastController и метод GetForecast в клиенте)
TEST(ForecastTest, DISABLED_ReturnsVectorOfTemperatures) {
    SUCCEED();
}

// 4 Тест для batch-запроса (требует GetMultipleCurrentWeather в контроллере)
TEST(BatchWeatherTest, DISABLED_MultipleLocations) {
    SUCCEED();
}

// 5 Тест для геокодинга (требует GeocodingService)
TEST(GeocodingTest, DISABLED_ReturnsCoordinatesForCity) {
    SUCCEED();
}

// 6 Тест для получения погоды по названию города (требует перегрузки GetCurrentWeather(string city))
TEST(CurrentWeatherControllerTest, DISABLED_GetWeatherByCityName) {
    SUCCEED();
}

int main(int argc, char **argv) {
    ::testing::InitGoogleTest(&argc, argv);
    return RUN_ALL_TESTS();
}