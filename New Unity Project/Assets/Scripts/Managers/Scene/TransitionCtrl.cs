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

    public bool fadingFinished = true;

    private void Awake() {
        fadingFinished = true;
    }
    
    public void Fade(WhichTransitioner transitioner, GameEvents.fadeUIType uiType, Narrative currentNar, bool fadingIn)
    {
        if(fadingFinished)
        {
            fadeUIItemsCtrl ctrlToCall = null;
            switch (transitioner)
            {
                case WhichTransitioner.START:
                {
                    ctrlToCall = blackStartingScreen;
                    break;
                }
                case WhichTransitioner.OPEN:
                {
                    ctrlToCall = blackStartingScreen;
                    break;
                }
                case WhichTransitioner.BAD:
                {
                    badScreen.SetPhase(currentNar.messageToShow);
                    ctrlToCall = badScreen;
                    break;
                }
                case WhichTransitioner.GOOD:
                {
                    goodScreen.SetPhase(currentNar.messageToShow);
                    ctrlToCall = goodScreen;
                    break;
                }
                default:break;
            }

            if(ctrlToCall!= null)
            {
                ctrlToCall.Fade(fadingIn,uiType,DoneFading);
            }
        }
        else
        {
            StartCoroutine(WaitingForFinish(transitioner, uiType, currentNar, fadingIn));
        }
        
    }

    IEnumerator WaitingForFinish(WhichTransitioner thisTransitioner, GameEvents.fadeUIType uiType, Narrative nar, bool fadingIn)
    {
        yield return new WaitUntil(() =>{return fadingFinished;});
        Fade(thisTransitioner, uiType, nar, fadingIn );
    }

    private void DoneFading()
    {
        fadingFinished = true;
        StopCoroutine("WaitingForFinish");
    }
}

public enum WhichTransitioner
{
    START,
    OPEN,
    GOOD,
    BAD
}
