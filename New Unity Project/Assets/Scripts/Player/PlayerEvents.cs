using System;
using UnityEngine;

namespace PlayerEvents
{

    public struct ObsCollision : iEvent
    {
        public readonly GameObject obstacle;

        public readonly Action<bool> isCorrectState;

        public ObsCollision(GameObject _obstacle, Action<bool> stateCheckCallback)
        {
            obstacle = _obstacle;
            isCorrectState = stateCheckCallback;
        }
    }

    public struct SendTransform : iEvent
    {
        public readonly Transform playerTransform;

        public SendTransform(Transform pTransform)
        {
            playerTransform = pTransform;
        }
    }

    public struct RespawnPlayer : iEvent
    {
        public Transform playerTransform;

        public RespawnPlayer(Transform referenceToPlayerTransform)
        {
            playerTransform = referenceToPlayerTransform;
        }
    }

}