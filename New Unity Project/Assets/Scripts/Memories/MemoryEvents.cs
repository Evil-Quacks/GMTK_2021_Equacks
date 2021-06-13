using UnityEngine;

namespace MemoryEvents
{
    public struct SpawnMemory : iEvent
    {
        public Transform memoryTransform;

        public SpawnMemory(Transform memoryTransformReference)
        {
            memoryTransform = memoryTransformReference;
        }
    }

    public struct DespawnMemory : iEvent
    {
        public readonly MemoryView memory;

        public DespawnMemory(MemoryView memory)
        {
            this.memory = memory;
        }
    }
}
