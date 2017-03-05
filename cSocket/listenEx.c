//sockfd = docket descriptor and backlog=number of connections allowed
//connections will wait in a queue until you accept

int listen(int sockfd, int backlog);
