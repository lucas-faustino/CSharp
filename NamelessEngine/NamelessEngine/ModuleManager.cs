namespace NamelessEngine
{
    public interface IModule
    {
        string Name { get; }
    }

    public interface IModuleManager
    {
        T GetByIndex<T>(int index) where T : class, IModule;
        T GetByName<T>(string name) where T : class, IModule;
    }

    public sealed class ModuleManager : IModuleManager
    {
        readonly IModule[] _modules;

        public ModuleManager(IModule[] modules)
        {
            _modules = modules;
        }

        public T GetByIndex<T>(int index) where T : class, IModule
        {
            return _modules[index] as T;
        }

        public T GetByName<T>(string name) where T : class, IModule
        {
            foreach(var m in _modules)
            {
                if (m.Name.Equals(name, System.StringComparison.OrdinalIgnoreCase))
                    return m as T;
            }

            return default;
        }
    }
}
