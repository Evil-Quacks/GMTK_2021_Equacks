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
    AudioSource BGMSpeaker;

    [SerializeField]
    List<AudioClip> bgmLibrary = new List<AudioClip>();

    Vector2 initialSpawnLocation;

    [Header("Transitioners")]
    [SerializeField]
    TransitionCtrl transitioner;

    [SerializeField]
    List<Narrative> narrativeBits;

    int narrativeIndex = 0;

    bool musicFade = false;

    [SerializeField]
    List<GameObject> memoryStages = new List<GameObject>();

    #endregion

    #region StartUp
    public void Initialize()
    {
        //Subscribe
        Subscribe();
        Debug.Log("GM ==> CREATED");
        EvilSceneManager.instance.WakeUp();
    }

     private void FixedUpdate() {
        if(musicFade)
        {
            if(BGMSpeaker.volume < musicVolume )
            {
                //Fade In
                BGMSpeaker.volume += .25f;
                if(BGMSpeaker.volume >= musicVolume) musicFade = false;
            }
            else
            {
                //Fade Out
                BGMSpeaker.volume =+ .25f;
                if(BGMSpeaker.volume <= 0) musicFade = false;
            }
        }
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
            EventManager.instance.AddListener<GameEvents.SendMemoryGOs>((e) =>
            {
                memoryStages = e.memoryGroups;
                memoryStages[0].SetActive(true);
                Log.Value($"memoryStages should have Count of 7: memoryStages.Count: {memoryStages.Count}");
            });
        }

        EventManager.instance.QueueEvent(new SceneEvents.LoadedSceneRequested("StartMenu"));
    }
    #endregion

    #region Ready Scene Change

    private void OnRequestedSceneLoaded(SceneEvents.LoadedSceneRequested @event)
    {
        if (@event.SceneName == "StartMenu")
        {
            BGMSpeaker.clip = bgmLibrary[0];
            BGMSpeaker.loop = true;
            musicFade = true;
            BGMSpeaker.Play();
            transitioner.Fade(WhichTransitioner.START, GameEvents.fadeUIType.BG, null, false);

        }
        else if (@event.SceneName == "Game")
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
        transitioner.Fade(WhichTransitioner.OPEN, GameEvents.fadeUIType.TXT, null, true);

        yield return new WaitUntil(() => { return transitioner.fadingFinished; });

        //Wait a bit with the quote on screen
        yield return new WaitForSeconds(5);

        //Fade Out Text
        transitioner.fadingFinished = false;
        transitioner.Fade(WhichTransitioner.OPEN, GameEvents.fadeUIType.TXT, null, false);
        yield return new WaitUntil(() => { return transitioner.fadingFinished; });

        //Fade in First Narrative
        transitioner.fadingFinished = false;
        transitioner.Fade(WhichTransitioner.GOOD, GameEvents.fadeUIType.BG, narrativeBits[narrativeIndex], true);
        //Start BGM

        yield return new WaitUntil(() => { return transitioner.fadingFinished; });

        //Fade out black screen
        transitioner.fadingFinished = false;
        transitioner.Fade(WhichTransitioner.OPEN, GameEvents.fadeUIType.BG, narrativeBits[narrativeIndex], false);

        yield return new WaitUntil(() => { return transitioner.fadingFinished; });

        //Fade in Narrative Text
        transitioner.fadingFinished = false;
        transitioner.Fade(WhichTransitioner.GOOD, GameEvents.fadeUIType.TXT, narrativeBits[narrativeIndex], true);
        yield return new WaitUntil(() => { return transitioner.fadingFinished; });

        //Let text sit on screen...
        yield return new WaitForSeconds(5);

        //Fade text out...
        transitioner.fadingFinished = false;
        transitioner.Fade(WhichTransitioner.GOOD, GameEvents.fadeUIType.TXT, narrativeBits[narrativeIndex], false);
        yield return new WaitUntil(() => { return transitioner.fadingFinished; });

        //Fade out Good BG...
        transitioner.fadingFinished = false;
        transitioner.Fade(WhichTransitioner.GOOD, GameEvents.fadeUIType.BG, narrativeBits[narrativeIndex], false);
        yield return new WaitUntil(() => { return transitioner.fadingFinished; });

        EventManager.instance.QueueEvent(new GameEvents.PlayerCanMove(true));

        StopCoroutine("WaitToFadeOutToScene");
    }

    IEnumerator WaitForQuote(string thenLoadThisScene)
    {
        yield return new WaitUntil(() => { return transitioner.fadingFinished; });
        if (thenLoadThisScene != "")
        {
            EventManager.instance.QueueEvent(new SceneEvents.LoadNextScene("Game"));
        }

        StopCoroutine("WaitForQuote");
    }

    IEnumerator WaitForReadText(WhichTransitioner transToUse)
    {
        yield return new WaitForSeconds(5);
        transitioner.Fade(transToUse, GameEvents.fadeUIType.BG, narrativeBits[narrativeIndex], false);
        transitioner.Fade(transToUse, GameEvents.fadeUIType.TXT, narrativeBits[narrativeIndex], false);
        yield return new WaitUntil(() => { return transitioner.fadingFinished; });
        EventManager.instance.QueueEvent(new GameEvents.PlayerCanMove(true));
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
        memoryStages[narrativeIndex].SetActive(false);
        narrativeIndex++;

        if (ms.wasPlayerCorrect)
        {
            playerStats.currentHealth += 1;
        }

        Narrative currentNarr;
        WhichTransitioner currentTrans;
        //3 pity tries
        if (playerStats.currentHealth >= narrativeIndex - 3)
        {
            //Doing great didn't miss any
            if (narrativeIndex >= 6)
            {
                //They diverge
                currentNarr = narrativeBits.Find(n => { return n.moralType == NarrativeType.GOOD && n.orderInStory == narrativeIndex; });
            }
            else
            {
                currentNarr = narrativeBits[narrativeIndex];
            }
            transitioner.Fade(WhichTransitioner.GOOD, GameEvents.fadeUIType.BG, currentNarr, true);
            transitioner.Fade(WhichTransitioner.GOOD, GameEvents.fadeUIType.TXT, currentNarr, true);
            currentTrans = WhichTransitioner.GOOD;

        }
        else
        {
            if (narrativeIndex >= 6)
            {
                //They diverge
                currentNarr = narrativeBits.Find(n => { return n.moralType == NarrativeType.BAD && n.orderInStory == narrativeIndex; });
            }
            else
            {
                currentNarr = narrativeBits[narrativeIndex];
            }

            transitioner.Fade(WhichTransitioner.BAD, GameEvents.fadeUIType.BG, currentNarr, true);
            transitioner.Fade(WhichTransitioner.BAD, GameEvents.fadeUIType.TXT, currentNarr, true);
            currentTrans = WhichTransitioner.BAD;
        }

        //Set Up all the memories & then call the same but reverse, txt first than bg.
        EventManager.instance.QueueEvent(new GameEvents.PlayerCanMove(false));
        memoryStages[narrativeIndex].SetActive(true);
        StartCoroutine(WaitForReadText(currentTrans));
    }

    private void OnStartGame(GameEvents.StartGame st)
    {
        //Fade in black BG
        transitioner.fadingFinished = false;
        transitioner.Fade(WhichTransitioner.OPEN, GameEvents.fadeUIType.BG, null, true);

        StartCoroutine(WaitForQuote("Game"));

    }
}
