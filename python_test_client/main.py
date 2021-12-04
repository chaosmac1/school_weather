import random
import socket
import json

ip = "127.0.0.1"
key = "BrotMot"
port = 3380


def send_value() -> str:
    return json.dumps({
        "Key": key,
        "Temp": random.randrange(1, 20),
        "WindSpeed": random.randrange(1, 20),
        "Humidity": random.randrange(1, 20),
        "WindDirection": random.randrange(1, 360)
    })


def create_client():
    client = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    client.connect((ip, port))
    return client


def main():
    client = create_client()
    while True:
        client.send(str.encode(send_value()))
        from_server = client.recv(4096).decode("UTF-8")
        print("From Server:")
        print(from_server)


if __name__ == '__main__':
    main()
