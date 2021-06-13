using EnvioEvents;
using UnityEngine;

public class MemorySpawner : MonoBehaviour
{
    [SerializeField]
    Memory memory;

    // Start is called before the first frame update
    void Start()
    {
        if (EventManager.instance != null)
        {
            EventManager.instance.QueueEvent(new MemorySpawnerCreated(this.transform, memory));
        }
    }
}
