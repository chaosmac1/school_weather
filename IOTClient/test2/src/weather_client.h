#include <Arduino.h>
#include <ESP8266WiFi.h>
#include "tuple.h"
#include "error.h"

#ifndef WEATHER_CLIENT_H
#define WEATHER_CLIENT_H
typedef String str;

class weather_client {
private:
  str ssid;
  str passwd;
  WiFiClient client;
  str host;
  uint16_t port;
  bool clientDefine;

  bool helf() {
    if (client.connected() != true)  {
      start(this->host.c_str(), this->port);
    }
   
    return client.connected();
  }

  void startClient() {
    client = WiFiClient();
      if (client.connect(host.c_str(), port)) {
        Serial.print(Error::clientNotDef);
        return;
    }
  }

  void startWiFi() {
    WiFi.begin((char *)ssid.c_str(), passwd.c_str());

    while (WiFi.status() != WL_CONNECTED) {
      delay(500);
    }
  }

  void stopWiFi() {
    WiFi.disconnect();
  }

  void stopClient() {
    client.stop();
  }

public:
    weather_client(const str &ssid, const str &passwd) {
      this->ssid = ssid;
      this->passwd = passwd;
      this->clientDefine = false;
    }

    void start(const str &host, const uint16_t &port) {
      this->host = host;
      this->port = port;
      this->clientDefine = true;

      startWiFi();
      startClient();
    }

    void send(const str &string) {
      if (clientDefine == false) {
        Serial.print(Error::clientNotDef);
        return;
      }

      client.print(string);
    }

    tuple<str, bool> receive() {
      if (clientDefine == false) {
        // TODO Error;
        return tuple<str, bool>(str(), true);
      }
      
      return tuple<str, bool>(client.readString(), false);
    }

    void restart() {
      stopClient();
      stopWiFi();
      startWiFi();
      startClient();
    }
};

#endif