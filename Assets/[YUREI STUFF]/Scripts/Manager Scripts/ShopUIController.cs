using System.Collections;
using System.Collections.Generic;
using TMPro;
using TriInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using YansaFork;


[RequireComponent(typeof(EventListenerComponent))]
public class ShopUIController : MonoBehaviour, IAudioSource
{
    [field: SerializeField] public SO_AudioFMODEventCollection AudioCollection { get; private set; }
    [field: SerializeField] public SO_Inventory Inventory { get; private set; }
    [field: InlineEditor][field: SerializeField] public SO_ShopListing_Combo AllListing { get; private set; }
    [field: SerializeField] public SO_ShopListing_Combo AvailableListing { get; private set; }
    [field: SerializeField] public GameObject ListingParent { get; private set; }
    [field: SerializeField] public GameObject SpellTemplate { get; private set; }

    private ShopItemSpell[] _spells;
    [field: Header("Event")]
    [field: SerializeField] public SO_ParameterGameEvent ChangeStateEvent { get; private set; }
    [field: SerializeField] public SO_VoidGameEvent ExitShopEvent { get; private set; }
    [field: Header("Canvas")]
    [field:SerializeField] public GameObject ShopUI { get; private set; }
    [field: Header("Spell Item Canvas")]
    [field: SerializeField] public Image SpellIcon { get; private set; }
    [field: SerializeField] public TextMeshProUGUI SpellName { get; private set; }
    [field: SerializeField] public TextMeshProUGUI SpellDescription { get; private set; }
    [field: SerializeField] public TextMeshProUGUI SpellCooldown { get; private set; }
    [field: SerializeField] public TextMeshProUGUI SpellManaConsumption { get; private set; }
    [field: SerializeField] public TextMeshProUGUI Price { get; private set; }
    [field: Header("Buy Prompt Canvas")]
    [field: SerializeField] public GameObject BuyingPrompt { get; private set; }


    [field: Header("Operator Canvas")]
    [field: SerializeField] public TextMeshProUGUI OperatorText { get; private set; }

    [field: Header("Other")]
    [field: SerializeField] public InputActionAsset Actions { get; private set; }
    [SerializeField] private ListingState state;
    private EventSystem _eventSystem;
    private GameObject _lastSelectedObject;
    public void Start()
    {
        _eventSystem = EventSystem.current;
        if (AllListing == null || AllListing.Items.Count <= 0) return;
        foreach (var Item in AllListing.Items)
        {
            if (Item.RequirementToListedFulfilled())
            {
                if (AvailableListing.Items.Count <= 0) AvailableListing.InitalizeListingData();
                if (!AvailableListing.Items.Contains(Item)) AvailableListing.Items.Add(Item);
            }
        }
        StartCoroutine(InitializeShopData());
    }
    public void OnLoadComplete()
    {
        _eventSystem = EventSystem.current;
        _eventSystem.SetSelectedGameObject(_spells[0].gameObject);
    }
    IEnumerator InitializeShopData()
    {

        for (int i = 0; i < AvailableListing.Items.Count; i++)
        {
            var spell = Instantiate(SpellTemplate, ListingParent.transform);
            spell.name = AvailableListing.Items[i].SpellCombo.Spell.SpellName;
        }
        _spells = ListingParent.GetComponentsInChildren<ShopItemSpell>();
        for (int i = 0; i < AvailableListing.Items.Count; i++)
        {
            
            _spells[i].shopCounter = this;
            _spells[i].shopItem = AvailableListing.Items[i];
        }
        yield break;
    }
    private void OnApplicationQuit()
    {
        AvailableListing.ResetValue();
    }
    private void OnDisable()
    {
        AvailableListing.ResetValue();
    }
    public void OnShopOpen()
    {
        ChangeStateEvent.Raise(state);
        ShopUI.SetActive(true);
        ReadText();
    }
    public void CancelFunction()
    {
        ShopUI.SetActive(false);
        ExitShopEvent.Raise();
    }
    public void ReadText()
    {
        string text = OperatorText.text;
        OperatorText.maxVisibleCharacters = 0;
        StartCoroutine(Read());

        IEnumerator Read()
        {
            while (OperatorText.maxVisibleCharacters < OperatorText.text.Length)
            {
                OperatorText.maxVisibleCharacters++;
                yield return new WaitForSeconds(1f / 30);

            }
        }
    }
    public void Buy(GameObject selected)
    {
        Debug.Log("BUY  :  "+selected.name);
    }
    public void ShowData(SO_ShopItem_Combo data, GameObject selected)
    {
        if (data.SpellCombo == null) return;
        var spell = data.SpellCombo.Spell;
        if(spell.Icon != null) SpellIcon.sprite = data.SpellCombo.Spell.Icon; 
        SpellName.text = spell.SpellName;
        SpellDescription.text = spell.Description;
        SpellCooldown.text = spell.Cooldown.ToString();
        SpellManaConsumption.text = spell.Consumption.ToString();
        Price.text = data.Price.ToString();

    }
    public void Select(GameObject selected)
    {
        _eventSystem.SetSelectedGameObject(selected);
    }
    public void Deselect()
    {
        _lastSelectedObject = _eventSystem.currentSelectedGameObject;
    }
    public void Reselect()
    {
        _eventSystem.SetSelectedGameObject(_lastSelectedObject);
    }

}
