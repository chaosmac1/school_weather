#include <Arduino.h>

#ifndef STRING_BUILDER_H
#define STRING_BUILDER_H

typedef String str;

class string_builder {
private:
    str main;
    str scratch;

public:
    string_builder & append(const str & string) {
        scratch += string;
        return *this;
    }

    const str & to_string() {
        main = scratch + "";
        return main;
    }
};

#endif