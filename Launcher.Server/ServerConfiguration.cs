using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Launcher.Server
{
    using Manager.Factories;

    public class ServerConfiguration : ConfigurationFactory<ServerConfiguration>.ConfigurationBase
    {
        public override void LoadDefaults()
        {
            this["ServerAddress"] = "127.0.0.1";
            this["ServerPort"] = "8088";
            this["MajorVersion"] = "1";
            this["MinorVersion"] = "0";
            this["SiteLink"] = "http://pb.momzgame.com/";
            this["RegisterLink"] = "http://pb.momzgame.com/";
            this["NewsLink"] = "http://pb.momzgame.com/";
            this["UpdateLink"] = "http://127.0.0.1/";
            this["GameExecutable"] = "PointBlank.exe";
            this["GameParameters"] = "";
            this["GameWorkingDirectory"] = "";
        }

        public static string ServerAddress
        {
            get
            {
                return Instance["ServerAddress"];
            }
        }

        public static string ServerPort
        {
            get
            {
                return Instance["ServerPort"]; //?
            }
        }

        public static string MajorVersion
        {
            get
            {
                return Instance["MajorVersion"];
            }
        }

        public static string MinorVersion
        {
            get
            {
                return Instance["MinorVersion"];
            }
        }

        public static string SiteLink
        {
            get
            {
                return Instance["SiteLink"];
            }
        }

        public static string RegisterLink
        {
            get
            {
                return Instance["RegisterLink"];
            }
        }

        public static string NewsLink
        {
            get
            {
                return Instance["NewsLink"];
            }
        }

        public static string UpdateLink
        {
            get
            {
                return Instance["UpdateLink"];
            }
        }

        public static string GameExecutable
        {
            get
            {
                return Instance["GameExecutable"];
            }
        }

        public static string GameParameters
        {
            get
            {
                return Instance["GameParameters"];
            }
        }

        public static string GameWorkingDirectory
        {
            get
            {
                return Instance["GameWorkingDirectory"];
            }
        }
    }
}