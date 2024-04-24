using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EventListenerComponent))]
public class ShopCounterComponent : MonoBehaviour, IInteractable, IAudioSource
{
    [field: SerializeField] public SO_CompletionObserver Observer { get; private set; }
    [field: SerializeField] public SO_AudioFMODEventCollection AudioCollection { get; private set; }

    [field: Header("Shop Listing")]



    [field: Header("Interaction Dialogue")]
    [field: SerializeField] public SO_StoryData InteractDialogue { get; private set; }
    [field: SerializeField] public SO_StoryData ExitDialogue { get; private set; }

    [field: Header("Event")]
    [field: SerializeField] public SO_ParameterGameEvent ChangeStateEvent { get; private set; }
    [field: SerializeField] public SO_VoidGameEvent OpenShopEvent { get; private set; }


    [field: Header("Other")]
    [SerializeField] private HubState _hubState;

    private Queue<SO_StoryData> _storyQueue;
    private Queue<SO_StoryData> _shopStoryQueue;

 
    private void Start()
    {
        _storyQueue = new Queue<SO_StoryData>();
        _shopStoryQueue = new Queue<SO_StoryData>();

    }
    public void OnLoadComplete()
    {
    }

    public void OnExitListing()
    {
        if (ExitDialogue == null)
        {
            ChangeStateEvent.Raise(_hubState);
            return;
        }
        ExitDialogue.StartStoryDialogue();
    }
    private void CheckShopListing()
    {
   
    }

    public void EnqueueEvents()
    {
        foreach (SO_StoryData story in Observer.StoryObserver.AllStoryData)
        {
            if (story.PlayAt == PlayAt.EnteringHub) continue;
            if (story.HasSeen() || story.TempSeen()) continue;
            _storyQueue.Enqueue(story);
        }
        PlayEvents();

    }
    private void PlayEvents()
    {
        

    }
    IEnumerator PlayQueuedEvents()
    {
        yield return new WaitForSeconds(0.25f);

        PlayEvents();
    }
    public void OnDialogueEnd()
    {
        StartCoroutine(PlayQueuedEvents());
    }


    [ContextMenu("Interact")]
    public void OnInteract() //Check Shop Items
    {
        Debug.Log("Interacted");
        OpenShopEvent.Raise();
    }

}
