using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class fadUIItems : MonoBehaviour
{   
    [SerializeField]
    GameEvents.fadeUIType whatIAm;

    [SerializeField]
    Image img;
    [SerializeField]
    TextMeshProUGUI text;
    
    bool fadingBG;
    bool fadingText;

    bool fadingIn;

    [SerializeField]
    float fadeSpeed;

    Action currentCallback;

    public GameEvents.fadeUIType GetType()
    {
        return whatIAm;
    }

    public void SetMessage(string currentMessage)
    {
        text.text =currentMessage;
    }

    public void SetMode(bool infade)
    {
        fadingIn = infade;
    }

    public void SetCallback(Action callMeBack)
    {
        currentCallback = callMeBack;
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

    public void FadeText()
    {
        fadingText = true;
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
            if(currentCallback != null) currentCallback();
            currentCallback = null;
        }

    }

    public void FadeBG()
    {
        fadingBG = true;
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
            if(currentCallback != null) currentCallback();
            currentCallback = null;
        }
    }

    private float AdjustAlpha(Color startingColor, bool fadeOut)
    {
        float newAlpha;
        if(fadeOut)
        {
            newAlpha = startingColor.a - (fadeSpeed * Time.deltaTime);
        }
        else
        {
            newAlpha = startingColor.a + (fadeSpeed * Time.deltaTime);
        }

        return newAlpha;
        
    }

}
