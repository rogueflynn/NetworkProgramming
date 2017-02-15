import socket
import select
import Queue

server = socket.socket()			#Create socket object
port = 12345					#Set port to listen on
ip = '192.168.1.133'				#Specify the ip address listeing from

server.bind((ip, port))				#bind the port to the ip

server.listen(5)				#listen for connections

inputs = [ server ]				#sockets reading from

outputs = []					#sockets writing to

message_queue = {}				#stores data in dictionary 

print 'Listening'

#nonblocking
while inputs:
	#readable server socket is ready to accept connections
	read, write, exceptional = select.select(inputs, outputs, inputs)

	#handle inputs 
	for s in read:
		if s is server:
			#client connected
			client, addr = server.accept()
			print 'Got connection from ', addr
			client.setblocking(0)
			inputs.append(client)
		
			#Give connection a queue for data we want to send
			message_queue[client] = Queue.Queue()
		else:
			data = s.recv(1024)
			#readable client socket has data
			if data:
				print 'Received: ', data
				message_queue[s].put(data)
				#Add output channel for response
				if s not in outputs:
					outputs.append(s)
			else:
				#Interpret empty result as closed connection
				print addr, ' has disconnected'
				#stop listening for input on connection
				if s in outputs:
					outputs.remove(s)
				inputs.remove(s)
				#s.close()
				del message_queue[s]
	#handle outputs
	for s in write:
		try:
			next_msg = message_queue[s].get_nowait()
		except Queue.Empty:
			#no messages so stop checking for writability
			print 'Output queue for ', s.getpeername(), ' is empty'
			outputs.remove(s)
		else:
			print 'Sending ', next_msg, ' to ', s.getpeername() 
			s.send(next_msg)

	#handle errors
	for s in exceptional:
		print 'Error occured for ', s.getpeername()
		#stop listening for inputs on incoming connection
		inputs.remove(s)
		if s in outputs:
			outputs.remove(s)
		s.close()

		del message_queue[s]
		
	
#Blocking
#while True:
#	client, addr = server.accept()
#	print 'Got connection from ', addr
#	client.send('Thanks for connecting')
#	client.close()
