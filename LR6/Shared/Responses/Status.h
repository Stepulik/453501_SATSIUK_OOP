#pragma once
#include <string>
#include <cstdint>

namespace Forecast::Shared::Responses {
	struct Status {
		uint16_t code;
		std::string message;

		static Status create(uint16_t code, std::string message) {
			return { code, std::move(message) };
		}
	};
}