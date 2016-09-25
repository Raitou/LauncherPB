using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using System.IO;
using System.Security.Cryptography;


namespace Launcher.Client
{
    using Manager.Abstracts;
    public struct File
    {
        public string Filename;
        public string Hash;
        public long Size;
    };

    public class LauncherClientHandles : ASingleton<LauncherClient>
    {
        public static event EventHandler InvalidClientVersion;
        public static event EventHandler FileInfoReceived;
        public static event EventHandler EndFileInfo;
        public static event EventHandler EndFileProcess;
        public static event EventHandler FileProcessed;

        public static bool EndProcess { get; private set; }
        public static string SiteLink { get; private set; }
        public static string RegisterLink { get; private set; }
        public static string UpdateLink { get; private set; }
        public static string NewsLink { get; private set; }
        public static string GameExecutable { get; private set; }
        public static string GameParameters { get; private set; }
        public static string GameWorkingDirectory { get; private set; }
        public static int FilesProcessed { get; private set; }

        private List<File> List;
        private List<string> UpdateList;

        public static int TotalFileCount
        {
            get
            {
                return Instance.List.Count;
            }
        }

        public static int FileCount
        {
            get
            {
                return Instance.UpdateList.Count;
            }
        }

        public static string[] Updates
        {
            get
            {
                return Instance.UpdateList.ToArray();
            }
        }

        public override void Initalize()
        {
            UpdateList = new List<string>();
            List = new List<File>();
        }

        public override void Destroy()
        {

        }

        protected void HandleClientVersion()
        {
            LauncherClient.SendClientVersion();
        }

        protected void HandleInvalidVersion()
        {
            App.SyncCallback(() => InvalidClientVersion(null, EventArgs.Empty));
        }

        protected void HandleServerInformation(LauncherPacket Packet)
        {
            SiteLink = Packet.ReadString();
            RegisterLink = Packet.ReadString();
            NewsLink = Packet.ReadString();
            UpdateLink = Packet.ReadString();
            GameExecutable = Packet.ReadString();
            GameParameters = Packet.ReadString();
            GameWorkingDirectory = Packet.ReadString();
        }

        protected void HandleFileList(LauncherPacket Packet)
        {
            string File = Packet.ReadString();
            string Hash = Packet.ReadString();
            long Size = Packet.ReadLong();

            List.Add(new Client.File { Filename = File, Hash = Hash, Size = Size });
            App.SyncCallback(() => FileInfoReceived(null, EventArgs.Empty));
            LauncherClient.SendListRequest();
        }

        protected void HandleEndList()
        {
            App.SyncCallback(() => EndFileInfo(null, EventArgs.Empty));
            Task.Factory.StartNew(ProcessList);

            EndProcess = true;
            LauncherClient.Disconnect();
        }

        private void ProcessList()
        {
            foreach (File FileInfo in List)
            {
                string Filename = Path.Combine(GameWorkingDirectory, FileInfo.Filename);

                if (!System.IO.File.Exists(Filename)) //1ª fase
                    UpdateList.Add(FileInfo.Filename);
                else
                {
                    long FileSize = new FileInfo(Filename).Length;
                    if (FileSize != FileInfo.Size)
                        UpdateList.Add(FileInfo.Filename); //2ª fase
                    else if (CalculateHash(Filename) != FileInfo.Hash)
                        UpdateList.Add(FileInfo.Filename); //3º fase
                    FilesProcessed++;
                    App.SyncCallback(() => FileProcessed(null, EventArgs.Empty));
                }
            }
            App.SyncCallback(() => EndFileProcess(null, EventArgs.Empty));
        }

        string CalculateHash(string Filename)
        {
            using (MD5 hash = MD5.Create())
            {
                using (BufferedStream Stream = new BufferedStream(System.IO.File.OpenRead(Filename), 16 * 1024 * 1024))
                {
                    return BitConverter.ToString(hash.ComputeHash(Stream)).Replace('-', '.');
                }
            }
        }
    }
}