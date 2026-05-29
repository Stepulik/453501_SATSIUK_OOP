cmake_minimum_required(VERSION 3.10.0)
project(logistics_cpp VERSION 0.1.0 LANGUAGES C CXX)

include_directories(${CMAKE_CURRENT_SOURCE_DIR}/Api)
include_directories(${CMAKE_CURRENT_SOURCE_DIR}/Clients)
include_directories(${CMAKE_CURRENT_SOURCE_DIR}/Controllers)
include_directories(${CMAKE_CURRENT_SOURCE_DIR}/Models/Weather)
include_directories(${CMAKE_CURRENT_SOURCE_DIR}/Shared/Responses)
include_directories(${CMAKE_CURRENT_SOURCE_DIR}/Utils)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

find_package(Crow REQUIRED)

add_executable(logistics_cpp LR6.cpp)

target_link_libraries(logistic_cpp Crow::Crow)
