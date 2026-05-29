#pragma once
#include <stdexcept>
#include <string>

namespace Forecast::Utils {
	class ApiCallException : public std::runtime_error {
	public:
		explicit ApiCallException(const std::string& message)
			: std::runtime_error(message) {}
	};
}