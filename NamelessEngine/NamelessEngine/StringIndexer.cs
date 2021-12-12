using System;
using System.Collections.Generic;

namespace NamelessEngine
{
    public sealed class StringIndexer : IModule, IIndexer<string>
    {
        readonly Dictionary<string, int> _table = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        public string Name { get; }

        public StringIndexer(string name, string[] keys)
        {
            Name = name;

            var index = 0;
            foreach (var key in keys)
                _table.Add(key, index++);
        }

        public int GetIndex(string key)
        {
            if(_table.TryGetValue(key, out int value))
                return value;

            return 0;
        }
    }
}
