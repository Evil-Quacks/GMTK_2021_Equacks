using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EvilSceneManager : MonoSingleton<EvilSceneManager>
{
    private Scene currentScene;
    private Scene oldSceneStillActive;

    private Scene mainScene;
    public void WakeUp()
    {
        Subscribe();
        mainScene = SceneManager.GetSceneByName("main");
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
            Scene sceneToLoad = SceneManager.GetSceneByName(sceneName);
            SceneManager.LoadSceneAsync(sceneName,LoadSceneMode.Additive);

            oldSceneStillActive = currentScene;
            currentScene = sceneToLoad;
        }
        catch (System.Exception x)
        {
            Debug.LogErrorFormat("No Scene Of {0} name in build settings. Double check scene name & build settings.", sceneName);
            throw;
        }
    }

    void OnSceneLoaded(Scene sceneLoaded, LoadSceneMode mod)
    {
        if(sceneLoaded == currentScene && oldSceneStillActive != mainScene )
        {
            SceneManager.UnloadSceneAsync(oldSceneStillActive.name);
            SceneManager.SetActiveScene(currentScene);
        }
    }

    void OnSceneUnload( Scene sceneUnloaded)
    {
        if(sceneUnloaded == oldSceneStillActive)
        {
            oldSceneStillActive = mainScene;
        }
    }

}
