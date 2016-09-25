using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace Launcher.Server
{
    using Manager.Interfaces;
    using Manager.Factories;
    using Manager.Abstracts;

    public class LauncherServer : ASingleton<LauncherServer>, IComponent
    {
        private Socket ServerSocket;
        private IPEndPoint EndPoint;
        private Thread ServerThread;
        private List<LauncherClient> Clients;

        public override void Initalize()
        {
            ServerThread = new Thread(ServerCallback);
            Clients = new List<LauncherClient>();
        }

        public override void Destroy()
        {
            
        }

        public string Name {  get { return "LauncherServer"; } }
        public bool Enable()
        {
            try
            {
                EndPoint = new IPEndPoint(IPAddress.Parse(ServerConfiguration.ServerAddress), int.Parse(ServerConfiguration.ServerPort));
                ServerSocket = new Socket(EndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                ServerSocket.Bind(EndPoint);
                ServerSocket.Listen((int)SocketOptionName.MaxConnections);

                ServerThread.Start();
                return true;
            }
            catch(Exception e)
            {
                LogFactory.GetLog(this).LogFatal(e);
                return false;
            }
        }

        public bool Disable()
        {
            try
            {
                foreach(LauncherClient Client in Clients)
                {
                    Client.Disconnect();
                }

                if (ServerSocket.Connected) ServerSocket.Shutdown(SocketShutdown.Both);

                ServerThread.Interrupt();
                ServerSocket.Close();

                return true;
            }
            catch(Exception e)
            {
                LogFactory.GetLog(this).LogFatal(e);
                return false;
            }
        }

        private void ServerCallback()
        {
            while(true)
            {
                Socket ClientSocket = ServerSocket.Accept();
                LauncherClient Client = new LauncherClient(ClientSocket);
                Clients.Add(Client);

                Thread.Sleep(100);
            }
        }
    }
}
