using System.Collections.Generic;

namespace NamelessEngine
{
    public sealed class SparseSet
    {
        readonly GenerationalIndexAllocator _allocator = new GenerationalIndexAllocator();
        readonly List<GenerationalIndex?> _sparse = new List<GenerationalIndex?>();

        public readonly List<ushort> Dense = new List<ushort>();
        public ushort Count { get; private set; } = 0;

        public bool IsInSet(GenerationalIndex handle)
        {
            // Searched element must be in range
            if (handle.Index > Dense.Count)
                return false;
            // check if it's a match
            if (_sparse[handle.Index] is GenerationalIndex id && handle == id && id.Index < Count && Dense[id.Index] == handle.Index)
                return true;
            // Not found
            return false;
        }

        public GenerationalIndex? Insert()
        {
            if(_allocator.Allocate() is GenerationalIndex handle)
            {
                // increase size if index is out of range
                while (_sparse.Count <= handle.Index)
                    _sparse.Add(null);

                while (Dense.Count <= handle.Index)
                    Dense.Add(0);

                Dense[Count] = handle.Index;
                _sparse[handle.Index] = handle;
                Count++;
                return handle;
            }

        	return null;
        }

        public GenerationalIndex? RemoveAt(GenerationalIndex handle)
        {
            if(_allocator.Deallocate(handle))
            {
                var temp = Dense[Count - 1];
                Dense[handle.Index] = temp;
                _sparse[temp] = _sparse[handle.Index];
                Count--;
                return handle;
            }

            return null;
        }

        public void Clear()
        {
            // return to allocator
            for (int i = 0; i < _sparse.Count; i++)
            {
                if (_sparse[i] is GenerationalIndex handle)
                    _allocator.Deallocate(handle);
            }

            for (int i = 0; i < Count; i++)
                _sparse[i] = null;

            Count = 0;
        }
    }
}
