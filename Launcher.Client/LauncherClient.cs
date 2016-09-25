using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.Net.Sockets;
using System.IO;

using System.Threading;
using System.Windows.Forms;

namespace Launcher.Client
{
    using Manager.Factories;
    using Manager.Interfaces;

    public class LauncherClient : LauncherClientHandles, IComponent
    {
        private Socket ClientSocket;
        private NetworkStream ClientStream;
        private IPEndPoint EndPoint;
        private Thread ClientThread;
        private static object syncLock = new object();

        public string Name {  get { return "LauncherClient"; } }

        public bool Enable()
        {
            try
            {
                EndPoint = new IPEndPoint(IPAddress.Parse(LauncherConfiguration.ServerAddress), int.Parse(LauncherConfiguration.ServerPort));
                ClientSocket = new Socket(EndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                ClientSocket.Connect(EndPoint);
                ClientStream = new NetworkStream(ClientSocket);

                ClientThread = new Thread(ClientCallback);
                ClientThread.Start();
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("ERRO {0}", e));
                LogFactory.GetLog(this).LogFatal(e);
                return false;
            }
        }

        public bool Disable()
        {
            try
            {
                if (ClientSocket.Connected) ClientSocket.Shutdown(SocketShutdown.Both);
                ClientSocket.Close();

                ClientThread.Interrupt();
            }
            catch (SocketException) { }
            catch (IOException) { }
            catch (ThreadInterruptedException) { }
            catch (Exception e)
            {
                LogFactory.GetLog(this).LogFatal(e);
                return false;
            }
            return true;
        }

        //Read callback
        private void ClientCallback()
        {
            try
            {
                while (true)
                {
                    byte[] IDBuffer = new byte[sizeof(int)];
                    if (ClientStream.Read(IDBuffer, 0, IDBuffer.Length) == 0) break;

                    byte[] SizeBuffer = new byte[sizeof(int)];
                    if (ClientStream.Read(SizeBuffer, 0, SizeBuffer.Length) == 0) break;

                    byte[] Buffer = new byte[BitConverter.ToInt32(SizeBuffer, 0)];
                    if (Buffer.Length > 0)
                    {
                        if (ClientStream.Read(Buffer, 0, Buffer.Length) == 0) break;
                    }

                    LauncherPacket Packet = new LauncherPacket(BitConverter.ToInt32(IDBuffer, 0), Buffer);
                    PacketReceived(Packet);

                    Thread.Sleep(10);
                }
            }
            catch (SocketException) { }
            catch (IOException) { }
            catch (ThreadInterruptedException) { }
            catch (Exception e)
            {
                LogFactory.GetLog(this).LogFatal(e);
            }
            Disconnect();
        }

        public static void Disconnect()
        {
            if (Disconnected != null) App.SyncCallback(() => Disconnected(null, EventArgs.Empty));
            ComponentFactory.DisableComponent<LauncherClient>();
        }

        //Read packet
        private void PacketReceived(LauncherPacket Packet)
        {
            switch(Packet.ID)
            {
                case 0x0001:
                    HandleClientVersion();
                    break;
                case 0x0002:
                    HandleInvalidVersion();
                    break;
                case 0x0004:
                    HandleFileList(Packet);
                    break;
                case 0x0006:
                    HandleServerInformation(Packet);
                    break;
                case 0x0007:
                    HandleEndList();
                    break;
            }
        }

        private void Send(LauncherPacket Packet)
        {
            lock (syncLock)
            {
                byte[] IDBuffer = BitConverter.GetBytes(Packet.ID);
                ClientStream.Write(IDBuffer, 0, IDBuffer.Length);

                byte[] SizeBuffer = BitConverter.GetBytes(Packet.Buffer.Length);
                ClientStream.Write(SizeBuffer, 0, SizeBuffer.Length);

                byte[] Buffer = Packet.Buffer;
                ClientStream.Write(Buffer, 0, Buffer.Length);
            }
        }

        public static void SendClientVersion()
        {
            LauncherPacket Packet = new LauncherPacket(0x0001);
            Packet.WriteInt(App.MajorVersion);
            Packet.WriteInt(App.MinorVersion);
            Instance.Send(Packet);
        }

        public static void SendListRequest()
        {
            LauncherPacket Packet = new LauncherPacket(0x0003);
            Instance.Send(Packet);
        }

        public static event EventHandler Disconnected;
        public static bool Connected
        {
            get
            {
                return Instance.ClientSocket != null && Instance.ClientSocket.Connected;
            }
        }
    }
}
