#define TINY_BME280_I2C

#include <Arduino.h>
#include "weather_client.h"
#include "weather.h"
#include "tuple.h"

typedef String str;

const str ssid = "ssid";
const str passwd = "passwd";
const str hostname = "myNetwork";
const uint16_t port = 0;
const str key = "key";

weather_client client = weather_client(ssid, passwd); 
weather sensor = weather();


void setup() {
    Serial.begin(9600);
    client.start(hostname, port);
}

void loop() {	
    client.send(sensor.getInfo(key).to_string());
    tuple<str, bool> fromServer = client.receive();
    if (fromServer.sek == true) {
        // TODO Error
    }
}