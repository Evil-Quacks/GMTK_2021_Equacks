using System;
using MemoryEvents;
using UnityEngine;
using Utilities.Logger;

public class MemoryController
{
    MemoryView memoryPrefab;
    MemoryView memory;
    Memory memoryModel;

    public MemoryController(MemoryView prefab, Memory model, Transform memorySpawn)
    {
        this.memoryPrefab = prefab;
        this.memoryModel = model;

        Subscribe();
        Log.Created("MC");
        InitializeMemory(memorySpawn);
    }

    void Subscribe()
    {
        if (EventManager.instance != null)
        {
            EventManager.instance.AddListener<DespawnMemory>(OnDespawnMemory);
        }
    }

    void InitializeMemory(Transform memorySpawn)
    {
        if (memorySpawn != null && memory == null)
        {
            memory = GameObject.Instantiate(memoryPrefab, memorySpawn);
            // EventManager.instance.QueueEvent(new);
            memory.Initialize(memoryModel, null, 0f, 0.5f);
        }
    }

    void OnDespawnMemory(DespawnMemory @event)
    {
        if (memory.Equals(@event.memory))
        {
            Log.Message("MemoryController", $"Despawning this memory: {@event.memory}");
            memory.gameObject.SetActive(false);
        }
    }
}
