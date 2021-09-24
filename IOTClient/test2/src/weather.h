#include <Arduino.h>
#include <TinyBME280.h>
#include "string_builder.h"
#include "array.h"

#ifndef WEATHER_H
#define WEATHER_H

typedef String str;

class weatherInfo {
    str key;
    str temp;
    str windSpeed;
    str humidity;
    str windDirection;
public:
    weatherInfo(const str &key, const uint32_t &temp, const uint32_t &windSpeed, const uint32_t &humidity, const uint32_t &windDirection) {
        this->key = key;
        this->temp = String(temp);
        this->windSpeed = String(windSpeed);
        this->humidity = String(humidity);
        this->windDirection = String(windDirection);
    }

    void setKey(const int &key) {
        this->key = String(key);
    }

    void setTemp(const int &temp) {
        this->temp = String(temp);
    }

    void setWindSpeed(const int &windSpeed) {
        this->windSpeed = String(windSpeed);
    }

    void setHumidity(const int &humidity) {
        this->humidity = String(humidity);
    }

    void setWindDirection(const int &windDirection) {
        this->windDirection = String(windDirection);
    }

    str to_string() {
        string_builder builder;
        builder.append("{ ");
        builder.append("key: ");
        builder.append(this->key);
        builder.append(", temp: ");
        builder.append(this->temp);
        builder.append(", windSpeed: ");
        builder.append(this->windSpeed);
        builder.append(", humidity: ");
        builder.append(this->humidity);
        builder.append(", windDirection: ");
        builder.append(this->windDirection);
        builder.append(" }");
        return builder.to_string();
    }

    array<byte> to_byte() {
        auto string = to_string();
        const byte * c_ptr = (const byte*)string.c_str();
        array<byte> res = array<byte>(string.length());

        for (int i = 0; i < res.length(); ++i) {
            res.set_at(i, c_ptr[i]);
        }

        return res;
    }
};


class weather {
    tiny::BME280 sensor;
public:
    weather() {
        sensor.begin();
    }

    weatherInfo getInfo(const str &key) {
        uint32_t temp = sensor.readFixedTempC() / 100;
        uint32_t humidity = sensor.readFixedHumidity();
        // uint32_t pressure = sensor.readFixedPressure() / 100;
        uint32_t windSpeed = 0;
        uint32_t windDirection = 0;
        // TODO WindSpeed
        // TODO WindDir

        return weatherInfo(key, temp, windSpeed, humidity, windDirection);
    }
};

#endif