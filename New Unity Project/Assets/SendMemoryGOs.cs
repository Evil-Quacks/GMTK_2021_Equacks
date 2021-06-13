using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendMemoryGOs : MonoBehaviour
{
    [SerializeField]
    List<GameObject> memoryGroups = new List<GameObject>();
    void Start()
    {
        EventManager.instance.QueueEvent(new GameEvents.SendMemoryGOs(memoryGroups));
    }

}
