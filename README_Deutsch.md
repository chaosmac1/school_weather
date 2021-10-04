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

In diesem Schulprojekt werden zwei Wetterstation gebaut, eine auf dem Schuldach und die andere am Steinhuder Meer, die mithilfe eines Arduino BME280 Sensor's? Daten wie
die Windgeschwindigkeit, Windrichtung, Temperatur, Luftdruck und Höhe des Sensors und das Datum (Timestamp) ermittelt und anschließend dank einer Client/Server Applikation
an eine Zentrale Webseite, zur Auswertung sowie der grafischen Darstellung der Ergebnisse, weiterleitet.

## Website Deployment
### Requirements
- docker
- docker-compose
- git

## Network
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
