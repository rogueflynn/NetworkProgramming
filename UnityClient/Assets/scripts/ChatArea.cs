using UnityEngine;
using System.Threading;
using System.Collections;
using System;
using System.IO;
using System.Net.Sockets;
using UnityEngine.UI;
using SimpleJSON;

public class ChatArea : MonoBehaviour {
    Thread clientRecvThread;
    Stream s;
    StreamWriter sw;
    TcpClient client;
    LiteralEscape literal;
    Text messageArea;
    InputField sendText;

    string sender = "victor2";
    string recipient = "victor1";
    string data = "";
    string messages = "";
    string socket = "";
    float connectTimer = 0.0f;

    bool disconnected;
    bool initialized = false;
    bool displayNotConnectedMessage = false;

	// Use this for initialization
	void Start () {
        //Client client = new Client(sender, recipient);
        literal = new LiteralEscape();
        messageArea = GameObject.Find("messageArea").GetComponent<Text>();
        sendText = GameObject.Find("sendText").GetComponent<InputField>();
        displayNotConnectedMessage = true;
        connectToServer();
	}

    void connectToServer()
    {
        try
        {
            client = new TcpClient();
            client.Connect("159.203.219.224", 32645);
            s = client.GetStream();
            disconnected = false;
            //initialize the user on the server
            StartCoroutine(clientSend());
            clientRecvThread = new Thread(new ThreadStart(recvMsg));
            clientRecvThread.IsBackground = true;
            displayNotConnectedMessage = false;
            clientRecvThread.Start();
           
        } catch(SocketException sx) {
            disconnected = true;
            if (displayNotConnectedMessage)
            {
                messages += "Not Connected";
                displayNotConnectedMessage = false;
            }
        }
    }

    public void sendStreamMessage()
    {
        if (sendText.text != "")
        {
            ClientGlobals.message = sendText.text;
            sendText.text = "";
            ClientGlobals.messageSent = true;
        }
    }


    //Checks to see if the socket is still connected
    bool SocketConnected(Socket s)
    {
        bool part1 = s.Poll(1000, SelectMode.SelectRead);
        bool part2 = (s.Available == 0);
        if (part1 && part2)
            return false;
        else
            return true;
    }

    void Update()
    {
        messageArea.text = messages;
        if (!disconnected) {
            if(!SocketConnected(client.Client))
            {
                disconnected = true;
                s.Close();
                sw.Close();
                initialized = false;
                messages += "Disconnected from the server\n";
            }
        }

        if(disconnected)
        {
            connectTimer += Time.deltaTime;
            if (connectTimer > 5.0f)
            {
                connectTimer = 0.0f;
                connectToServer();
            }
        }
    }

    IEnumerator clientSend()
    {
         if (!disconnected)
        {
            data = "{\"user\" : \"" + this.sender + "\", \"recipient\": \"\", \"message\": \"\", \"init\": \"1\", \"disconnect\": \"0\", \"socket\": \"\"}";
            sw = new StreamWriter(s);
            sw.WriteLine(data);
            sw.AutoFlush = true;
            Debug.Log("Client Send Called");
            while (!disconnected)
            {
                sw.Flush();
                Debug.Log("Made it to send method loop");
                if (ClientGlobals.messageSent)
                {
                    Debug.Log("Made it to send method");
                    string message = ClientGlobals.message;

                    if (message.Contains("\\"))
                    {
                        message = literal.escapeCharacter(message, '\\');
                    }
                    if (message.Contains("\""))
                    {
                        message = literal.escapeCharacter(message, '\"');
                    }

                    //User has disconnected
                    if (message == "exit()")
                    {
                        data = "{\"user\" : \"" + this.sender + "\", \"recipient\": \"\", \"message\": \"\", \"init\": \"0\", \"disconnect\": \"1\", \"socket\": \"" + socket + "\"}";
                        sw.WriteLine(data);
                        disconnected = true;
                        break;
                    }
                    //User has sent a message
                    else
                    {
                        data = "{\"user\" : \"" + this.sender + "\", \"recipient\": \"" + this.recipient + "\", \"message\": \"" + message + "\", \"init\": \"0\", \"disconnect\": \"0\", \"socket\": \"" + socket + "\"}";
                        sw.WriteLine(data);
                        Debug.Log("Message: " + message);
                        messages += "<b><color=red>" + this.sender + ":</color></b> " + message + "\n";
                        ClientGlobals.messageSent = false;
                    }
                }
                yield return new WaitForSeconds(0.001f);
            }
        }
    }

    void recvMsg()
    {
	    string serverMessage = "";
        
        //Debug.Log("Client Recv Called");
        while (!disconnected)
        {
            Debug.Log("Made it to recv method");
            byte[] bb = new byte[1000];
            int k = s.Read(bb, 0, 1000);     //Reads in a stream of bytes

            for (int i = 0; i < k; i++)
            {
                serverMessage += Convert.ToChar(bb[i]).ToString();
            }

            if (serverMessage != "")
            {
                //Console.WriteLine(serverMessage);
                Debug.Log(serverMessage);
                if (!initialized)
                {
                    var data = JSON.Parse(serverMessage);
                    socket = data["socket"].Value;
                    Debug.Log("Socket: " + socket);
                    messages += data["message"].Value + "\n";
                    initialized = true;
                }
                else
                {
                    var data = JSON.Parse(serverMessage);
                    messages += "<b><color=blue>" + this.recipient + ":</color></b> " + data["message"].Value + "\n";
                }

                serverMessage = "";
            }
        }
    }

    private void OnApplicationQuit()
    {
        if (!disconnected)
        {
            disconnected = true;
            s.Close();
            sw.Close();
        }
    }
}
