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

    public struct SetCurrentMessage:iEvent
    {
        public readonly string messageToDisplay;

        public SetCurrentMessage( string msgToDis)
        {
            messageToDisplay = msgToDis;
        }
    }

    public struct FadeUI : iEvent
    {
        public readonly bool fadingIn;
        public readonly fadeUIType;

        public FadeUI(bool fadeIn)
        {
            fadingIn = fadeIn;
        }
    };

}
