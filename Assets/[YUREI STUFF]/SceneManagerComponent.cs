using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
[RequireComponent(typeof(EventListenerComponent))]
public class SceneManagerComponent : MonoBehaviour
{
    public void OnCutsceneStart(ScriptableObject story)
    {
        SceneManager.LoadSceneAsync("UI_Dialogue", LoadSceneMode.Additive);
        
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

