using System.Collections;
using System.Collections.Generic;
using System;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.LowLevel;


[RequireComponent (typeof(SpriteRenderer))]
public class memoryView : MonoBehaviour
{
    [SerializeField]
    AudioSource audio_mem;
    [SerializeField]
    SpriteRenderer spr_mem;
    [SerializeField]
    BoxCollider2D col_mem;

    memory memoryToSpawn;
    AudioClip sfx_mem;

    bool fadeSprite;

    //! PUBLIC FOR TESTING ONLY 
    public float sfxVolume;
    public float fadeSpeed;
    //!-------------------------

    public void Initalize(memory memor, AudioClip sfx, float sfxVol, float fadeSpeed)
    {
        memoryToSpawn = memor;
        sfx_mem = sfx;
        sfxVolume = sfxVol;
    }

    private void FixedUpdate() {
        if(fadeSprite)
        {
            AdjustAlpha();
        }
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
        if(!audio_mem.isPlaying)
        {
            audio_mem.PlayOneShot(sfx_mem, sfxVolume);
        }
    }

    private void RemoveOneAttempt()
    {
        //Remove one attempt from the memory & update visuals
        if(memoryToSpawn.attempts > 0)
        {
            memoryToSpawn.attempts -= 1;

            if(memoryToSpawn.attempts <= 0)
            {
               fadeSprite = true;
            }
        }
    }

    private void AdjustAlpha()
    {
        Color currentColor = spr_mem.color;
        float newAlpha = currentColor.a - (Mathf.Exp(fadeSpeed) * Time.deltaTime);
        spr_mem.color = new Color(currentColor.r, currentColor.g, currentColor.b, newAlpha);
        if(newAlpha <= 0.3f)
        {
            fadeSprite = false;
        }
    }

    [ContextMenu("TEST FADE")]
    public void FadeTest()
    {
        fadeSprite = true;
    }


}
