# Project documentation - Status: unfinished

Language to: [German](https://github.com/chaosmac1/school_weather/blob/main/README_DE.md)

Language to: [English](https://github.com/chaosmac1/school_weather/blob/main/README.md)

## Project description:

In this schoolproject we build two weatherstations, one on top of the school roof and another near the Steinhuder Sea,
that collect data with an Arduino BME280 sensor. The Data like the wind speed, wind direction, temperature, air pressure, altitude
and a timestamp will be detected. For the evaluation as well as the graphic representation of the results the collected
information will be send via a client/server application to a central website.

## Website Deployment
### Requirements
- docker
- docker-compose
- git

## Network
The ports 80, HTTP, and 3370, TCP/UDP, are being used for this project.

To increase performance and secrurity we decided to use ***cloudflare*** and ***certbot*** and ***yaml files*** for docker compose.

## Installation
Download branche
```bash
$ git clone https://github.com/chaosmac1/school_weather.git
```
Run Build Script
```bash
$ cd ./school_weather
$ ./build.sh
```
