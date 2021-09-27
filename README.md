# Projektdokumentation - Status: unfinished
[![Build Status](https://travis-ci.org/marktext/marktext.svg?branch=gghhhh)](https://travis-ci.org/marktext/marktext)

## Inhalt
- Projektbeschreiugn
- Website Deployment
  -> Requirements
  -> Hardware
  -> Software
- Used Ports 80 & 3370
- Installation
    -> Commands
  (For HTTPS you're using cloudfare and/or yaml file, certbot: yes)
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

### Hardware

### Software

## Used Ports
Für dieses Projekt wurden der Standard Port 80 von HTTP und Port 3370 von ...

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
