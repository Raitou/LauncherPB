using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Launcher.Server
{
    using Manager.Factories;

    public class UpdateListConfiguration : ConfigurationFactory<UpdateListConfiguration>.ConfigurationBase
    {
        private List<File> UpdateList;
        private static object syncLock = new object();

        public override void LoadDefaults()
        {
           // this["Filename"] = "Hash|Size";
        }

        public static File[] Files
        {
            get
            {
                lock (syncLock)
                {
                    if (Instance.UpdateList == null)
                    {
                        Instance.UpdateList = new List<File>();
                        for (int i = 0; i < Instance.SectionCount; i++)
                        {
                            string Section = Instance.GetSection(i);
                            string Value = Instance[Section].Split('|')[0];
                            long Size = long.Parse(Instance[Section].Split('|')[1]);

                            Instance.UpdateList.Add(new File { Filename = Section, Hash = Value, Size = Size });
                        }
                    }
                    return Instance.UpdateList.ToArray();
                }
            }
        }
    }
}