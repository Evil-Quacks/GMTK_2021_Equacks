using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EvilSceneManager : MonoSingleton<EvilSceneManager>
{
    private string currentScene;
    private string oldSceneStillActive;

    private string mainScene = "main";
    public void WakeUp()
    {
        Subscribe();
        currentScene = mainScene;
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
            SceneManager.LoadSceneAsync(sceneName,LoadSceneMode.Additive);

            oldSceneStillActive = currentScene;
            currentScene = sceneName;
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
        if(loadedSceneName == currentScene && oldSceneStillActive != mainScene )
        {
            this.UnloadSceneAsync(SceneManager.GetSceneByName(oldSceneStillActive));
            SceneManager.SetActiveScene(sceneLoaded);
        }
    }

    public static AsyncOperation UnloadSceneAsync(SceneManagement.Scene sc)
    {
        
    }
    void OnSceneUnload( Scene sceneUnloaded)
    {
        if(sceneUnloaded.name == oldSceneStillActive)
        {
            oldSceneStillActive = mainScene;
        }
    }

}
