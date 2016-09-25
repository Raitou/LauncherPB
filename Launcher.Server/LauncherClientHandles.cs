using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Launcher.Server
{
    public struct File
    {
        public string Filename;
        public string Hash;
        public long Size;
    };

    public class LauncherClientHandles
    {
        protected LauncherClient Client;
        private bool ClientVersion = false;
        private int ListPosition = 0;
        private File[] List;

        protected void HandleClientVersion(LauncherPacket Packet)
        {
            int Major = Packet.ReadInt();
            int Minor = Packet.ReadInt();

            if (Major != int.Parse(ServerConfiguration.MajorVersion) || Minor != int.Parse(ServerConfiguration.MinorVersion))
            {
                Client.SendInvalidVersion();
                Client.Disconnect();
            }
            else
            {
                ClientVersion = true;
                Client.SendInformation();
                HandleSendList();
            }
        }

        protected void HandleSendList()
        {
            if (!ClientVersion)
            {
                Client.Disconnect();
            }
            else
            {
                if (List == null)
                    List = UpdateListConfiguration.Files;

                if (List.Count() == 0 || ListPosition == List.Count())
                {
                    Client.SendEndList();
                }
                else
                {
                    File File = List[ListPosition++];
                    Client.SendFileInfo(File.Filename, File.Hash, File.Size); //??
                }
            }
        }

        protected void HandleInformationRequest()
        {
            if(!ClientVersion)
            {
                Client.Disconnect();
            }
            else
            {
                Client.SendInformation();
            }
        }
    }
}