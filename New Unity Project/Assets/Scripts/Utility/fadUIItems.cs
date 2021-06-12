using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class fadUIItems : MonoBehaviour
{   
    [SerializeField]
    Image img;
    [SerializeField]
    TextMeshProUGUI text;

    [SerializeField]
    fadUIItems nextFadeTriggered;
    
    bool fadingBG;
    bool fadingText;

    bool fadingIn;

    [SerializeField]
    float fadeSpeed;

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
    private void OnSetPhase(GameEvents.SetCurrentMessage @event)
    {
        text.text = @event.messageToDisplay;
    }
    private void FixedUpdate() 
    {
        if(fadingBG)
        {
            FadeBG();   
        }
        else if(fadingText)
        {
            FadeText();
        }
    }

    private void OnFade(GameEvents.FadeUI @event)
    {
        fadingIn = @event.fadingIn;
        if(@event.fadingIn)
        {
            if(img != null)
                fadingBG = true;
            else
            {
                fadingText = true;
            }
        }
        else
        {
            fadingText = true;
        }
    }

    private void OnFadeOut()
    {
        fadingText = true;
    }

    private void FadeText()
    {
        float newAlpha;
        if(fadingIn)
        {
            newAlpha = AdjustAlpha(text.color,false);
            text.color = new Color(text.color.r, text.color.g, text.color.b, newAlpha);
        }
        else
        {
            newAlpha = AdjustAlpha(text.color,true);
            text.color = new Color(text.color.r, text.color.g, text.color.b, newAlpha);
        }

        if(newAlpha<= 0 || newAlpha >= 1)
        {
            fadingText = false;
            if(nextFadeTriggered != null)
            {
                nextFadeTriggered.TriggeredChainFade(fadingIn);
            }
            else if(!fadingIn)
            {
                //We're not fading in and there's no text after this.
                fadingBG = true;
            }
        }

    }

    private void FadeBG()
    {
        float newAlpha;
        if(!fadingIn)
        {
            newAlpha = AdjustAlpha(img.color,true);
            img.color = new Color(img.color.r, img.color.g, img.color.b, newAlpha);
        }
        else
        {
            newAlpha = AdjustAlpha(img.color,false);
            img.color = new Color(img.color.r, img.color.g, img.color.b, newAlpha);
        }

        if(newAlpha<= 0 || newAlpha >= 1)
        {
            fadingBG = false;
            if(fadingIn) fadingText = true;
        }
    }

    private float AdjustAlpha(Color startingColor, bool fadeOut)
    {
        float newAlpha;
        if(fadeOut)
        {
            newAlpha = startingColor.a - (Mathf.Exp(fadeSpeed) * Time.deltaTime);
        }
        else
        {
            newAlpha = startingColor.a + (Mathf.Exp(fadeSpeed) * Time.deltaTime);
        }

        return newAlpha;
        
    }

    public void TriggeredChainFade(bool fadeIn)
    {
        fadingIn = fadeIn;
        if(fadeIn && img != null)
        {
            fadingBG = true;
        }
        else
        {
            fadingText = true;
        }
    }

    [ContextMenu("TEST FADE IN")]
    public void testFade_IN()
    {
        GameEvents.FadeUI testFade = new GameEvents.FadeUI(true);
        OnFade(testFade);
    }

    [ContextMenu("TEST FADE OUT")]
    public void testFade_OUT()
    {
        GameEvents.FadeUI testFade = new GameEvents.FadeUI(false);
        OnFade(testFade);
    }

}
