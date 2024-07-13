using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal interface IServerFileReciever
    {
        public void ServerStart();
        public void GetStatus();
        public void WaitForConnection();
        public void Stop();
        public void RemoveClient(TcpClient client);
        public void Send(TcpClient client);
        public void Recieve(TcpClient client);
    }
}
