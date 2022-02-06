#ifndef WEATHER_H
#define WEATHER_H

#include <Arduino.h>
#include "./../.pio/libdeps/d1_mini/Adafruit BMP280 Library/Adafruit_BMP280.h"
#include "string_builder.h"
#include "array.h"
#include "console.h"
#include "../.pio/libdeps/d1_mini/Tiny BME280/lib/TinyBME280Impl.h"
#include <Wire.h>
#include <SPI.h>

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
        builder.append("\""+ this->key + "\"");
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
    Adafruit_BMP280 bmp;
public:
    weather() { }

    void init() {
        Serial.println(F("BMP280 Sensor event test"));
        Serial.flush();
        unsigned status = bmp.begin(BMP280_ADDRESS_ALT, BMP280_CHIPID);
        if (!status) {
            Serial.println(F("Could not find a valid BMP280 sensor, check wiring or "
                             "try a different address!"));
            Serial.print("SensorID was: 0x"); Serial.println(bmp.sensorID(),16);
            Serial.print("        ID of 0xFF probably means a bad address, a BMP 180 or BMP 085\n");
            Serial.print("   ID of 0x56-0x58 represents a BMP 280,\n");
            Serial.print("        ID of 0x60 represents a BME 280.\n");
            Serial.print("        ID of 0x61 represents a BME 680.\n");
            while (true) delay(100);
        }
        bmp.setSampling(Adafruit_BMP280::MODE_NORMAL,     /* Operating Mode. */
                        Adafruit_BMP280::SAMPLING_X2,     /* Temp. oversampling */
                        Adafruit_BMP280::SAMPLING_X16,    /* Pressure oversampling */
                        Adafruit_BMP280::FILTER_X16,      /* Filtering. */
                        Adafruit_BMP280::STANDBY_MS_500); /* Standby time. */
    }

    weatherInfo getInfo(const str &key) {
        auto temp = (uint32)this->bmp.readTemperature();
        auto windSpeed = (uint32)this->bmp.readPressure();

        auto humidity = (uint32)0;
        auto windDirection = (uint32)0;

        return weatherInfo(key, temp, windSpeed, humidity, windDirection);
    }
};

#endif