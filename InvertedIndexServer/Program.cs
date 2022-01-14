using System.Net;
using System.Net.Sockets;

namespace InvertedIndexServer
{
    class Program
    {
        const int port = 9001;
        static TcpListener listener;
        static void Main(string[] args)
        {
            try
            {
                listener = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
                listener.Start();
                Console.WriteLine("Waiting for connections...");

                while (true)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    ClientObject clientObject = new ClientObject(client);

                    // create new thread for a new client
                    Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (listener != null)
                    listener.Stop();
            }       
        }
    }
}
