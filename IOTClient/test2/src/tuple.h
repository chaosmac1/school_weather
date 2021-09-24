#include <Arduino.h>

#ifndef TUPLE_H
#define TUPLE_H
template <typename T, typename E>
class tuple {
  public:
    T frist = "";
    E sek = "";

    tuple(const T frist, const E sek) {
      this->frist = frist;
      this->sek  = sek;
    }
};

#endif