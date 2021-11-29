# Projektdokumentation - Status: unfinished

## Projektbeschreibung:

In diesem Schulprojekt werden zwei Wetterstation gebaut, eine auf dem Schuldach und die andere am Steinhuder Meer, die mithilfe eines Arduino BME280 Sensor's? Daten wie
die Windgeschwindigkeit, Windrichtung, Temperatur, Luftdruck und Höhe des Sensors und das Datum (Timestamp) ermittelt und anschließend dank einer Client/Server Applikation
an eine Zentrale Webseite, zur Auswertung sowie der grafischen Darstellung der Ergebnisse, weiterleitet.

## Website Deployment
### Anforderungen
- docker
- docker-compose
- git

## Netzwerk
Die Ports 80, HTTP, und 3370, TCP/UDP, werden für dieses Projekt verwendet.

Zum erhöhen der Leistung und Sicherheit entschieden wir uns ***cloudflare*** und ***certbot*** sowie ***yaml files*** für docker komponieren.

## Installation
Download brance
```bash
$ git clone https://github.com/chaosmac1/school_weather.git
```
Run Build Script
```bash
$ cd ./school_weather
$ ./build.sh
```
