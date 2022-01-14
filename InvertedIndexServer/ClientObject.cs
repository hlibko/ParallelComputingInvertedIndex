using System.Text;
using System.Net.Sockets;

namespace InvertedIndexServer
{
    internal class ClientObject
    {
        public TcpClient client;
        public ClientObject(TcpClient tcpClient)
        {
            client = tcpClient;
        }
        public void Process()
        {
            string[] directories =
                    {
                        "C:\\Users\\Gleb\\Desktop\\IndexApp\\datasets\\aclImdb\\test\\neg",
                        "C:\\Users\\Gleb\\Desktop\\IndexApp\\datasets\\aclImdb\\test\\pos",
                        "C:\\Users\\Gleb\\Desktop\\IndexApp\\datasets\\aclImdb\\train\\neg",
                        "C:\\Users\\Gleb\\Desktop\\IndexApp\\datasets\\aclImdb\\train\\pos",
                        "C:\\Users\\Gleb\\Desktop\\IndexApp\\datasets\\aclImdb\\train\\unsup",
                    };
            int NUNMBER_THREADS = directories.Length;

            Dictionary[] dictionaryThread = new Dictionary[NUNMBER_THREADS];
            Thread[] myThread = new Thread[NUNMBER_THREADS];

            NetworkStream stream = null;
            try
            {
                stream = client.GetStream();
                byte[] data = new byte[64];
                while (true)
                {
                    // get message from server
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (stream.DataAvailable);

                    string message = builder.ToString();
                    Console.WriteLine(message);

                    // send message to server
                    for (int i = 0; i < NUNMBER_THREADS; i++)
                    {
                        dictionaryThread[i] = new Dictionary(directories[i], message);
                        myThread[i] = new Thread(dictionaryThread[i].Search);
                        myThread[i].Start();
                    }

                    for (int i = 0; i < NUNMBER_THREADS; i++)
                    {
                        myThread[i].Join();
                    }

                    Dictionary<string, string> result = new Dictionary<string, string>();
                    for (int i = 0; i < NUNMBER_THREADS; i++)
                    {
                        var searchResult = dictionaryThread[i].GetResult();

                        if (searchResult != null)
                        {
                            result.Add(directories[i], string.Join(", ", searchResult));
                            //Console.WriteLine($"The given key '{message}' was found in directory: '{directories[i]}' in files:\n{string.Join("\n", searchResult)}\n");
                        }
                        else
                        {
                            result.Add(directories[i], "Not Found");
                            //Console.WriteLine($"The given key '{message}' was NOT FOUND in directory: '{directories[i]}'\n");
                        }
                    }
                    data = Encoding.Unicode.GetBytes(string.Join(" \n ", result));
                    stream.Write(data, 0, data.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (stream != null)
                    stream.Close();
                if (client != null)
                    client.Close();
            }
        }
    }
}