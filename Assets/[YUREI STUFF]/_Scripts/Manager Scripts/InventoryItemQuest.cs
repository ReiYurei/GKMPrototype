using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class InventoryItemQuest : MonoBehaviour, ISelectHandler, IPointerEnterHandler, IPointerExitHandler
{
    [NonSerialized] public InventoryUIController inventoryController;
    public SO_QuestItem questItem;
    public void OnPointerEnter(PointerEventData eventData)
    {
        inventoryController.Select(eventData.pointerEnter);
        inventoryController.Deselect();
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        inventoryController.Reselect();
    }
    public void OnSelect(BaseEventData eventData)
    {
        inventoryController.ShowQuestData(this);
    }

}