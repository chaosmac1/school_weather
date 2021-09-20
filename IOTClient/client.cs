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

public static class tool {
	public static byte[] JsonStringToByteArray(string jsonByteString) {
        	jsonByteString = jsonByteString.Substring(1, jsonByteString.Length - 2);
        	string[] arr = jsonByteString.Split(',');
        	byte[] bResult = new byte[arr.Length];
        	
		for (int i = 0; i < arr.Length; i++) {
        	    bResult[i] = byte.Parse(arr[i]);
        	}
        	return bResult;
    	}
}

private class ReceiveIotValue {
	// ReSharper disable once UnusedAutoPropertyAccessor.Local
        public string? Key { get; set; }
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        public float Temp { get; set; }
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        public float WindSpeed { get; set; }
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        public float Humidity { get; set; }
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        public float WindDirection { get; set; }

        public Point ToPoint(long createTime) {
        	return new(createTime, Temp, WindSpeed, Humidity, WindDirection);
        }

        public static ReceiveIotValue? Factory(string jsonString) {
		try {
			return JsonConvert.DeserializeObject<ReceiveIotValue>(jsonString);
                }
                catch (Exception) {
                    return null;
                }
	}
}

void loop() {
	connect();
	
	//send_json() {}
	//rev_json() {}
}
