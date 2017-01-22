#include<sys/type.h>
#include<sys/socket.h>
#include<netdb.h>

/*
int getaddrinfo(const char *node,		//e.g. "www.google.com" or IP
		const char *service,		//e.g. "http" or port number
		const struct addrinfo *hints,
		struct addrinfo **res);		//results
*/

int status;
struct addrinfo hints;
struct addrinfo *servinfo;		//will point to the results

memset(&hints, 0, sizeof hints);	//Make sure the struct is empty
hints.ai_family = AF_UNSPEC;  		//Dont care if IPv4 or IPv6
hints.ai_socktype = SOCK_STREAM;	//TCP stream sockets
hints.ai_flags = AI_PASSIVE;		//fill in my ip for me


if ((status = getaddrinfo(NULL, "3490", &hints, &servinfo)) != 0) {
	fprintf(stderr, "getaddrinfo error: %s\n", gai_strerror(status));
	exit 1;
}

freeaddrinfo(servinfo); 	//free linked list
