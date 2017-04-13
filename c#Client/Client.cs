using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Net.Sockets;

namespace tcpTest
{
    class Client
    {
        Stream s;
        string data;
        string email;
        TcpClient client = new TcpClient();
        public bool disconnected;
        public bool serverStarted;

        public Client(string email)
        {
            data = "";
            this.email = email;
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
                sw.AutoFlush = true;
                data = "{\"user\" : \"" + email + "\", \"recipient\": \"victor2\", \"message\": \"\", \"init\": \"1\", \"disconnect\": \"0\"}";
                sw.WriteLine(data);
                Console.WriteLine("Made it here");
                while (!disconnected)
                {
                    string message = Console.ReadLine();
                    //message = ToLiteral(message);
                    //message = message.Remove(0, 1);
                    //int end = message.Length - 1;
                    //message = message.Remove(end, 1);

                    if (message == "exit()")
                    {
                        data = "{\"user\" : \"" + email + "\", \"recipient\": \"victor1\", \"message\": \"\", \"init\": \"0\", \"disconnect\": \"1\"}";
                        sw.WriteLine(data);
                        disconnected = true;
                        break;
                    }
                    else
                    {
                        data = "{\"user\" : \"" + email + "\", \"recipient\": \"victor1\", \"message\": \"" + message + "\", \"init\": \"0\", \"disconnect\": \"0\"}";
                        sw.WriteLine(data);
                    }
                }
            }
        }

        public void recv()
        {
            string serverMessage = "";
            
            while (!disconnected)
            {
                byte[] bb = new byte[100];
                int k = s.Read(bb, 0, 100);     //Reads in a stream of bytes

                for (int i = 0; i < k; i++)
                {
                    serverMessage += Convert.ToChar(bb[i]).ToString();
                }

                if (serverMessage != "")
                {
                    Console.WriteLine(serverMessage);
                    serverMessage = "";
                }
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

        private static string ToLiteral(string input)
        {
            using (var writer = new StringWriter())
            {
                using (var provider = CodeDomProvider.CreateProvider("CSharp"))
                {
                    provider.GenerateCodeFromExpression(new CodePrimitiveExpression(input), writer, null);
                    return writer.ToString();
                }
            }
        }
    }
}
