//sockfd is that socket desrcriptor that you want to send data to.
//msg is the message you want to send
int send(int sockfd, const void *msg, int len, int flags);

int recv(int sockfd, void *buf, int len, int flags);
