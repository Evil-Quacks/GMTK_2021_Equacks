using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollowPlayer : MonoBehaviour
{
    [HideInInspector]
    public Transform playerTransform;

    [HideInInspector]
    public Vector2 offSet;

    public void WakeUp()
    {
        Debug.Log("CFP ==> AWAKE");
        if (EventManager.instance)
        {
            EventManager.instance.AddListener<PlayerEvents.SendTransform>((e) =>
            {
                playerTransform = e.playerTransform;
            });
        }
    }
}
