//
// Created by chaosmac on 31.01.22.
//

#ifndef SLIT_CONSOLE_H
#define SLIT_CONSOLE_H
#include <Arduino.h>


namespace console {
    void begin(int speed) {
        Serial.begin(speed);
    }

    void log(unsigned long long n, int base) {
        Serial.print(n, base);
        Serial.flush();
    }

    void log(double n, int digits) {
        Serial.print(n, digits);
        Serial.flush();
    }

    void log(const Printable& x) {
        Serial.print(x);
        Serial.flush();
    }

    void log(const __FlashStringHelper* ifsh) {
        Serial.print(ifsh);
        Serial.flush();
    }

    void log(const String &s) {
        Serial.print(s);
        Serial.flush();
    }

    void log(const char c[]) {
        Serial.print(c);
        Serial.flush();
    }

    void log(char c) {
        Serial.print(c);
        Serial.flush();
    }

    void log(unsigned char b, int base) {
        Serial.print(b, base);
        Serial.flush();
    }

    void log(int num, int base) {
        Serial.print(num, base);
        Serial.flush();
    }

    void log(unsigned int num, int base) {
        Serial.print(num, base);
        Serial.flush();
    }

    void log(long num, int base) {
        Serial.print(num, base);
        Serial.flush();
    }

    void log(unsigned long num, int base) {
        Serial.print(num, base);
        Serial.flush();
    }

    void log(long long num, int base) {
        Serial.print(num, base);
        Serial.flush();
    }
}

#endif //SLIT_CONSOLE_H
