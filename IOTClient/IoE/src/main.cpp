#define TINY_BME280_I2C
#define DEBUG

#include <Arduino.h>
#include <TinyBME280.h>
#include "console.h"
#include "weather_client.h"
#include "weather.h"
#include "tuple.h"
#include "passwd.h"
#include <Wire.h>
#include <SPI.h>
using namespace console;

typedef String str;

const str ssid = "masterBox";
const str passwd = get_passwd();
const str hostname = "192.168.2.21";
const uint16_t port = 3380;
const str key = "BrotMot";

weather_client client = weather_client(ssid, passwd);
weather sensor = weather();

void setup() {
    console::begin(9600);
    sensor.init();

    client.start(hostname, port);
}

void loop() {
    auto info = sensor.getInfo(key);
    auto infoJson = info.to_string();
#ifdef DEBUG
    console::log("info From Sensor: " + infoJson + "\n");
#endif

    client.send(infoJson);
    tuple<str, bool> fromServer = client.receive();
    if (fromServer.sek) {
#ifdef DEBUG
        log("Error: (fromServer.sek)\n");
#endif
        ESP.restart();
    } else {
#ifdef DEBUG
        log("ok: (fromServer.sek)\n");
#endif
    }
}