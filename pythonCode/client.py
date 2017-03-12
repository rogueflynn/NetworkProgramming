#victor2
import socket
import sys
import json
import threading

#exit flag
exitFlag = False
client = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
ip = '172.16.0.9'
port = 12345

client.connect((ip, port))

email = sys.argv[1]	#read in email as command line argument

class clientThread(threading.Thread):
	def __init__(self, threadID, name):
		threading.Thread.__init__(self)
		self.threadID = threadID
		self.name = name
	def run(self):
		print("Starting: " + self.name + " thread")
		if self.name == "send":
			sendMessages(self.name)
		elif self.name == "receive":
			receiveMessages(self.name)
		print("Exiting thread: " + self.name)
			


def receiveMessages(threadName): 
	while True:
		received = client.recv(1024)
		print(received.decode()) 	#1024 is how much data that client is going to be receiving.

def sendMessages(threadName):
	#JSON string that is used to initialize the user
	initialize = '{"user": "' + email + '", "message": "init"}'
	Init = True

	while True:
		if Init is False: 
			message = input("")
			data = '{"user" : "' + email + '", "recipient": "victor1", "message": "' + message + '"}'
			if message == "exit()":
				break
			else:
				client.send(data.encode())
		else:
			#Send initialization data to the server
			print("Initializing...")
			client.send(initialize.encode())
			Init = False
			print("Ready")

#Create threads
clientSend = clientThread(0, "send")
clientReceive = clientThread(1, "receive")

#start threads
clientSend.start()
clientReceive.start()

#wait for threads to finish
clientSend.join()
clientReceive.join()

#close connection
client.close()

print("closing connection and exiting main thread")
		
