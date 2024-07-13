using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    class FileReciever : IServerFileReciever
    {
        public TcpListener server;
        public Dictionary<int, TcpClient> list_clients = new Dictionary<int, TcpClient>();
        public int count = 0;
        public int clientCount = 0;
        public int maxClients = 3;
        string bufferPath = Path.GetFullPath(@"D:\filesender\recieved_files\");

        public FileReciever(string ip_adress, int port)
        {
            this.server = new TcpListener(IPAddress.Parse(ip_adress), port);
            string bufferPath = Path.GetFullPath(@"D:\filesender\recieved_files\");
        }
        public FileReciever(string ip_adress, int port, string buffer_path)
        {
            this.server = new TcpListener(IPAddress.Parse(ip_adress), port);
            string bufferPath = Path.GetFullPath(buffer_path);
        }
        public void ServerStart()
        {
            server.Start();
            if (!Directory.Exists(bufferPath))
                System.IO.Directory.CreateDirectory(bufferPath);
            Console.WriteLine("Server started. Awaiting... ");
        }
        public void Stop()
        {
            foreach (TcpClient target_client in list_clients.Values)
            {
                target_client.Close();
            }
            this.server.Stop();
        }
        public void GetStatus()
        {
            Console.WriteLine("Server still Online");
            Console.WriteLine("Currently clients: ", list_clients.Count);
            Console.WriteLine("Max clients: ", maxClients);
        }
        public void RemoveClient(TcpClient client)
        {
            client.Close();
            clientCount--;
        }
        public async void WaitForConnection()
        {
            try
            {
                TcpClient new_client = await server.AcceptTcpClientAsync();
                list_clients.Add(clientCount, new_client);
                clientCount++;
                Console.WriteLine(new_client.Client.RemoteEndPoint.ToString(), " connected");
                Recieve(new_client);
                RemoveClient(new_client);
            }
            catch (Exception e) { Console.WriteLine("ConnectionCheck error: ", e); }
        }
        public async void Send(TcpClient client)
        {
            var stream = client.GetStream();
            var response = new List<byte>();
            int bytesRead = 10;
            while (true)
            {
                while ((bytesRead = stream.ReadByte()) != '\n')
                {
                    response.Add((byte)bytesRead);
                }
                var word = Encoding.UTF8.GetString(response.ToArray());
                Console.WriteLine($"Smth sended");
                var answer1 = "answered";
                answer1 += '\n';
                await stream.WriteAsync(Encoding.UTF8.GetBytes(answer1));
                response.Clear();
            }
        }
        public void Recieve(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            // Читаем длину файла
            byte[] lengthBytes = new byte[4];
            stream.Read(lengthBytes, 0, 4);
            int fileLength = BitConverter.ToInt32(lengthBytes, 0);

            // Читаем файл
            byte[] fileData = new byte[fileLength];
            int totalBytesRead = 0;
            while (totalBytesRead < fileLength)
            {
                int bytesRead = stream.Read(fileData, totalBytesRead, fileLength - totalBytesRead);
                if (bytesRead == 0)
                    break;
                totalBytesRead += bytesRead;
            }
            // Читаем длину файла
            byte[] lengthFormat = new byte[4];
            stream.Read(lengthFormat, 0, 4);
            int formatLength = BitConverter.ToInt32(lengthFormat, 0);

            // Читаем файл
            byte[] formatData = new byte[formatLength];
            int totalBytesRead2 = 0;
            while (totalBytesRead2 < formatLength)
            {
                int bytesRead = stream.Read(formatData, totalBytesRead2, formatLength - totalBytesRead2);
                if (bytesRead == 0)
                    break;
                totalBytesRead2 += bytesRead;
            }
            // Сохраняем файл на диск
            string outputFilePath = bufferPath + "file_" + count + Encoding.ASCII.GetString(formatData);
            File.WriteAllBytes(outputFilePath, fileData);
            count++;

            Console.WriteLine($"Файл сохранен: {outputFilePath}");
        }
    }
}
