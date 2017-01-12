#include<string.h>
#include<stdio.h>
#include<stdlib.h>
#include<sys/types.h>
#include<sys/socket.h>
#include<netinet/in.h>
#include<errno.h>
#include<arpa/inet.h>

main() {
	int sock, cli;	
	struct sockaddr_in server, client;
	unsigned int len;
	char mesg[] = "Hello to the world of socket programming!";
	int sent;

	if((sock = socket(AF_INET, SOCK_STREAM, 0)) == -1) {
		perror("socket: ");
		exit(-1);
	}

	server.sin_family = AF_INET;
	server.sin_port = htons(10000);
	server.sin_addr.s_addr = INADDR_ANY;
	bzero(&server.sin_zero, 8);

	//call bind
	len = sizeof(struct sockaddr_in);
	if((bind(sock, (struct sockaddr *) &server, len)) == -1) {
		perror("bind");
		exit(-1);
	}

	//listen
	if((listen(sock, 5)) == -1) {
		perror("listen");
		exit(-1);
	}

	//accept
	while(1) {
		//client
		if((cli = accept(sock, (struct sockaddr *) &client, &len)) == -1) {
			perror("accept");
			exit(-1);
		}	

		//send
		sent = send(cli, mesg, strlen(mesg), 0);
		printf("Sent %d bytes to client : %s\n", sent, inet_ntoa(client.sin_addr));
		//printf("Sent stuff to client");

		close(cli);
	}
}
