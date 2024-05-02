using UnityEngine;
using UnityEngine.EventSystems;
using System;
using YansaFork;
using UnityEngine.UI;

public class InventoryItemSpell : MonoBehaviour, ISelectHandler, IPointerEnterHandler, IPointerExitHandler
{
    [NonSerialized] public InventoryUIController inventoryController;
    public SO_Combo combo;
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
        inventoryController.ShowSpellData(this);
    }

}
