# Projektdokumentation - Status: unfinished

## Inhalt
- Projektbeschreiung
- Website Deployment
  -> Requirements
  -> Hardware
  -> Software
- Installation
    -> Commands
- Quellen

## Projektbeschreibung:

In this schoolproject we build two weatherstations, one on top of the school roof and another near the Steinhuder Sea,
that collect data with an Arduino BME280 sensor. The Data like the wind speed, wind direction, temperature, air pressure, altitude
and a timestamp will be detected. For the evaluation as well as the graphic representation of the results the collected
information will be send via a client/server application to a central website.

In diesem Schulprojekt werden zwei Wetterstation gebaut, eine auf dem Schuldach und die andere am Steinhuder Meer, die mithilfe eines Arduino BME280 Sensor's? Daten wie
die Windgeschwindigkeit, Windrichtung, Temperatur, Luftdruck und Höhe des Sensors und das Datum (Timestamp) ermittelt und anschließend dank einer Client/Server Applikation
an eine Zentrale Webseite, zur Auswertung sowie der grafischen Darstellung der Ergebnisse, weiterleitet.

## Website Deployment
### Requirements
- docker
- docker-compose
- git

## Network
Die Ports 80, HTTP, und 3370, TCP/UDP, werden für dieses Projekt verwendet.

Zum erhöhen der Leistung und Sicherheit entschieden wir uns ***cloudflare*** und ***certbot*** sowie ***yaml files*** für die Datenserialisierung.

The ports 80, HTTP, and 3370, TCP/UDP, are being used for this project.

To increase performance and secrurity we decided to use ***cloudflare*** and ***certbot*** and ***yaml files*** for data serialization.

## Installation
Download brance
```bash
$ git clone TODO
```
Run Build Script
```bash
$ cd ./school_weather
$ ./build.sh
```
