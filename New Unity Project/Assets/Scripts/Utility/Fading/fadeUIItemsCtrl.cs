using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fadeUIItemsCtrl : MonoBehaviour
{
    [SerializeField]
    List<fadUIItems> itemsOfThisCtrl;

    [SerializeField]
    float waitTimeBetweenChainCalls;
    string currentMessage;

    bool fadingIn;

    Action whenFinished;

    public void SetPhase(string messageToDisplay)
    {
        currentMessage = messageToDisplay;
    }

    public void Fade(bool ctrlfadingIn, GameEvents.fadeUIType whatType, Action callbackOnFinished)
    {
        List<fadUIItems> matchingItemsOfType = itemsOfThisCtrl.FindAll( fi => fi.GetType() == whatType);

        fadingIn = ctrlfadingIn;

        if(matchingItemsOfType.Count != 0 )
        {
            //Start The Fade
            StartCoroutine(FadeTheMatchingItems(matchingItemsOfType));
        }
    }

    IEnumerator FadeTheMatchingItems(List<fadUIItems> uiItems)
    {
        int numberOfItemsFaded = 0;
        GameEvents.fadeUIType currentType = uiItems[0].GetType();
        do
        {
            fadUIItems currentFader = uiItems[numberOfItemsFaded];
            currentFader.SetMode(fadingIn);

            if(currentType == GameEvents.fadeUIType.BG)
            {
                currentFader.FadeBG();
            }
            else
            {
                currentFader.FadeText();
            }
            
            yield return new WaitForSeconds(waitTimeBetweenChainCalls);
            numberOfItemsFaded++;
            
        } while (numberOfItemsFaded < uiItems.Count);
        if(whenFinished != null) whenFinished();
    }

    
}
