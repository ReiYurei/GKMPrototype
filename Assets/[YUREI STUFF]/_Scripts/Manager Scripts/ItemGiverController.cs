using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(EventListenerComponent))]
public class ItemGiverController : MonoBehaviour
{
    [field: SerializeField] public List<SO_QuestItem> Items { get; private set; }
    [field: SerializeField] public SO_Inventory Inventory { get; private set; }
    [field: SerializeField] public GameObject ItemGiverCanvas { get; private set; }
    [field: SerializeField] public Image ItemTemplate { get; private set; }
    [field: SerializeField] public Image ItemFrame { get; private set; }

    [field: SerializeField] public GameObject ItemParent { get; private set; }
    private GameObject[] _items;
    bool _hasGiven;
    public void InitializeItems(ScriptableObject data)
    {
        Reinitialize();
        var story = data as SO_StoryData;
        Items ??= new List<SO_QuestItem>();
        if (story.DialogueRewards.Count == 0) 
        {
            _hasGiven = true;
            return;
        }
        _hasGiven = false;

        for (int i = 0; i < story.DialogueRewards.Count; i++) 
        {
            if (!(story.DialogueRewards[i] is SO_ItemReward)) continue;
            var rewardData = story.DialogueRewards[i] as SO_ItemReward;
            if (!(rewardData.Item is SO_QuestItem)) continue;
            var reward = rewardData.Item as SO_QuestItem;
            Items.Add(reward);
        }
        _hasGiven = false;

    }

    public void Give()
    {
        _items = new GameObject[Items.Count];
        ItemGiverCanvas.SetActive(true);
        for(int i = 0; i < Items.Count; i++) 
        {
            var frame = Instantiate(ItemFrame, ItemParent.transform);
            var item = Instantiate(ItemTemplate, frame.transform);
            item.sprite = Items[i].Icon;
            frame.gameObject.SetActive(true);
            _items[i] = frame.gameObject;
            Inventory.AddQuestItem(Items[i]);
        }
        _hasGiven = true;
    }
    public void GiveNoVisual()
    {

        for (int i = 0; i < Items.Count; i++)
        {
            Inventory.AddQuestItem(Items[i]);
        }
        _hasGiven = true;
    }

    public void Clear()
    {
        if (!_hasGiven) return;
        if (_items == null)
        {
            ItemGiverCanvas.SetActive(false);
            Items.Clear();
            return;
        }
        for (int i = 0; i < _items.Length;i++)
        {
            Destroy(_items[i].gameObject);
        }
        _items = null;
        Items.Clear();
        ItemGiverCanvas.SetActive(false);

    }
    public void Reinitialize()
    {
        if (_items == null)
        {
            ItemGiverCanvas.SetActive(false);
            Items.Clear();
            return;
        }
        for (int i = 0; i < _items.Length; i++)
        {
            Destroy(_items[i].gameObject);
        }
        _items = null;
        Items.Clear();
        ItemGiverCanvas.SetActive(false);

    }
}
