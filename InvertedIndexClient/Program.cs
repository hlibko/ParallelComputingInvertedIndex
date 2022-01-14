using System.Net.Sockets;
using System.Text;

namespace TCP_Client
{
    class Program
    {
        const int port = 9001;
        const string address = "127.0.0.1";
        static void Main(string[] args)
        {
            Console.Write("Your name: ");
            string userName = Console.ReadLine();
            TcpClient client = null;
            try
            {
                client = new TcpClient(address, port);
                NetworkStream stream = client.GetStream();

                while (true)
                {
                    // send message to server
                    string message;
                    do
                    {
                        Console.Write(userName + ": ");
                        message = Console.ReadLine();
                    } while (String.IsNullOrWhiteSpace(message));

                    message = String.Format($"{message}");
                    byte[] data = Encoding.Unicode.GetBytes(message);
                    stream.Write(data, 0, data.Length);

                    // get message from server
                    data = new byte[64];
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (stream.DataAvailable);

                    message = builder.ToString();
                    Console.WriteLine($"Сервер:\n{message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                client.Close();
            }
        }
    }
}