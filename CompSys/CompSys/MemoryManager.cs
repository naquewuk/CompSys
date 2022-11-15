using System;
using System.Collections.Generic;
using System.Text;

namespace ComputingSystem
{
    class MemoryManager
    {
        public void Save(Memory mem)
        {
            memory = mem;
        }

        public Memory Allocate(Process process)
        {
            if (memory.FreeSize >= process.AddrSpace)
            {
                memory.OccupiedSize += process.AddrSpace;
                return memory;
            }
            return null;
        }

        public Memory Free(Process process)
        {
            memory.OccupiedSize -= process.AddrSpace;
            return memory;
        }

        private Memory memory;
    }
}
