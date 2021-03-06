struct addrinfo hints, *res;
int sockfd;

//first, load up address of structs with getaddrinfo
memset(&hints, 0, sizeof hints);
hints.ai_family = AF_UNSPEC;
hints.ai_socktype = SOCK_STREAM;

getaddrinfo("www.example.com", "3490", &hints, &res);

//make socket

sockfd = socket(res->ai_family, res->ai_socktype, res->ai_protocol);

//connect

connect(sockfd, res->ai_addr, res->ai_addrlen);
