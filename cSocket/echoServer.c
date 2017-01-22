#include<string.h>
#include<stdio.h>
#include<stdlib.h>
#include<sys/types.h>
#include<sys/socket.h>
#include<netinet/in.h>
#include<error.h>
#include<arpa/inet.h>
#include<unistd.h>
#include<fcntl.h>  	//Needed for non-blocking (asynchronus)

#define ERROR 	-1
#define MAX_CLIENTS 	4
#define MAX_DATA	1024

main(int argc, char **argv) {
	struct sockaddr_in server;
	struct sockaddr_in client;
	int sock;	//Socket descripter
	int new;	//Client descripter
	int sockaddr_len = sizeof(struct sockaddr_in);
	int data_len;
	char data[MAX_DATA];

	//Server
	if((sock = socket(AF_INET, SOCK_STREAM, 0)) == ERROR) {
		perror("server socket: ");
		exit(-1);
	}
	//Set to asynchronus
	fcntl(sock, F_SETFL, O_NONBLOCK);

	server.sin_family = AF_INET;
	server.sin_port = htons(atoi(argv[1])); //Convert from host byte order to network byte order
	server.sin_addr.s_addr = INADDR_ANY;	//Instruct kernel to listen on all interfaces
	bzero(&server.sin_zero, 8);

	//bind the socket to an ip address
	if((bind(sock, (struct sockaddr *) &server, sockaddr_len)) == ERROR) {
		perror("bind : ");
		exit(-1);
	}

	//listen
	if((listen(sock, MAX_CLIENTS)) == ERROR) {
		perror("listen");
		exit(-1);
	}
	

	//accept
	while(1) {
		//Accept client connection 
		if((new = accept(sock, (struct sockaddr *) &client, &sockaddr_len)) == ERROR) {
			perror("accept");
			exit(-1);
		}	


		printf("New IP: %d\n", new);
		printf("New Client connected on port no %d and IP %s\n", ntohs(client.sin_port), inet_ntoa(client.sin_addr));
		data_len = 1;

		while(data_len) {
			data_len = recv(new, data, MAX_DATA, 0);

			if(data_len) {
				send(new, data, data_len, 0);
				data[data_len] = '\0';
				printf("Sent mesg: %s", data);
			}
		}

		printf("Client disconnected\n");

		close(new);

	}
}
