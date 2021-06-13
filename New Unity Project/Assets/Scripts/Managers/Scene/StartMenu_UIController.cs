using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class StartMenu_UIController : MonoBehaviour
{
    
    [SerializeField]
    Button startButton;

    [SerializeField]
    Button quitButton;

    private void Start() {
        startButton.onClick.AddListener(() =>{
            EventManager.instance.QueueEvent(new GameEvents.StartGame());
        });

        quitButton.onClick.AddListener(() =>{
            Application.Quit();
        });
    }

}
