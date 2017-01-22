#include <sys/types.h>
#include <sys/socket.h>

//Arguments say what type of socket it is.
//domain = IPv4 or IPv6 
//type = stream or datagram
//protocol = TCP or UDP
int socket(int domain, int type, int protocol);

int bind(int sockfd, struct sockaddr *myaddr, int addrlen);


