using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameEvents
{
    public struct GameOver:iEvent{};

    public struct MemorySelected:iEvent
    {
        public readonly bool wasPlayerCorrect;

        public MemorySelected(bool pStatus)
        {
            wasPlayerCorrect = pStatus;
        }
    }

    public struct TransitionerReady:iEvent{}

    public enum fadeUIType
    {
        TXT,
        BG
    }

    public struct StartGame:iEvent{}

    public struct PlayerCanMove:iEvent
    {
        public readonly bool canTheyMove;

        public PlayerCanMove(bool movePlease)
        {
            canTheyMove = movePlease;
        }
    };

}
