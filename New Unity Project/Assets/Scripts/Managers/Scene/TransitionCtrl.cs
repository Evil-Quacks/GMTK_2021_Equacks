using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionCtrl : MonoBehaviour
{
    [SerializeField]
    fadeUIItemsCtrl blackStartingScreen;

    [SerializeField]
    fadeUIItemsCtrl goodScreen;

    [SerializeField]
    fadeUIItemsCtrl badScreen;

    bool fadingFinished = false;
    public void StartingTransition()
    {
        blackStartingScreen.Fade(false,GameEvents.fadeUIType.BG);
    }

    public void FadeIn(WhichTransitioner transitioner, GameEvents.fadeUIType uiType, Narrative currentNar)
    {
        if(fadingFinished)
        {
            switch (transitioner)
            {
                case WhichTransitioner.START:
                {
                    //Background first
                    blackStartingScreen.Fade(true,GameEvents.fadeUIType.BG, DoneFading);
                }
                case WhichTransitioner.OPEN:
                {
                    blackStartingScreen.Fade(false, uiType, DoneFading);
                }

                default:
            }
        }
        else
        {
            StartCoroutine(WaitingForFinish);
        }
        
    }

    IEnumerator WaitingForFinish(WhichTransitioner thisTransitioner, GameEvents.fadeUIType uiType)
    {
        yield return new WaitUntil(fadingFinished);
        FadeIn(thisTransitioner, uiType);
    }

    private void DoneFading()
    {
        fadingFinished = true;
    }
}

public enum WhichTransitioner
{
    START,
    OPEN,
    GOOD,
    BAD
}
