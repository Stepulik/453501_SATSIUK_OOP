cmake_minimum_required(VERSION 3.10.0)
project(orders_c++ VERSION 0.1.0 LANGUAGES C CXX)

set(CMAKE_CXX_STANDARD 20)

set(CMAKE_CXX_STANDARD_REQUIRED ON)

set(CMAKE_CXX_EXTENSIONS OFF)

add_executable(orders_c++ 

                main.cpp

                infrastructure.h
                models.h
                processor.h
                staff.h
)

