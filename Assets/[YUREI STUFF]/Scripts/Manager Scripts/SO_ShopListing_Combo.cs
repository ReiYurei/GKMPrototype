using UnityEngine;
using System.Collections.Generic;
using YansaFork;
using System;
using TriInspector;

[Serializable]
[CreateAssetMenu(fileName = "Shop Listing Combo", menuName ="Shop/Listing/Combo Listing")]
public class SO_ShopListing_Combo : ScriptableObject
{
    [field: SerializeField] public List<SO_ShopItem_Combo> Items {  get; private set; }
    public void ResetValue()
    {
        Items.Clear();
    }
    public void InitalizeListingData()
    {
        Items = new List<SO_ShopItem_Combo>();
    }
}
