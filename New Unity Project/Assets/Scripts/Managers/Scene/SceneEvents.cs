using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SceneEvents
{
    public struct LoadNextScene : iEvent
    {
        public readonly string SceneName;

        public LoadNextScene(string Name)
        {
            SceneName = Name;
        }
    }

    public struct LoadedSceneRequested : iEvent
    {
        public readonly string SceneName;

        public LoadedSceneRequested(string Name)
        {
            SceneName = Name;
        }
    }
}
