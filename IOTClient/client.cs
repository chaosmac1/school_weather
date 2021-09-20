#include <SPI.h>
#include <WiFi.h>
#include <Dns.h>
#include <ArduinoJson.h>

typedef std::string str;
typedef char byte;

template <typename T>
class array {
    size_t size;
    T * pointer_to_array;
public:
    array(const int size) {
        this->size = size;
        pointer_to_array = new T[size];
    }

    int length() { return this->size; }

    void set_at(const size_t index, const T value) {
        if (index >= this->size) {
            throw true;
        }
        pointer_to_array[index] = value;
    }

    T get_at(const size_t index) {
        if (index >= this->size) {
            throw true;
        }

        return pointer_to_array[index];
    }

    ~array() {
        free(this->pointer_to_array);
    }

    T * get_ptr() {
       return this->pointer_to_array;
    }
};

class string_builder {
private:
    std::string main;
    std::string scratch;

    const std::string::size_type ScratchSize = 1024;  // or some other arbitrary number

public:
    string_builder & append(const std::string & string) {
        scratch.append(string);
        if (scratch.size() > ScratchSize) {
            main.append(scratch);
            scratch.resize(0);
        }
        return *this;
    }

    const std::string & to_string() {
        if (scratch.size() > 0) {
            main.append(scratch);
            scratch.resize(0);
        }
        return main;
    }
};

class weather {
    str key;
    str temp;
    str windSpeed;
    str humidity;
    str windDirection;
public:
    weather(const str &key, const int &temp, const int &windSpeed, const int &humidity, const int &windDirection) {
        this->key = key;
        this->temp = std::to_string(temp);
        this->windSpeed = std::to_string(windSpeed);
        this->humidity = std::to_string(humidity);
        this->windDirection = std::to_string(windDirection);
    }

    void setKey(const int &key) {
        weather::key = std::to_string(key);
    }

    void setTemp(const int &temp) {
        weather::temp = std::to_string(temp);
    }

    void setWindSpeed(const int &windSpeed) {
        weather::windSpeed = std::to_string(windSpeed);
    }

    void setHumidity(const int &humidity) {
        weather::humidity = std::to_string(humidity);
    }

    void setWindDirection(const int &windDirection) {
        weather::windDirection = std::to_string(windDirection);
    }

    str to_string() {
        string_builder builder;
        builder.append("{ ");
        builder.append("key: ");
        builder.append(this->key);
        builder.append(", temp: ");
        builder.append(this->temp);
        builder.append(", windSpeed: ");
        builder.append(this->windSpeed);
        builder.append(", humidity: ");
        builder.append(this->humidity);
        builder.append(", windDirection: ");
        builder.append(this->windDirection);
        builder.append(" }");
        return builder.to_string();
    }

    array<byte> to_byte() {
        auto string = to_string();
        const byte * c_ptr = string.c_str();
        array<byte> res = array<byte>(string.size());

        for (int i = 0; i < res.length(); ++i) {
            res.set_at(i, c_ptr[i]);
        }

        return res;
    }
};



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

/*
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
*/
	
void loop() {
	connect();
	
	/*
	send_json() {
	}
	*/
	/*
	void rev_json() {
		
	}
	*/
}
