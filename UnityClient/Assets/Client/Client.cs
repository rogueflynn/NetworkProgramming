using System;
using System.IO;
using System.Net.Sockets;
using UnityEngine;
using System.Threading;

class Client
{
    Stream s;
    string sender, recipient, data;
    TcpClient client = new TcpClient();
    public bool disconnected;
    public bool serverStarted;
    LiteralEscape literal;

    public Client(string sender, string recipient)
    {
        data = "";
        this.sender = sender;
        this.recipient = recipient;
        literal = new LiteralEscape();
        try
        {
            client.Connect("192.168.1.133", 12345);
            s = client.GetStream();
            disconnected = false;
            serverStarted = true;
        } catch(SocketException sx) {
            disconnected = true;
            serverStarted = false;
        }
    }

    public void send()
    {
        if (!disconnected)
        {
            StreamWriter sw = new StreamWriter(s);
            //initialize the user on the server
            data = "{\"user\" : \"" + this.sender + "\", \"recipient\": \"\", \"message\": \"\", \"init\": \"1\", \"disconnect\": \"0\"}";
            sw.WriteLine(data);
            Debug.Log("Made it to send method");
            sw.AutoFlush = true;
            while (!disconnected)
            {
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
                    }
                }
                Thread.Sleep(100);
            }
        }
    }

    public void recv()
    {
        string serverMessage = "";
        
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
                serverMessage = "";
            }
            Thread.Sleep(100);
        }
    }

    public void Close()
    {
        if (serverStarted)
        {
            s.Close();
            client.Close();
        }
    }
}
