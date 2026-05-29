cmake_minimum_required(VERSION 3.10.0)
project(logistics_cpp VERSION 0.1.0 LANGUAGES C CXX)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

include_directories(${CMAKE_CURRENT_SOURCE_DIR}/Api)
include_directories(${CMAKE_CURRENT_SOURCE_DIR}/Clients)
include_directories(${CMAKE_CURRENT_SOURCE_DIR}/Controllers)
include_directories(${CMAKE_CURRENT_SOURCE_DIR}/Models/Weather)
include_directories(${CMAKE_CURRENT_SOURCE_DIR}/Shared/Responses)
include_directories(${CMAKE_CURRENT_SOURCE_DIR}/Utils)

find_package(Crow REQUIRED)
find_package(cpr REQUIRED)
find_package(nlohmann_json REQUIRED)
find_package(GTest REQUIRED)
find_package(CURL REQUIRED)

add_executable(logistics_cpp
    LR6.cpp
    Api/WeatherApi.cpp
    Clients/OpenWeatherDataClient.cpp
    Clients/WeatherClientFactory.cpp
)
target_link_libraries(logistics_cpp
    Crow::Crow
    cpr::cpr
    nlohmann_json::nlohmann_json
    CURL::libcurl
)

enable_testing()

add_executable(run_tests
    tests/test_main.cpp
    Api/WeatherApi.cpp
    Clients/OpenWeatherDataClient.cpp
    Clients/WeatherClientFactory.cpp
)
target_link_libraries(run_tests
    GTest::GTest
    GTest::Main
    cpr::cpr
    nlohmann_json::nlohmann_json
    CURL::libcurl
)
add_test(NAME WeatherTests COMMAND run_tests)