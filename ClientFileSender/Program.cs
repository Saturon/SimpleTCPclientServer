using System.Net.Sockets;
using System.Text;

// Создаем TCP-клиент
Console.WriteLine("Client starting \n Input ip or localhost will be used");
string ip = Console.ReadLine();
if (ip == "")
{
    ip = "127.0.0.1";
}
TcpClient client = new TcpClient(ip, 8888);
// Путь к файлу, который хотим отправить
string filePath = Console.ReadLine();
if (File.Exists(filePath)) 
{
    Console.WriteLine("File doesnt exists");
    Environment.Exit(0);
}else if (filePath.Length == 0)
{
    filePath = @"C:\\Users\\msk-2\\Desktop\123123123.txt";
}
string format;

if (filePath.LastIndexOf('.') != -1)
    format = filePath.Substring(filePath.LastIndexOf('.'));
else
    format = "";

// Читаем файл в байтовый массив
byte[] fileData = File.ReadAllBytes(filePath);

// Получаем сетевой поток клиента
NetworkStream stream = client.GetStream();

// Отправляем длину файла
stream.Write(BitConverter.GetBytes(fileData.Length), 0, 4);

// Отправляем формат
stream.Write(fileData, 0, fileData.Length);
Console.WriteLine(format);
// Читаем формат в байтовый массив
byte[] formatData = Encoding.ASCII.GetBytes(format);

// Отправляем длину формат
stream.Write(BitConverter.GetBytes(formatData.Length), 0, 4);

// Отправляем формат
stream.Write(formatData, 0, formatData.Length);
// Закрываем соединение
client.Close();