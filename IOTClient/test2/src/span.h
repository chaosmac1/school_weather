#include <Arduino.h>

#ifndef SPAN_H
#define SPAN_H
template <typename T>
class span {
    size_t size;
    T * pointer_to_array;
public:
    span(const T &arr, const size_t size) {
        this->size = size;
        this->pointer_to_array = arr;
    }

    size_t get_size() const {
        return size;
    }

    T *get_ptr() const {
        return pointer_to_array;
    }

    array<T> to_own() {
        return array<T>(new T[this->size], size);
    }
};

#endif