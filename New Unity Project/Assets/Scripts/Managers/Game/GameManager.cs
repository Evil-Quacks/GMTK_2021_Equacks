#define DEBUG

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Logger;

public class GameManager : MonoSingleton<GameManager>
{
    #region Properties

    [Header("Player Information")]
    [SerializeField]
    PlayerStats playerStats;

    [SerializeField]
    PlayerView playerPrefab;

    [SerializeField]
    MemoryView memoryPrefab;

    [Header("Camera Set Up")]
    [SerializeField]
    CameraFollowPlayer follower;

    [SerializeField]
    Vector2 cameraFollowOffset;

    [Header("Sound Set Up")]
    [Range(0, 1)]
    [SerializeField]
    float musicVolume = .5f;

    [SerializeField]
    // AudioClip musicClip;

    Vector2 initialSpawnLocation;

    [Header("Transitioners")]
    [SerializeField]
    TransitionCtrl transitioner;

    [SerializeField]
    List<Narrative> narrativeBits;

    int narrativeIndex = 0;
    
    #endregion

    #region StartUp
    public void Initialize()
    {
        //Subscribe
        Subscribe();
        Debug.Log("GM ==> CREATED");
        EvilSceneManager.instance.WakeUp();
    }


    void Subscribe()
    {
        if (EventManager.instance != null)
        {
            //Subscribe to player / game events
            EventManager.instance.AddListener<EnvioEvents.SpawnerCreated>(OnSpawnFound);
            EventManager.instance.AddListener<EnvioEvents.MemorySpawnerCreated>(OnSpawnMemory);
            EventManager.instance.AddListener<PlayerEvents.RespawnPlayer>(OnRespawnPlayer);
            EventManager.instance.AddListener<GameEvents.GameOver>(OnGameOver);
            EventManager.instance.AddListener<GameEvents.MemorySelected>(OnMemory);
            EventManager.instance.AddListener<SceneEvents.LoadedSceneRequested>(OnRequestedSceneLoaded);
            EventManager.instance.AddListener<GameEvents.StartGame>(OnStartGame);
        }

        EventManager.instance.QueueEvent(new SceneEvents.LoadedSceneRequested("StartMenu"));
    }
    #endregion

    #region Ready Scene Change

    private void OnRequestedSceneLoaded(SceneEvents.LoadedSceneRequested @event)
    {
        if(@event.SceneName == "StartMenu")
        {
            //Start Menu Loaded fade out the black screen
            transitioner.Fade(WhichTransitioner.START, GameEvents.fadeUIType.BG, null, false);
            
        }
        else if(@event.SceneName == "Game")
        {
            //Game Scene as loaded successfully...
            StartCoroutine(WaitToFadeOutToScene());
            follower.WakeUp();
        }
    }

    IEnumerator WaitToFadeOutToScene()
    {
        //Fade In Quote & Author
        transitioner.fadingFinished = false;
        transitioner.Fade(WhichTransitioner.OPEN, GameEvents.fadeUIType.TXT,null,true);
        
        yield return new WaitUntil(() => {return transitioner.fadingFinished;});

        //Wait a bit with the quote on screen
        yield return new WaitForSeconds(5);

        //Fade Out Text
        transitioner.fadingFinished = false;
        transitioner.Fade(WhichTransitioner.OPEN, GameEvents.fadeUIType.TXT, null, false);
        yield return new WaitUntil(() => {return transitioner.fadingFinished;});

        //Fade in First Narrative
        transitioner.fadingFinished = false;
        transitioner.Fade(WhichTransitioner.GOOD, GameEvents.fadeUIType.BG, narrativeBits[narrativeIndex], true);
        //Start BGM

        yield return new WaitUntil(() => {return transitioner.fadingFinished;});

        //Fade out black screen
        transitioner.fadingFinished = false;
        transitioner.Fade(WhichTransitioner.OPEN, GameEvents.fadeUIType.BG, narrativeBits[narrativeIndex], false);

        yield return new WaitUntil(() => {return transitioner.fadingFinished;});

        //Fade in Narrative Text
        transitioner.fadingFinished = false;
        transitioner.Fade(WhichTransitioner.GOOD, GameEvents.fadeUIType.TXT, narrativeBits[narrativeIndex], true);
        yield return new WaitUntil(() => {return transitioner.fadingFinished;});

        //Let text sit on screen...
        yield return new WaitForSeconds(5);

        //Fade text out...
        transitioner.fadingFinished = false;
        transitioner.Fade(WhichTransitioner.GOOD, GameEvents.fadeUIType.TXT, narrativeBits[narrativeIndex], false);
        yield return new WaitUntil(() => {return transitioner.fadingFinished;});

        //Fade out Good BG...
        transitioner.fadingFinished = false;
        transitioner.Fade(WhichTransitioner.GOOD, GameEvents.fadeUIType.BG, narrativeBits[narrativeIndex], false);
        yield return new WaitUntil(() => {return transitioner.fadingFinished;});

        StopCoroutine("WaitToFadeOutToScene");
    }

    IEnumerator WaitForQuote(string thenLoadThisScene)
    {
        yield return new WaitUntil(() => {return transitioner.fadingFinished;});
        if(thenLoadThisScene != "")
        {
            EventManager.instance.QueueEvent(new SceneEvents.LoadNextScene("Game"));
        }

        StopCoroutine("WaitForQuote");
    }
    private void OnSpawnFound(EnvioEvents.SpawnerCreated @event)
    {
        PlayerController pCtrl = new PlayerController(playerPrefab, playerStats, @event.spawnerLoc);
        initialSpawnLocation = @event.spawnerLoc.position;
        // AudioSource audioSrc = gameObject.AddComponent<AudioSource> ();
        // audioSrc.volume = musicVolume;
        // audioSrc.clip = musicClip;
        // audioSrc.Play ();
    }

    private void OnSpawnMemory(EnvioEvents.MemorySpawnerCreated @event)
    {
        Log.Value($"This memory: {@event.memory} is spawning");
        MemoryController memoryController = new MemoryController(memoryPrefab, @event.memory, @event.memorySpawnerLocation);
    }

    private void OnRespawnPlayer(PlayerEvents.RespawnPlayer @event)
    {
        Transform playerTransform = @event.playerTransform;

        playerTransform.position = initialSpawnLocation;
    }
    #endregion

    private void OnGameOver(GameEvents.GameOver ev)
    {
        //do the gameover stuffs like credits
    }

    private void OnMemory(GameEvents.MemorySelected ms)
    {
        narrativeIndex++;
        if (ms.wasPlayerCorrect)
        {
            playerStats.currentHealth += 1;
        }

        //3 pity tries
        if(playerStats.currentHealth >= narrativeIndex - 3)
        {
            //Doing great didn't miss any
            transitioner.Fade(WhichTransitioner.GOOD, GameEvents.fadeUIType.BG, narrativeBits[narrativeIndex], true);
            transitioner.Fade(WhichTransitioner.GOOD, GameEvents.fadeUIType.TXT, narrativeBits[narrativeIndex], true);
        }
        else
        {
            transitioner.Fade(WhichTransitioner.BAD, GameEvents.fadeUIType.BG, narrativeBits[narrativeIndex], true);
            transitioner.Fade(WhichTransitioner.BAD, GameEvents.fadeUIType.TXT, narrativeBits[narrativeIndex], true);
        }

        //Set Up all the memories & then call the same but reverse, txt first than bg.
    }

    private void OnStartGame(GameEvents.StartGame st)
    {
        //Fade in black BG
        transitioner.fadingFinished = false;
        transitioner.Fade(WhichTransitioner.OPEN, GameEvents.fadeUIType.BG, null, true);
        
        StartCoroutine(WaitForQuote("Game"));


    }
}
