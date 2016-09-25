using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Launcher.Manager.Interfaces
{
    public interface IConfiguration : ISingleton
    {
        string this[string Section] { get; set; }
        int SectionCount { get; }

        void LoadDefaults();

        string GetSection(int i);
    }
}
