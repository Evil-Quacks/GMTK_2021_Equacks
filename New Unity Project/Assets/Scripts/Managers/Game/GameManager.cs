#define DEBUG

using System.Collections;
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
    #endregion

    #region StartUp
    public void Initialize()
    {
        //Subscribe
        Subscribe();
        Debug.Log("GM ==> CREATED");
        //Load Scene
        StartCoroutine(LoadAsyncFirstScene());
        // follower.offSet = cameraFollowOffset;
    }

    IEnumerator LoadAsyncFirstScene()
    {
#if !DEBUG
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("LEVEL-1", LoadSceneMode.Additive);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
#else
        yield return null;
#endif
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
        }
    }
    #endregion

    #region Ready Scene Change
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
        if (ms.wasPlayerCorrect)
        {
            //Yay...confetti....woo...so smart, much wow
            //Add to player points
        }

        // No seriously load the next narrative line & memories or this time
    }

}
