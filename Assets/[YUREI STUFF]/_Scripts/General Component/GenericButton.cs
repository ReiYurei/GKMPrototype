using UnityEngine;
using UnityEngine.EventSystems;
public class GenericButton : MonoBehaviour, IAudioSource, IPointerEnterHandler, ISelectHandler
{
    [field:SerializeField] public SO_AudioFMODEventCollection AudioCollection { get; private set; }



    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioCollection.Play_OneShot("Navigate");
    }

    public void OnSelect(BaseEventData eventData)
    {
        AudioCollection.Play_OneShot("Navigate");
    }


    public void Start()
    {
        AudioCollection.InitializeStartData();
    }

}