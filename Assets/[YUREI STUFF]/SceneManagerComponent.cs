using System.Collections;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
[RequireComponent(typeof(EventListenerComponent))]
public class SceneManagerComponent : MonoBehaviour
{
    [Button("Load Scene")]
    public void LoadSingleScene(string sceneName)
    {
    }
    public void OnLoadSceneRequested(ScriptableObject sceneName)
    {
        var data = sceneName as StringVariable;
        SceneManager.LoadSceneAsync(data.value, LoadSceneMode.Additive);
    }
    public void OnUnloadSceneRequest(ScriptableObject sceneName)
    {
        var data = sceneName as StringVariable;
        SceneManager.UnloadSceneAsync(data.value);
    }
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
}

