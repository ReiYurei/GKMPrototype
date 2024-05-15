using UnityEngine;

public class EventSystemComponent : MonoBehaviour
{
    public static EventSystemComponent Instance { get; private set; }
    private void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }
}