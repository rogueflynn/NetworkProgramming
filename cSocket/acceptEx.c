#include<string.h>
#include<sys/types.h>
#include<sys/socket.h>
#include<netinet/in.h>

#define MYPORT "3490"	//the port users will be connecting to
#define BACKLOG 10	//how many pending connections will hold

int main(void) {
	struct sockaddr_storage their_addr;
	socklen_t addr_size;
	struct addrinfo hints, *res;
	int sockfd, new_fd;

	//Don't forget error checking calls
	
	//first load up addres with getaddrinfo();
	memset(&hints, 0, sizeof hints);
	hints.ai_family = AF_UNSPEC;	//ipv4 or ipv6
	hints.ai_socktype = SOCK_STREAM;
	hints.ai_flags = AI_PASSIVE;	//Fill in my ip for me
	
	getaddrinfo(NULL, MYPORT, &hints, &res);

	//make a socket, bind it, and listen on it.
	sockfd = socket(res->ai_family, res->ai_socktype, res->ai_protocol);
	bind(sockfd, res->ai_addr, res->ai_addrlen);
	listen(sockfd, BACKLOG);

	//Now accept incoming connections
	addr_size = sizeof their_addr;
	new_fd = accept(sockfd, (struct sockaddr *)&their_addr, &addr_size);
	
	//ready to communicate on socket descriptor

	return 0;
}
