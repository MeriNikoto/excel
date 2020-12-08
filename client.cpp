#include &lt;stdio.h&gt;
#include &quot;winsock2.h&quot;
void main() {
	//----------------------
	// Initialize Winsock
	WSADATA wsaData;
	int iResult = WSAStartup(MAKEWORD(2, 2), &amp; wsaData);
	if (iResult != NO_ERROR)
		printf(&quot; Error at WSAStartup()\n & quot;);
	//----------------------
	// Create a SOCKET for connecting to server
	SOCKET ConnectSocket;
	ConnectSocket = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
	if (ConnectSocket == INVALID_SOCKET) {
		printf(&quot; Error at socket() : % ld\n & quot;, WSAGetLastError());
		WSACleanup();
		return;
	}
	//----------------------
	// The sockaddr_in structure specifies the address family,
	// IP address, and port of the server to be connected to.
	sockaddr_in clientService;
	clientService.sin_family = AF_INET;

	Взаємодія розподілених процесів через механізм сокетів

		13
		clientService.sin_addr.s_addr = inet_addr(&quot; 127.0.0.1 & quot; );
	clientService.sin_port = htons(27015);
	//----------------------
	// Connect to server.
	if (connect(ConnectSocket, (SOCKADDR*) & amp; clientService, sizeof(clientService)) == SOCKET_ERROR) {
		printf(&quot; Failed to connect.\n & quot; );
		WSACleanup();
		return;
	}
	//----------------------
	// Declare and initialize variables.
	int bytesSent;
	int bytesRecv = SOCKET_ERROR;
	char sendbuf[32] = &quot; Client: Sending data.& quot;;
	char recvbuf[32] = &quot; &quot;;
	//----------------------
	// Send and receive data.
	bytesSent = send(ConnectSocket, sendbuf, strlen(sendbuf), 0);
	printf(&quot; Bytes Sent : % ld\n & quot;, bytesSent);
	while (bytesRecv == SOCKET_ERROR) {
		bytesRecv = recv(ConnectSocket, recvbuf, 32, 0);
		if (bytesRecv == 0 || bytesRecv == WSAECONNRESET) {
			printf(&quot; Connection Closed.\n & quot;);
			break;
		}
		printf(&quot; Bytes Recv : % ld\n & quot;, bytesRecv);
	}
	WSACleanup();
	return;
}