using System;
using System.Collections;
using MemoryEvents;
using UnityEngine;
using Utilities.Logger;

[RequireComponent(typeof(SpriteRenderer))]
public class MemoryView : MonoBehaviour
{
    [SerializeField]
    AudioSource audio_mem;
    [SerializeField]
    SpriteRenderer spr_mem;
    [SerializeField]
    BoxCollider2D col_mem;

    //! ONLY SERIALIZED FOR TESTING
    [SerializeField]
    Memory memoryToSpawn;
    AudioClip sfx_mem;
    bool fadeSprite;
    int defaultAttempts;
    bool playerInRange;
    float timePressed;

    //! PUBLIC FOR TESTING ONLY 
    public float sfxVolume;
    public float fadeSpeed;
    public float timeToHoldInteractToLock;
    //!-------------------------

    IEnumerator timer;
    bool timerRunning;

    public void Initialize(Memory memory, AudioClip sfx, float sfxVol, float fadeSpeed)
    {
        memoryToSpawn = memory;
        sfx_mem = sfx;
        sfxVolume = sfxVol;
        defaultAttempts = memory.attempts;
    }

    private void Start()
    {
        Log.Message("MV", "CREATED");
        timerRunning = false;
        EventManager.instance.AddListener<SpawnMemory>(OnSpawnMemory);
    }

    private void Update()
    {
        if (playerInRange && !memoryToSpawn.levelOne)
        {
            if (Input.GetKeyDown("e"))
            {
                timer = RunTimer();
                StartCoroutine(timer);
            }
            else if ((Input.GetKeyUp("e") && timerRunning) || timePressed > timeToHoldInteractToLock)
            {
                StopTimer();
                if (timePressed >= timeToHoldInteractToLock)
                {
                    Log.Message(message: "long e press - this memory was selected");
                    SelectThisMemory();
                }
                else
                {
                    Log.Message(message: "short e press - this memory's note was played");
                    PlayNote();
                    RemoveOneAttempt();
                }
                ResetPressTime();
            }
            // else if (Input.GetKey("e"))
            // {
            //     timePressed += Time.deltaTime;
            // }
        }
        else if (memoryToSpawn.levelOne)
        {
            //This is the memory they are trying to match to - just play it if they press
            if (Input.GetKeyDown("f"))
            {
                PlayNote();
            }
        }
    }

    private void FixedUpdate()
    {
        if (fadeSprite)
        {
            AdjustAlpha();
        }

    }

    IEnumerator RunTimer()
    {
        Log.Message(message: "Starting timer");
        timerRunning = true;

        while (timerRunning)
        {
            timePressed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        yield return null;
    }

    void StopTimer()
    {
        Log.Message(message: "Stopping timer");
        timerRunning = false;
        // StopCoroutine(timer);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            Log.Message("Memory", "Player's in range! :3");
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            Log.Message("Memory", "Player's left range.. :(");
            playerInRange = false;
            ResetPressTime();
        }
    }

    private void ResetPressTime()
    {
        Log.Value($"in MemoryView.ResetPressTime()\ntimePressed: {timePressed}");
        timePressed = 0;
        Log.Value($"in MemoryView.ResetPressTime()\ntimePressed: {timePressed}");
    }

    private void SelectThisMemory()
    {
        Debug.Log("SELECT THIS ONE");
        //EventManager.instance.QueueEvent(new GameEvents.MemorySelected(memoryToSpawn.correct));
    }

    private void OnSpawnMemory(SpawnMemory @event)
    {
        //
    }

    private void SpawnMemory(bool toSpawn)
    {
        if (toSpawn)
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
        memoryToSpawn.attempts = defaultAttempts;
        spr_mem.color = new Color(spr_mem.color.r, spr_mem.color.g, spr_mem.color.b, 1);
    }

    private void PlayNote()
    {
        Debug.Log("PLAY NOTE :D");
        //Play the audio file from the audio source.
        if (audio_mem != null && !audio_mem.isPlaying)
        {
            // audio_mem.PlayOneShot(sfx_mem, sfxVolume);
        }
    }

    private void RemoveOneAttempt()
    {
        //Remove one attempt from the memory & update visuals
        if (memoryToSpawn.attempts > 0)
        {
            memoryToSpawn.attempts -= 1;

            if (memoryToSpawn.attempts <= 0)
            {
                fadeSprite = true;
            }
        }
    }

    private void AdjustAlpha()
    {
        try
        {
            Log.Message(message: "AdjustAlpha is adjusting the alpha in MemoryView");
            Color currentColor = spr_mem.color;
            float newAlpha = currentColor.a - (Mathf.Exp(fadeSpeed) * Time.deltaTime);
            spr_mem.color = new Color(currentColor.r, currentColor.g, currentColor.b, newAlpha);
            if (newAlpha <= 0.3f)
            {
                fadeSprite = false;
                EventManager.instance.QueueEvent(new DespawnMemory(this));
            }
        }
        catch (Exception ex)
        {
            Log.Error($"in MemoryView at AdjustAlpha with {ex}");
            Log.Message(message: "Set the alpha to 0f so this doesn't run ad infinitum");
            float newAlpha = 0f;

            if (newAlpha <= 0.3f)
            {
                fadeSprite = false;
            }
        }
    }

    [ContextMenu("TEST FADE")]
    public void FadeTest()
    {
        fadeSprite = true;
    }
}
