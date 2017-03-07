import socket
import select
import queue

server = socket.socket(socket.AF_INET, socket.SOCK_STREAM)			#Create socket object
port = 12345					#Set port to listen on
ip = '172.16.0.9'				#Specify the ip address listeing from

#can get ip bast on the doman name of a site
#server_ip = socket.gethostbyname(server)

server.bind((ip, port))				#bind the port to the ip

server.listen(5)				#listen for connections

inputs = [ server ]				#sockets reading from

outputs = []					#sockets writing to

message_queue = {}				#stores data in dictionary 

print('Listening')

#nonblocking
while inputs:
	#readable server socket is ready to accept connections
	read, write, exceptional = select.select(inputs, outputs, inputs)

	#handle inputs 
	for s in read:
		if s is server:
			#client connected
			client, addr = server.accept()
			print('Got connection from ', addr)
			client.setblocking(0)
			inputs.append(client)
		
			#Give connection a queue for data we want to send
			message_queue[client] = queue.Queue()
		else:
			data = s.recv(1024)
			#readable client socket has data
			if data:
				print('Received: ', data.decode())
				message_queue[s].put(data.decode())
				#Add output channel for response
				if s not in outputs:
					outputs.append(s)
			else:
				#Interpret empty result as closed connection
				print(addr, ' has disconnected')
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
		except queue.Empty:
			#no messages so stop checking for writability
			print('Output queue for ', s.getpeername(), ' is empty')
			outputs.remove(s)
		else:
			print('Sending ', next_msg, ' to ', s.getpeername())
			s.send(next_msg.encode())

	#handle errors
	for s in exceptional:
		print('Error occured for ', s.getpeername())
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
