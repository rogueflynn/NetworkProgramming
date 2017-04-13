import socket
import select
import queue
import json
from socket import error as SocketError

server = socket.socket(socket.AF_INET, socket.SOCK_STREAM)			#Create socket object
port = 12345					#Set port to listen on
ip = '192.168.1.133'				#Specify the ip address listeing from

#can get ip bast on the doman name of a site
#server_ip = socket.gethostbyname(server)

server.bind((ip, port))				#bind the port to the ip

server.listen(5)				#listen for connections

inputs = [ server ]				#sockets reading from

outputs = []					#sockets writing to

message_queue = {}				#stores data in dictionary 

router = {}					#user to store routing information
connected = {}

print('Listening')

#nonblocking
while inputs:
	#readable server socket is ready to accept connections
	read, write, exceptional = select.select(inputs, outputs, inputs) #returns 3 lists

	#handle inputs 
	for sock in read:
		if sock is server:
			#client connected
			client, addr = server.accept()
			print('Got connection from ', addr)
			client.setblocking(0)
			inputs.append(client)
		
			#Give connection a queue for data we want to send
			message_queue[client] = queue.Queue()
		else:
			try:
				data = sock.recv(1024)
			except ConnectionResetError:
				continue

			#readable client socket has data
			if data:
				print('Received: ', data.decode())
				jsonData = data.decode()
				parsedData = json.loads(jsonData)
				message = parsedData["message"]	
				init = parsedData["init"]
				disconnect = parsedData["disconnect"]
				if disconnect == "1":
					#Interpret empty result as closed connection
					identifier = ""
					user = parsedData["user"]
					terminatingSock = router[user]
					print(terminatingSock.getpeername(), ' has disconnected')
					#stop listening for input on connection
					
					if terminatingSock in outputs:
						outputs.remove(terminatingSock)
					
					#this for block needs to change to account for different users on the same 
					#ip address. can use mac address of device to distiguish
	#				for key in router:
	#					if router[key] == sock:
	#						identifier = key 
	#						break
	#				if identifier != "":
	#					del router[identifier]	#delete entry from the router table	

					inputs.remove(terminatingSock)
					terminatingSock.close()
					del router[user]	#delete entry from the router table	
					del message_queue[terminatingSock]
				else:
					if init == "1":
						#set socket for user
						user = parsedData["user"]
						router[user] = sock		
						print(user, " has connected.");
						message_queue[sock].put("Connection Initialized")	
						#Add output channel for response
						if sock not in outputs:
							outputs.append(sock)
					else:
						receiverName = parsedData["recipient"]
						print("Receiver", receiverName)
						if receiverName in router:
							recepientSocket = router[receiverName]	#get recepient socket
							message_queue[recepientSocket].put(message)
							#Add output channel for response
							if recepientSocket not in outputs:
								outputs.append(recepientSocket)
						else:
							user = parsedData["user"]
							sendingSock = router[user]
							outputs.append(sendingSock)
							message_queue[sendingSock].put("User is not connected")	
			else:
				identifier = ""
				#print(sock.getpeername(), ' has disconnected')
				#stop listening for input on connection
				if sock in outputs:
					outputs.remove(sock)
				
				#this for block needs to change to account for different users on the same 
				#ip address. can use mac address of device to distiguish
				for key in router:
					if router[key] == sock:
						identifier = key 
						break
				print(identifier, " has disconnected");	
				if identifier != "":
					del router[identifier]	#delete entry from the router table	

				inputs.remove(sock)
				sock.close()
						
	#handle outputs
	for sock in write:
		try:
			next_msg = message_queue[sock].get_nowait()
		except queue.Empty:
			#no messages so stop checking for writability
			print('Output queue for ', sock.getpeername(), ' is empty')
			outputs.remove(sock)
		else:
			print('Sending ', next_msg, ' to ', sock.getpeername())
			sock.send(next_msg.encode())

	#handle errors
	for sock in exceptional:
		print('Error occured for ', sock.getpeername())
		#stop listening for inputs on incoming connection
		inputs.remove(sock)
		if sock in outputs:
			outputs.remove(sock)
		sock.close()

		del message_queue[sock]
		
	
