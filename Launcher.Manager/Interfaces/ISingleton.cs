using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Launcher.Manager.Interfaces
{
    public interface ISingleton
    {
        void Initalize();
        void Destroy();
    }
}
