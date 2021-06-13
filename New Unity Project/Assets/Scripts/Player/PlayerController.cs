using UnityEngine;

public class PlayerController
{
    PlayerView playerPrefab;
    PlayerView currentPlayer;
    PlayerStats playerModel;

    public PlayerController(PlayerView prefab, PlayerStats model, Transform spawner)
    {
        playerPrefab = prefab;
        playerModel = model;

        Subscribe();
        Debug.Log(" PC ==> CREATED");
        InitializePlayer(spawner);
    }

    void Subscribe()
    {
        if (EventManager.instance != null)
        {
            EventManager.instance.AddListener<PlayerEvents.ObsCollision>(OnCollisionWithObstacle);
        }
    }

    void InitializePlayer(Transform playerSpawn)
    {
        if (playerSpawn != null && currentPlayer == null)
        {
            //Found Spawn
            currentPlayer = GameObject.Instantiate(playerPrefab, playerSpawn);
            EventManager.instance.QueueEvent(new PlayerEvents.SendTransform(currentPlayer.transform));
            currentPlayer.Initialize(playerModel.jumpImpulseForce, playerModel.moveForce, playerModel.maxMoveVelocity, playerModel.airForce);
        }
    }

    void OnCollisionWithObstacle(PlayerEvents.ObsCollision @event)
    {
        //Get Obstacle Class
        Obstacle collidedObs = @event.obstacle.GetComponent<Obstacle>();
        if (collidedObs)
        {
            if (collidedObs.isWinObj)
            {
                @event.isCorrectState(true);
            }
            else
            {
                @event.isCorrectState(false);
            }
        }
    }
}
