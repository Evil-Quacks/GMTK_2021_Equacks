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

    public void Fade(WhichTransitioner transitioner, GameEvents.fadeUIType uiType, Narrative currentNar, bool fadingIn)
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
        else
        {
            fadingFinished = true;
        }
        
    }

    IEnumerator BufferTime()
    {
        yield return new WaitForSeconds(1);
        fadingFinished = true;
        StopCoroutine("BufferTime");
    }
    private void DoneFading()
    {
        StartCoroutine(BufferTime());
    }
}

public enum WhichTransitioner
{
    START,
    OPEN,
    GOOD,
    BAD
}
