using System.Threading;

namespace tcpTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string email = "victor2";
            
            Client client = new Client(email);

            Thread clientSendThread = new Thread(new ThreadStart(client.send));
            Thread clientRevcThread = new Thread(new ThreadStart(client.recv));

            try{
                clientSendThread.Start();
                clientRevcThread.Start();
                clientSendThread.Join();
                clientRevcThread.Join();
                
            } finally {
                client.Close();
            } 
        }
    }
}
   