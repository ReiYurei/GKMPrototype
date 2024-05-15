using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using YansaFork;
using System;

public class ShopItemSpell : MonoBehaviour, ISelectHandler, ISubmitHandler, IPointerClickHandler ,IPointerEnterHandler, IPointerExitHandler
{
    [NonSerialized]public ShopUIController shopCounter;
    [NonSerialized] public SO_ShopItem_Combo shopItem;

    public void OnPointerClick(PointerEventData eventData)
    {
        shopCounter.ShowData(this);
        shopCounter.Buy();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        shopCounter.Select(eventData.pointerEnter);
        shopCounter.Deselect();
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        shopCounter.Reselect();
    }
    public void OnSelect(BaseEventData eventData)
    {
        shopCounter.ShowData(this);
    }
    public void OnSubmit(BaseEventData eventData)
    {
        shopCounter.ShowData(this);
        shopCounter.Buy();
    }


}
