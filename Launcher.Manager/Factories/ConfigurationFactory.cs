using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace Launcher.Manager.Factories
{
    using Interfaces;
    using Abstracts;

    public class ConfigurationFactory<T> : ASingleton<ConfigurationFactory<T>>, IComponent where T : IConfiguration
    {
        private Dictionary<string, IConfiguration> Configurations;

        private string GetConfigurationPath()
        {
            return Path.Combine(Environment.CurrentDirectory, "Configuration", string.Format("{0}.xml", typeof(T).Name));
        }

        public string Name { get { return "ConfigurationFactory"; } }
        public bool Enable()
        {
            try
            {
                IConfiguration Configuration = SingletonFactory.GetSingleton<T>();
                string ConfigurationPath = GetConfigurationPath();

                if(!File.Exists(ConfigurationPath))
                {
                    Configuration.LoadDefaults();
                    Disable();
                    return false;
                }
                else
                {
                    using (FileStream Stream = File.Open(ConfigurationPath, FileMode.Open))
                    {
                        XElement Element = XElement.Load(Stream);
                        foreach(XElement PropertyElement in Element.Elements())
                        {
                            string Section = PropertyElement.Attribute(XName.Get("Section")).Value;
                            string Value = PropertyElement.Attribute(XName.Get("Value")).Value;

                            Configuration[Section] = Value;
                        }
                    }
                    return true;
                }
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
                IConfiguration Configuration = SingletonFactory.GetSingleton<T>();
                string ConfigurationPath = GetConfigurationPath();

                if (!Directory.Exists(Path.GetDirectoryName(ConfigurationPath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(ConfigurationPath));

                using (FileStream Stream = File.Open(ConfigurationPath, FileMode.OpenOrCreate))
                {
                    XElement RootElement = new XElement(XName.Get("root"));
                    for(int i = 0; i < Configuration.SectionCount; i++)
                    {
                        XElement PropertyElement = new XElement(XName.Get("Property"));

                        string Section = Configuration.GetSection(i);
                        PropertyElement.SetAttributeValue(XName.Get("Section"), Section);
                        PropertyElement.SetAttributeValue(XName.Get("Value"), Configuration[Section]);
                        RootElement.Add(PropertyElement);
                    }
                    RootElement.Save(Stream);
                }
                return true;
            }
            catch(Exception e)
            {
                LogFactory.GetLog(this).LogFatal(e);
                return false;
            }
        }

        public override void Initalize()
        {
            Configurations = new Dictionary<string, IConfiguration>();
        }

        public override void Destroy()
        {

        }

        public abstract class ConfigurationBase : ASingleton<T>, IConfiguration
        {
            private Dictionary<string, string> Values;
            public override void Initalize()
            {
                Values = new Dictionary<string, string>();
            }

            public override void Destroy()
            {
                
            }

            public string this[string Section]
            {
                get
                {
                    return Values.ContainsKey(Section) ? Values[Section] : string.Empty;
                }
                set
                {
                    if (Values.ContainsKey(Section))
                        Values[Section] = value;
                    else
                        Values.Add(Section, value);
                }
            }

            public int SectionCount
            {
                get
                {
                    return Values.Count;
                }
            }

            public string GetSection(int i)
            {
                string[] Keys = new string[Values.Count];
                Values.Keys.CopyTo(Keys, 0);

                return i < 0 || i >= Values.Count ? string.Empty : Keys[i];
            }

            public abstract void LoadDefaults();
        }
    }
}
