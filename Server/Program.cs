using System;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;
Console.WriteLine("Starting server \n Input ip or localhost will be used");
string ip = Console.ReadLine();
if (ip == "")
{
    ip = "127.0.0.1";
}
Server.FileReciever server_handle = new Server.FileReciever(ip, 8888);
server_handle.ServerStart();

while (true)
{
    server_handle.WaitForConnection();
    ReadStop(server_handle);
}

async void ReadStop(Server.FileReciever server_handle)
{
    string s = Console.ReadLine();
    if (s != null)
    {
        server_handle.Stop();
        Environment.Exit(0);
    }
}