using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Launcher.Manager.Interfaces
{
    public interface IComponent
    {
        string Name { get; }
        bool Enable();
        bool Disable();
    }
}
