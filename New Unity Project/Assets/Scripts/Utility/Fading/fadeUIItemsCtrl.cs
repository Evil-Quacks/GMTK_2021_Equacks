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

    private void Awake() {
        Subscribe();
    }

    void Subscribe()
    {
        if(EventManager.instance != null)
        {
            EventManager.instance.AddListener<GameEvents.SetCurrentMessage>(OnSetPhase);
            EventManager.instance.AddListener<GameEvents.FadeUI>(OnFade);
        }
    }

    void OnSetPhase(GameEvents.SetCurrentMessage @event)
    {
        currentMessage = @event.messageToDisplay;
    }

    void OnFade(GameEvents.FadeUI @event)
    {
        List<fadUIItems> matchingItemsOfType = itemsOfThisCtrl.FindAll( fi => fi.GetType() == @event.whatToFade);

        fadingIn = @event.fadingIn;

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
    }

    [ContextMenu("Test Fade In Text")]
    public void TEST_FADEIN()
    {
        GameEvents.FadeUI mockEvent = new GameEvents.FadeUI(true, GameEvents.fadeUIType.TXT);
        OnFade(mockEvent);
    }

    [ContextMenu("Test Fade Out Text")]
    public void TEST_FADEIOUT()
    {
        GameEvents.FadeUI mockEvent = new GameEvents.FadeUI(false, GameEvents.fadeUIType.TXT);
        OnFade(mockEvent);
    }
}
