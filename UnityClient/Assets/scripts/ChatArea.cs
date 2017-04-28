using UnityEngine;
using System.Threading;
using System.Collections;
using System;
using System.IO;
using System.Net.Sockets;
using UnityEngine.UI;

public class ChatArea : MonoBehaviour {
    string sender = "victor1";
    string recipient = "victor2";
    //Client client;
    //Thread clientSendThread;
    Thread clientRecvThread;
    Stream s;
    StreamWriter sw;
    TcpClient client;
    LiteralEscape literal;
    Text messageArea;
    InputField sendText;
    public bool disconnected;
    public bool serverStarted;
    string data = "";
    string messages = "";
    bool initialized = false;

	// Use this for initialization
	void Start () {
        //Client client = new Client(sender, recipient);
        client = new TcpClient();
        literal = new LiteralEscape();
        messageArea = GameObject.Find("messageArea").GetComponent<Text>();
        sendText = GameObject.Find("sendText").GetComponent<InputField>();
        try
        {
            client.Connect("192.168.1.133", 12345);
            s = client.GetStream();
            disconnected = false;
            serverStarted = true;
            //initialize the user on the server
           
        } catch(SocketException sx) {
            disconnected = true;
            serverStarted = false;
        }
        //clientSendThread = new Thread(new ThreadStart(client.send));
        //clientSendThread.IsBackground = true;
        //clientRecvThread.IsBackground = true;
        //clientSendThread.Start();
        //clientRecvThread.Start();
        StartCoroutine(clientSend());
        clientRecvThread = new Thread(new ThreadStart(recvMsg));
        clientRecvThread.IsBackground = true;
        clientRecvThread.Start();
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

    void Update()
    {
        messageArea.text = messages;
    }

    IEnumerator clientSend()
    {
         if (!disconnected)
        {
            data = "{\"user\" : \"" + this.sender + "\", \"recipient\": \"\", \"message\": \"\", \"init\": \"1\", \"disconnect\": \"0\"}";
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
                        data = "{\"user\" : \"" + this.sender + "\", \"recipient\": \"\", \"message\": \"\", \"init\": \"0\", \"disconnect\": \"1\"}";
                        sw.WriteLine(data);
                        disconnected = true;
                        break;
                    }
                    //User has sent a message
                    else
                    {
                        data = "{\"user\" : \"" + this.sender + "\", \"recipient\": \"" + this.recipient + "\", \"message\": \"" + message + "\", \"init\": \"0\", \"disconnect\": \"0\"}";
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
            byte[] bb = new byte[100];
            int k = s.Read(bb, 0, 100);     //Reads in a stream of bytes

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
                    messages += serverMessage + "\n";
                    initialized = true;
                }
                else
                {
                    messages += "<b><color=blue>" + this.recipient + ":</color></b> " + serverMessage + "\n";
                }

                serverMessage = "";
            }
        }
    }

    private void OnApplicationQuit()
    {
        disconnected = true;
        s.Close();
        sw.Close();
    }
}
