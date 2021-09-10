/*
Libary

// Marcel
Socket TCP
DNS Auflösnung

Send Json  {
key: string,
temp: float,
windSpeed: float,
humidity: float,
windDirection: float
}

Rev Json { error: boolen }
Port ist erst mal egal
URL Ist erst mal egal

*/

#include <SPI.h>
#include <WiFi.h>
#include <Dns.h>
#include <ArduinoJson.h>

StaticJsonBuffer<200> jsonBuffer;

string hostname = "myNetwork";
char pass[] = "myPassword";
char ssid[] dns.getHostByName(hostname, IPv4);
int status = WL_IDLE_STATUS;
IPAddress server(ssid);

char json[] = "{\"key\":"[Contence]",\"temp\":"[Contence]",\"windSpeed\":"[Contence]",\"humidity\":"[Contence]",\"windDirection\":"[Contence]"}";
JsonObject& test_json = jsonBuffer.parseObject(json); //decode/parse the JSON string to a JsonObject

/* Extract data from JsonObject
string e_Key = test_json["key"];
float e_Temp = test_json["temp"];
float e_WindSpeed = test_json["windSpeed"];
float e_Humidity = test_json["humidity"];
float e_WindDirection = test_json["windDirection"];
*/

WiFiClient client;

void connect() {
	Serial.println("\nStarting connection...");
	if (client.connect(server, 80)) {
		Serial.println("connected");
	}
}

void setup() {
	Serial.begin(9600);
	Serial.println("Attempting to connect to WPA network...");
	Serial.print("SSID: ");
	Serial.println(hostname);
	
	status = WiFi.begin(hostname, pass);
	if ( status != WL_CONNECTED) {
		Serial.println("Couldn't get a wifi connection");
		while(true);
	}
	
	Serial.println("Connected to wifi");
	
	if(!test_json.success()) {
		Serial.println("parseObject() failed");
		return false;
	}
	
	
	
}

void loop() {
	connect();
}