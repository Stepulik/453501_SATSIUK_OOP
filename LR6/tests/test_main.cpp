#include <gtest/gtest.h>
#include <future>
#include <tuple>
#include "../Services/GeocodingService.h"
#include "../Clients/WeatherDataClient.h"
#include "../Clients/WeatherClientFactory.h"
#include "../Controllers/CurrentWeatherController.h"
#include "../Utils/ApiCallException.h"

// ── Mock клиент ──────────────────────────────────────────────────────────────
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

    std::future<std::vector<double>> GetForecast(double, double, int days) override {
        return std::async(std::launch::async, [this, days]() {
            if (shouldThrow)
                throw Forecast::Utils::ApiCallException(throwMessage);
            return std::vector<double>(days, returnValue);
        });
    }
};

// ── Фикстура ─────────────────────────────────────────────────────────────────
class CurrentWeatherControllerTest : public ::testing::Test {
protected:
    std::shared_ptr<MockWeatherClient> mock;

    void SetUp() override {
        mock = std::make_shared<MockWeatherClient>();
    }
};

// ── CurrentWeatherController — базовые тесты ─────────────────────────────────

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

// ── WeatherClientFactory ──────────────────────────────────────────────────────

TEST(FactoryProviderSelectionTest, ControllerUsesFactoryToSelectClient) {
    auto factory = std::make_shared<Forecast::Clients::WeatherClientFactory>();
    auto mockOpen = std::make_shared<MockWeatherClient>();
    auto mockGoogle = std::make_shared<MockWeatherClient>();
    mockOpen->returnValue = 15.0;
    mockGoogle->returnValue = 25.0;
    factory->registerClient("openweather", [mockOpen]() { return mockOpen; });
    factory->registerClient("google", [mockGoogle]() { return mockGoogle; });
    Forecast::Controllers::CurrentWeatherController ctrl(factory);
    auto tempOpen   = ctrl.GetCurrentWeather(0, 0, "openweather").get().temperature;
    auto tempGoogle = ctrl.GetCurrentWeather(0, 0, "google").get().temperature;

    EXPECT_DOUBLE_EQ(tempOpen,   15.0);
    EXPECT_DOUBLE_EQ(tempGoogle, 25.0);
}

TEST(GoogleWeatherClientTest, DefaultFactoryCreatesGoogleClient) {
    auto factory = Forecast::Clients::WeatherClientFactory::defaultFactory();
    ASSERT_NE(factory, nullptr);
    auto client = factory->create("google");
    ASSERT_NE(client, nullptr);
}

TEST(GoogleWeatherClientTest, FactoryReturnsNullForUnknownProvider) {
    auto factory = Forecast::Clients::WeatherClientFactory::defaultFactory();
    auto client = factory->create("nonexistent");
    EXPECT_EQ(client, nullptr);
}

// ── Forecast ──────────────────────────────────────────────────────────────────

TEST(ForecastTest, ReturnsVectorOfTemperatures) {
    auto factory = std::make_shared<Forecast::Clients::WeatherClientFactory>();
    auto mock = std::make_shared<MockWeatherClient>();
    mock->returnValue = 20.0;
    factory->registerClient("openweather", [mock]() { return mock; });

    Forecast::Controllers::CurrentWeatherController ctrl(factory);
    auto forecast = ctrl.GetForecast(55.75, 37.62, 3, "openweather").get();
    EXPECT_EQ(forecast.size(), 3u);
    for (double t : forecast)
        EXPECT_DOUBLE_EQ(t, 20.0);
}

TEST(ForecastTest, ForecastThrowsOnClientError) {
    auto factory = std::make_shared<Forecast::Clients::WeatherClientFactory>();
    auto mock = std::make_shared<MockWeatherClient>();
    mock->shouldThrow = true;
    factory->registerClient("openweather", [mock]() { return mock; });

    Forecast::Controllers::CurrentWeatherController ctrl(factory);
    EXPECT_THROW(
        ctrl.GetForecast(55.75, 37.62, 3, "openweather").get(),
        Forecast::Utils::ApiCallException
    );
}

TEST(ForecastTest, ForecastMinOneDayReturnsData) {
    auto factory = std::make_shared<Forecast::Clients::WeatherClientFactory>();
    auto mock = std::make_shared<MockWeatherClient>();
    mock->returnValue = 5.0;
    factory->registerClient("openweather", [mock]() { return mock; });

    Forecast::Controllers::CurrentWeatherController ctrl(factory);
    auto forecast = ctrl.GetForecast(55.75, 37.62, 1, "openweather").get();
    EXPECT_FALSE(forecast.empty());
}

// ── Batch ─────────────────────────────────────────────────────────────────────

TEST(BatchWeatherTest, MultipleLocations) {
    auto factory = std::make_shared<Forecast::Clients::WeatherClientFactory>();
    auto mockOpen   = std::make_shared<MockWeatherClient>();
    auto mockGoogle = std::make_shared<MockWeatherClient>();
    mockOpen->returnValue   = 15.0;
    mockGoogle->returnValue = 25.0;
    factory->registerClient("openweather", [mockOpen]()   { return mockOpen; });
    factory->registerClient("google",      [mockGoogle]() { return mockGoogle; });

    Forecast::Controllers::CurrentWeatherController ctrl(factory);
    std::vector<std::tuple<double, double, std::string>> requests = {
        {10.0, 20.0, "openweather"},
        {30.0, 40.0, "google"}
    };
    auto results = ctrl.GetMultipleCurrentWeather(requests).get();
    ASSERT_EQ(results.size(), 2u);
    EXPECT_DOUBLE_EQ(results[0].temperature, 15.0);
    EXPECT_DOUBLE_EQ(results[1].temperature, 25.0);
}

TEST(BatchWeatherTest, EmptyRequestReturnsEmptyResult) {
    auto factory = std::make_shared<Forecast::Clients::WeatherClientFactory>();
    Forecast::Controllers::CurrentWeatherController ctrl(factory);

    std::vector<std::tuple<double, double, std::string>> requests = {};
    auto results = ctrl.GetMultipleCurrentWeather(requests).get();
    EXPECT_TRUE(results.empty());
}

TEST(BatchWeatherTest, PropagatesExceptionFromOneLocation) {
    auto factory = std::make_shared<Forecast::Clients::WeatherClientFactory>();
    auto mock = std::make_shared<MockWeatherClient>();
    mock->shouldThrow = true;
    factory->registerClient("openweather", [mock]() { return mock; });
    Forecast::Controllers::CurrentWeatherController ctrl(factory);
    std::vector<std::tuple<double, double, std::string>> requests = {
        {10.0, 20.0, "openweather"}
    };
    EXPECT_THROW(
        ctrl.GetMultipleCurrentWeather(requests).get(),
        Forecast::Utils::ApiCallException
    );
}

// ── GeocodingService ──────────────────────────────────────────────────────────

TEST(GeocodingTest, ReturnsCoordinatesForCity) {
    Forecast::Services::GeocodingService geocoder;
    auto [lat, lon] = geocoder.GetCoordinates("London");
    EXPECT_NEAR(lat, 51.5074, 0.001);
    EXPECT_NEAR(lon, -0.1278, 0.001);
}

TEST(GeocodingTest, AllCitiesReturnCorrectCoordinates) {
    Forecast::Services::GeocodingService geocoder;

    auto [latM, lonM] = geocoder.GetCoordinates("Minsk");
    EXPECT_NEAR(latM, 53.9045, 0.01);

    auto [latL, lonL] = geocoder.GetCoordinates("London");
    EXPECT_NEAR(latL, 51.5074, 0.01);

    auto [latT, lonT] = geocoder.GetCoordinates("Tokyo");
    EXPECT_NEAR(latT, 35.6895, 0.01);

    auto [latS, lonS] = geocoder.GetCoordinates("Shanghai");
    EXPECT_NEAR(latS, 31.2304, 0.01);

    auto [latW, lonW] = geocoder.GetCoordinates("Warsaw");
    EXPECT_NEAR(latW, 52.2297, 0.01);
}

TEST(GeocodingTest, CaseInsensitive) {
    Forecast::Services::GeocodingService geocoder;
    auto [lat, lon] = geocoder.GetCoordinates("minsk");
    EXPECT_NEAR(lat, 53.9045, 0.001);
}

TEST(GeocodingTest, UnknownCityThrows) {
    Forecast::Services::GeocodingService geocoder;
    EXPECT_THROW(geocoder.GetCoordinates("Amsterdam"), std::runtime_error);
}

TEST(GeocodingTest, AllCitiesPresent) {
    Forecast::Services::GeocodingService geocoder;
    EXPECT_NO_THROW(geocoder.GetCoordinates("Tokyo"));
    EXPECT_NO_THROW(geocoder.GetCoordinates("Shanghai"));
    EXPECT_NO_THROW(geocoder.GetCoordinates("Warsaw"));
}

// ── main ──────────────────────────────────────────────────────────────────────

int main(int argc, char **argv) {
    ::testing::InitGoogleTest(&argc, argv);
    return RUN_ALL_TESTS();
}