#victor2
import socket
import sys
import json

client = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
ip = '172.16.0.9'
port = 12345

client.connect((ip, port))

email = sys.argv[1]	#read in email as command line argument

#JSON string that is used to initialize the user
initialize = '{"user": "' + email + '", "message": "init"}'
Init = True

while True:
	if Init is False: 
		message = input("")
		data = '{"user" : "' + email + '", "recipient": "victor1", "message": "' + message + '"}'
		if message == "exit()":
			client.close()
			break
		else:
			client.send(data.encode())
			received = client.recv(1024)
			print(received.decode()) 	#1024 is how much data that client is going to be receiving.
	else:
		#Send initialization data to the server
		print("Initializing...")
		client.send(initialize.encode())
		received = client.recv(1024)
		print(received.decode()) 	#1024 is how much data that client is going to be receiving.
		Init = False
		print("Ready")
		
