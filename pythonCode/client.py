import socket

client = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
ip = '172.16.0.9'
port = 12345

client.connect((ip, port))

while True:
	message = input("")
	if message == "exit()":
		client.close()
		break
	else:
		client.send(message.encode())
		received = client.recv(1024)
		print(received.decode()) 	#1024 is how much data that client is going to be receiving.
