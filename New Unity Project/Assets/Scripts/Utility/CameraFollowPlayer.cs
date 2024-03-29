﻿using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollowPlayer : MonoBehaviour
{
    [HideInInspector]
    public Transform playerTransform;

    [HideInInspector]
    public Vector2 offSet;

    bool sleeping = true;

    public void WakeUp()
    {
        Debug.Log("CFP ==> AWAKE");
        if (EventManager.instance)
        {
            EventManager.instance.AddListener<PlayerEvents.SendTransform>((e) =>
            {
                playerTransform = e.playerTransform;
                sleeping = false;
            });
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(!sleeping)
        {
            Vector3 temp = this.transform.position;

            temp.x = playerTransform.position.x + offSet.x;
            temp.y = playerTransform.position.y + offSet.y;

            transform.position = temp;

            // if (playerTransform != null)
            // {
            //     if (playerTransform.position.y <= -6)
            //     {
            //         EventManager.instance.QueueEvent(new PlayerEvents.RespawnPlayer(playerTransform));
            //     }
            //     else
            //     {
            //         Vector3 temp = this.transform.position;

            //         temp.x = playerTransform.position.x + offSet.x;
            //         temp.y = playerTransform.position.y + offSet.y;

            //         transform.position = temp;
            //     }
            // }
        }
        
    }
}
