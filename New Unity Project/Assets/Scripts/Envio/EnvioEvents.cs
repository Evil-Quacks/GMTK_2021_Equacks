using UnityEngine;
namespace EnvioEvents
{
    public struct SpawnerCreated : iEvent
    {
        public readonly Transform spawnerLoc;

        public SpawnerCreated(Transform spawnerTrans)
        {
            spawnerLoc = spawnerTrans;
        }
    }

    public struct MemorySpawnerCreated : iEvent
    {
        public readonly Transform memorySpawnerLocation;
        public readonly Memory memory;

        public MemorySpawnerCreated(Transform memorySpawnerTransform, Memory memory)
        {
            memorySpawnerLocation = memorySpawnerTransform;
            this.memory = memory;
        }
    }
}
