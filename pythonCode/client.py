import socket

client = socket.socket()
ip = '192.168.1.133'
port = '12345'

client.connect((ip, port))
client.connect((ip, port))


while True:
	message = raw_input("")
	if message == "exit()":
		client.close()
		break
	else:
		client.send(message)
		print client.recv(1024) #1024 is how much data that client is going to be receiving.