using System.Collections;
using System.Collections.Generic;
using System;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.LowLevel;

[RequireComponent (typeof(AudioSource))]
[RequireComponent (typeof(SpriteRenderer))]
public class memorySpawner : MonoBehaviour
{
    memory memoryToSpawn;

    private void Start() {
        //subscribe to EM
    }

    public void Initalize(memory memor)
    {
        memoryToSpawn = memor;
    }

    private void SpawnMemory(bool toSpawn)
    {
        if(toSpawn)
        {
            //reveal it
        }
        else
        {
            //hide it
        }
    }

    private void ResetAttempts()
    {
        //Set memorytospawn attempts to default attempts
    }

    private void PlayNote()
    {
        //Play the audio file from the audio source.
    }

    private void RemoveOneAttempt()
    {
        //Remove one attempt from the memory & update visuals
    }


}
