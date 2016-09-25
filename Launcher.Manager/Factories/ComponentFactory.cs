using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Launcher.Manager.Factories
{
    using Abstracts;
    using Interfaces;

    public class ComponentFactory : ASingleton<ComponentFactory>
    {
        private Dictionary<string, IComponent> Components;

        public override void Initalize()
        {
            Components = new Dictionary<string, IComponent>();
        }

        public override void Destroy()
        {
            foreach (IComponent Component in Components.Values)
            {
                if (Component.Disable())
                {
                    LogFactory.GetLog(Component).LogSuccess("Component disabled successfully!");
                }
                else
                {
                    LogFactory.GetLog(Component).LogWarning("Failed to disable this component!");
                }
            }
            Components.Clear();
        }

        private static bool EnableComponent(string ID, IComponent Component)
        {
            if (Instance.Components.ContainsKey(ID))
            {
                return true;
            }
            else if (!Component.Enable())
            {
                LogFactory.GetLog(Component).LogWarning("Failed to enable this component!");
                return false;
            }
            else
            {
                LogFactory.GetLog(Component).LogSuccess("Component enabled successfully!");
                Instance.Components.Add(ID, Component);
                return true;
            }
        }

        public static  bool EnableComponent(IComponent Component)
        {
            string ID = Component.GetType().Name + "|" + Component.GetHashCode();
            return EnableComponent(ID, Component);
        }

        public static bool EnableComponent(Type ComponentType)
        {
            if (typeof(IComponent).IsAssignableFrom(ComponentType))
            {
                string ID = ComponentType.FullName;
                IComponent Component = (IComponent)(typeof(ISingleton).IsAssignableFrom(ComponentType) ? SingletonFactory.GetSingleton(ComponentType) : Activator.CreateInstance(ComponentType));

                return EnableComponent(ID, Component);
            }
            throw new NotSupportedException(string.Format("{0} type not implements IComponent type!", ComponentType.FullName));
        }

        public static bool EnableComponent<T>() where T : IComponent
        {
            return EnableComponent(typeof(T));
        }

        private static bool DisableComponent(string ID)
        {
            if (!Instance.Components.ContainsKey(ID))
            {
                return false;
            }
            else
            {
                IComponent Component = Instance.Components[ID];
                if (Component.Disable())
                {
                    LogFactory.GetLog(Component).LogSuccess("Component disabled successfully!");
                    Instance.Components.Remove(ID);
                    return true;
                }
                else
                {
                    LogFactory.GetLog(Component).LogWarning("Failed to disable this component!");
                    return false;
                }
            }
        }

        public static bool DisableComponent(IComponent Component)
        {
            string ID = Component.GetType().Name + "|" + Component.GetHashCode();
            return DisableComponent(ID);
        }

        public static bool DisableComponent(Type ComponentType)
        {
            if (typeof(IComponent).IsAssignableFrom(ComponentType))
            {
                string ID = ComponentType.FullName;
                return DisableComponent(ID);
            }
            throw new NotSupportedException(string.Format("{0} type not implements IComponent type!", ComponentType.FullName));
        }

        public static bool DisableComponent<T>() where T : IComponent
        {
            return DisableComponent(typeof(T));
        }
    }
}