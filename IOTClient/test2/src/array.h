#include <Arduino.h>

#ifndef ARRAY_H 
#define ARRAY_H 

template <typename T>
class array {
    size_t size;
    T * pointer_to_array;
public:
    array(const int size) {
        this->size = size;
        pointer_to_array = new T[size];
    }

    int length() { return this->size; }

    bool set_at(const size_t index, const T value) {
        if (index >= this->size) {
            return true;
        }
        pointer_to_array[index] = value;
        return false;
    }

    T get_at(const size_t index) {
        if (index >= this->size) {
            throw true;
        }

        return pointer_to_array[index];
    }

    ~array() {
        free(this->pointer_to_array);
    }

    T * get_ptr() {
       return this->pointer_to_array;
    }
};

#endif