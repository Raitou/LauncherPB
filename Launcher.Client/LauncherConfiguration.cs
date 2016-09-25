using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Launcher.Client
{
    using Manager.Factories;

    public class LauncherConfiguration : ConfigurationFactory<LauncherConfiguration>.ConfigurationBase
    {
        public override void LoadDefaults()
        {
            this["ServerAddress"] = "127.0.0.1";
            this["ServerPort"] = "8088"; //teste port 80 ??
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
                return Instance["ServerPort"];
            }
        }
    }
}
