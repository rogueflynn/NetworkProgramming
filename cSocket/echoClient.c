#include<string.h>
#include<stdio.h>
#include<stdlib.h>
#include<sys/types.h>
#include<sys/socket.h>
#include<netinet/in.h>
#include<error.h>
#include<arpa/inet.h>
#include<unistd.h>

#define ERROR 	-1
#define BUFFER 1024

main(int argc, char **argv) {
	struct sockaddr_in remote_server; //Contains the ip address and port number of the server
	int sock;			  //Socket descripter
	char input[BUFFER];		  //What the user is entering 
	char output[BUFFER];		  //What the server is sending back
	int len;

	//Client
	if((sock = socket(AF_INET, SOCK_STREAM, 0)) == ERROR) {
		perror("socket");
		exit(-1);
	}

	remote_server.sin_family = AF_INET;
	remote_server.sin_port = htons(atoi(argv[2])); //Convert from host byte order to network byte order
	remote_server.sin_addr.s_addr = inet_addr(argv[1]); 
	bzero(&remote_server.sin_zero, 8);

	//Connect to the server  
	if((connect(sock, (struct sockaddr *) &remote_server, sizeof(struct sockaddr_in))) == ERROR) {
		perror("connect");
		exit(-1);
	}

	//accept
	while(1) {
		fgets(input, BUFFER, stdin);	        //Gets input from user
		send(sock, input, strlen(input), 0);	//Send to the server

		len = recv(sock, output, BUFFER, 0);	//What server sends back
		
		output[len] = '\0';			//Put null terminator
		printf("%s\n", output);			//Print what the server sends back
	}
	close(sock);
}
