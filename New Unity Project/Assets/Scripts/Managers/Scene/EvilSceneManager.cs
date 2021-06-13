using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EvilSceneManager : MonoSingleton<EvilSceneManager>
{
    private string currentActiveScene;

    private string sceneToUnload;

    private int sqIndex = 0;
    private string mainScene = "main";
    public void WakeUp()
    {
        Subscribe();
    }

    private void Subscribe()
    {
        EventManager.instance.AddListener<SceneEvents.LoadNextScene>(OnLoadNextEvilScene);
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnload;

        LoadScene("StartMenu");
    }

    private void OnLoadNextEvilScene(SceneEvents.LoadNextScene @event)
    {
        this.LoadScene(@event.SceneName);
    }

    private void LoadScene(string sceneName)
    {
        try
        {
            sceneToUnload = currentActiveScene;
            currentActiveScene = sceneName;
            SceneManager.LoadSceneAsync(sceneName,LoadSceneMode.Additive);
        }
        catch (System.Exception x)
        {
            Debug.LogErrorFormat("No Scene Of {0} name in build settings. Double check scene name & build settings.", sceneName);
            throw;
        }
    }

    void OnSceneLoaded(Scene sceneLoaded, LoadSceneMode mod)
    {
        string loadedSceneName = sceneLoaded.name;
        if(loadedSceneName != "main" && sceneToUnload != null)
        {
            //If we're loading a scene other than main...
            SceneManager.UnloadSceneAsync(sceneToUnload);
        }
    }
    void OnSceneUnload( Scene sceneUnloaded)
    {
        EventManager.instance.QueueEvent(new SceneEvents.LoadedSceneRequested(currentActiveScene));
    }

}
