using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.Net.Sockets;
using System.IO;

using System.Threading;

namespace Launcher.Server
{
    using Manager.Factories;

    public class LauncherClient : LauncherClientHandles
    {
        private Socket ClientSocket;
        private NetworkStream ClientStream;
        private IPEndPoint EndPoint;
        private Thread ClientThread;
        private bool Disconnected = false;
        private object syncLock = new object();

        public LauncherClient(Socket ClientSocket)
        {
            Client = this;
            this.ClientSocket = ClientSocket;
            ClientStream = new NetworkStream(ClientSocket);

            EndPoint = (IPEndPoint)ClientSocket.RemoteEndPoint;

            ClientThread = new Thread(ClientCallback);
            ClientThread.Start();

            LogFactory.GetLog(this).LogInfo("Client <{0}> connected to the server!", EndPoint);
            SendVersionRequest();
        }

        public void Disconnect()
        {
            if (Disconnected) return;
            Disconnected = true;
            try
            {
                if (ClientSocket.Connected) ClientSocket.Shutdown(SocketShutdown.Both);
                ClientSocket.Close();

                ClientThread.Interrupt();
            }
            //catch (SocketException) { }
            //catch (IOException) { }
            catch (ThreadInterruptedException) { }
            catch (Exception e)
            {
                LogFactory.GetLog(this).LogFatal(e);
            }
            LogFactory.GetLog(this).LogInfo("Client <{0}> has been disconnected!", EndPoint);
        }

        internal void SendFileInfo(string filename, string hash, long size)
        {
            LauncherPacket Packet = new Server.LauncherPacket(0x0004);
            Packet.WriteString(filename);
            Packet.WriteString(hash);
            Packet.WriteLong(size);

            Send(Packet);
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
            //catch (SocketException) { }
            //catch (IOException) { }
            catch (ThreadInterruptedException) { }
            catch (Exception e)
            {
                LogFactory.GetLog(this).LogFatal(e);
            }
            Disconnect();
        }

        //Read packet
        private void PacketReceived(LauncherPacket Packet)
        {
            switch(Packet.ID)
            {
                case 0x0001:
                    HandleClientVersion(Packet);
                    break;
                case 0x0003:
                    HandleSendList();
                    break;
                case 0x0005:
                    HandleInformationRequest();
                    break;
                default:
                    LogFactory.GetLog(this).LogInfo("Invalid packet({1}) received from client <{0}>!", EndPoint, Packet.ID);
                    Disconnect();
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

        public void SendVersionRequest()
        {
            LauncherPacket Packet = new LauncherPacket(0x0001);
            Send(Packet);
        }

        public void SendInvalidVersion()
        {
            LauncherPacket Packet = new LauncherPacket(0x0002);
            Send(Packet);
        }

        public void SendFileInfo(string Filename, string Checksum, long Size, bool End)
        {
            LauncherPacket Packet = new LauncherPacket(0x0004);
            Packet.WriteString(Filename);
            Packet.WriteString(Checksum);
            Packet.WriteLong(Size);
            Packet.WriteBool(End);
            Send(Packet);
        }

        public void SendEndList()
        {
            LauncherPacket Packet = new LauncherPacket(0x0007);
            Send(Packet);
        }

        public void SendInformation()
        {
            LauncherPacket Packet = new LauncherPacket(0x0006);
            Packet.WriteString(ServerConfiguration.SiteLink);
            Packet.WriteString(ServerConfiguration.RegisterLink);
            Packet.WriteString(ServerConfiguration.NewsLink);
            Packet.WriteString(ServerConfiguration.UpdateLink);
            Packet.WriteString(ServerConfiguration.GameExecutable);
            Packet.WriteString(ServerConfiguration.GameParameters);
            Packet.WriteString(ServerConfiguration.GameWorkingDirectory);
            Send(Packet);
        }
    }
}
