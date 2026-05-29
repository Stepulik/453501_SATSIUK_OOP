#pragma once
#include <string>
#include <cstdint>

namespace Forecast::Shared::Responses {
	template <typename T>
	struct Success {
		uint16_t code;
		T data;
		std::string message;

		static Success create(uint16_t code, std::string message, T data) {
			return { code, std::move(data), std::move(message) };
		}
	};
}