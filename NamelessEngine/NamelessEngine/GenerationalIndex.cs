using System.Collections.Generic;

namespace NamelessEngine
{
    // this is so we can change the int type easily
    // (we may realize that u32 will better in the future)
    using GenerationalInt = System.UInt16;

    public readonly struct GenerationalIndex
    {
        public readonly GenerationalInt Index;
        public readonly GenerationalInt Generation;

        public GenerationalIndex(GenerationalInt index, GenerationalInt generation)
        {
            Index = index;
            Generation = generation;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return this == (GenerationalIndex)obj;
        }

        public GenerationalIndex CreateNewGeneration() => new GenerationalIndex(Index, (GenerationalInt)(Generation + 1));

        public static bool operator ==(GenerationalIndex a, GenerationalIndex b) => a.Index == b.Index && a.Generation == b.Generation;
        public static bool operator !=(GenerationalIndex a, GenerationalIndex b) => a.Index != b.Index || a.Generation != b.Generation;
    }

    public sealed class GenerationalIndexAllocator
    {
        readonly struct AllocatorEntry
        {
            public readonly bool IsLive;
            public readonly GenerationalInt Generation;

            public AllocatorEntry(bool isLive, GenerationalInt generation)
            {
                IsLive = isLive;
                Generation = generation;
            }

            public AllocatorEntry Deactivate() => new AllocatorEntry(false, Generation);
            public AllocatorEntry Activate() => new AllocatorEntry(true, (GenerationalInt)(Generation + 1));
        }

        readonly List<AllocatorEntry> _entries = new List<AllocatorEntry>();
        readonly Queue<GenerationalInt> _freeIndicies = new Queue<GenerationalInt>();

        public bool IsLive(GenerationalIndex handle)
        {
            return _entries[handle.Index].Generation == handle.Generation &&
            _entries[handle.Index].IsLive;
        }

        public GenerationalIndex Allocate()
        {
            if (_freeIndicies.Count > 0)
            {
                var index = _freeIndicies.Dequeue();
                _entries[index] = _entries[index].Activate();

                return new GenerationalIndex(index, _entries[index].Generation);
            }
            else
            {
                _entries.Add(new AllocatorEntry(true, 0));
                return new GenerationalIndex((GenerationalInt)(_entries.Count - 1), 0);
            }
        }

        public bool Deallocate(GenerationalIndex handle)
        {
            if (IsLive(handle))
            {
                _entries[handle.Index] = _entries[handle.Index].Deactivate();
                _freeIndicies.Enqueue(handle.Index);
                return true;
            }

            return false;
        }
    }

    public sealed class GenerationalIndexArray<T> where T : struct
    {
        readonly List<GenerationalIndex?> _indicies = new List<GenerationalIndex?>();
        readonly List<T> _items = new List<T>();

        public void Set(GenerationalIndex handle, T obj)
        {
            while (_indicies.Capacity <= handle.Index)
                _indicies.Add(null);

            if (_indicies[handle.Index] is GenerationalIndex id && id.Generation > handle.Generation)
                return;

            _indicies[handle.Index] = handle;
            _items[handle.Index] = obj;
        }

        public void Remove(GenerationalIndex handle)
        {
            if (handle.Index < _indicies.Count)
                _indicies[handle.Index] = null;
        }

        public T? Get(GenerationalIndex handle)
        {
            if (handle.Index >= _indicies.Capacity) return null;

            if (_indicies[handle.Index] is GenerationalIndex id && id == handle)
                return new T?(_items[handle.Index]);

            return null;
        }
    }
}
