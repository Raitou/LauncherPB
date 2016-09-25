using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Launcher.Server
{
    using Manager.Factories;
    using Manager.EventArgs;
    using Manager.Enums;

    class Program
    {
        static void Main(string[] args)
        {
            LogFactory.OnWrite += LogFactory_ConsoleWrite;
            if(!EnableComponents())
            {
                LogFactory.GetLog("Main").LogWarning("Cannot start the server!");
            }
            else
            {
                LogFactory.GetLog("Main").LogSuccess("Server enabled successfully!");
            }
            Console.Read();
        }

        private static bool EnableComponents()
        {
            if (!ComponentFactory.EnableComponent(typeof(ConfigurationFactory<UpdateListConfiguration>)))
                return false;
            else if (!ComponentFactory.EnableComponent(typeof(ConfigurationFactory<ServerConfiguration>)))
                return false;
            else if (!ComponentFactory.EnableComponent<LauncherServer>())
                return false;
            else
            {
                LogFactory.GetLog("Main").LogInfo("Loaded {0} files!", UpdateListConfiguration.Files.Length);
                return true;
            }
        }

        private static void LogFactory_ConsoleWrite(object sender, LogWriteEventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("[{0}] ", DateTime.Now);

            switch(e.Type)
            {
                case LogType.Information:
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    break;
                case LogType.Success:
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    break;
                case LogType.Warning:
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    break;
                case LogType.Error:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    break;
                case LogType.Fatal:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
            }
            Console.Write("<{0}>", e.Name);

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(": {0}", e.Message);
        }
    }
}
