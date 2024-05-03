using System;
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
    [field: SerializeField] public StateObserver CurrentState { get; private set; }
    [field: SerializeField] public SO_Inventory Inventory { get; private set; }
    [field: InlineEditor][field: SerializeField] public SO_ShopListing_Combo AllListing { get; private set; }
    [field: SerializeField] public RectTransform Cursor { get; private set; }
    [field: SerializeField] public SO_ShopListing_Combo AvailableListing { get; private set; }
    [field: SerializeField] public GameObject ListingParent { get; private set; }
    [field: SerializeField] public GameObject SpellTemplate { get; private set; }

    private List<ShopItemSpell> _spells;
    [field: Header("Event")]
    [field: SerializeField] public SO_ParameterGameEvent ChangeStateEvent { get; private set; }
    [field: SerializeField] public SO_VoidGameEvent ExitShopEvent { get; private set; }
    [field: SerializeField] public SO_VoidGameEvent NotEnoughMoneyEvent { get; private set; }

    [field: Header("Canvas")]
    [field: SerializeField] public GameObject ShopUICanvas { get; private set; }
    [field: Header("Spell Item Canvas")]
    [field: SerializeField] public Image SpellIcon { get; private set; }
    [field: SerializeField] public TextMeshProUGUI SpellName { get; private set; }
    [field: SerializeField] public TextMeshProUGUI SpellDescription { get; private set; }
    [field: SerializeField] public TextMeshProUGUI SpellCooldown { get; private set; }
    [field: SerializeField] public TextMeshProUGUI SpellManaConsumption { get; private set; }
    [field: Header("Spell Input")]
    [field: SerializeField] public GameObject SpellInputParent { get; private set; }
    [field: SerializeField] public SpellInputShopUI SpellInputIcon { get; private set; }
    [System.Serializable] public class SpellInputShopUI
    {
        public Image spellInputIconTemplate;
        public Image buttonX;
        public Image buttonY;
        public Image buttonB;

        [SerializeField] private Image[] _spellIcons;

        public void InstantiateSpellInput(Transform parent, int amount)
        {
            foreach (var icon in _spellIcons)
            {
                Destroy(icon.gameObject);
            }
            _spellIcons = null;
            _spellIcons = new Image[amount];
            for (int i = 0; i < amount; i++)
            {
                var icon = Instantiate(spellInputIconTemplate, parent);
                _spellIcons[i] = icon;
            }
        }
        public void ShowIcon(SO_Combo spell)
        {
            HideIcon();
            for (int i = 0; i < spell.Command.Count; i++)
            {
                _spellIcons[i].sprite = Icon(spell.Command[i]).sprite;
                _spellIcons[i].color = Icon(spell.Command[i]).color;
                _spellIcons[i].gameObject.SetActive(true);
            }
;
        }
        public void HideIcon()
        {
            for (int i = 0; i < _spellIcons.Length; i++)
            {
                _spellIcons[i].gameObject.SetActive(false);
            }
        }
        private Image Icon(SpellInput input)
        {
            switch (input)
            {
                case SpellInput.ButtonX: 
                    return buttonX;
                case SpellInput.ButtonY:
                    return buttonY;
                case SpellInput.ButtonB:
                    return buttonB;
                default: return null;
            }
        }
    }
    private int _maxSpellInput = 0;

    [field: SerializeField] public TextMeshProUGUI Price { get; private set; }
    [field: Header("Buy Prompt Canvas")]
    [field: SerializeField] public GameObject BuyingPrompt { get; private set; }
    [field: SerializeField] public GameObject YesOption { get; private set; }


    [field: Header("Operator Canvas")]
    [field: SerializeField] public TextMeshProUGUI OperatorText { get; private set; }

    [field: Header("Other")]
    [field: SerializeField] public InputActionAsset Actions { get; private set; }
    [SerializeField] private GenericUIState state;
    private ShopItemSpell _selectedItem;
    private EventSystem _eventSystem;
    private GameObject _lastSelectedObject;
    public void Start()
    {
        _eventSystem = EventSystem.current;

     }
    private void OnApplicationQuit()
    {
        foreach (var spell in AllListing.Items)
        {
            spell.Resale();
        }
        Actions.FindAction("Cancel").performed -= CancelFunction;
        AvailableListing.ResetValue();
        _spells?.Clear();
    }
    private void OnDisable()
    {
        Actions.FindAction("Cancel").performed -= CancelFunction;
        AvailableListing.ResetValue();
        _spells?.Clear();
    }
    [TriInspector.Button("Debug Raise : Initialization")]
    public void OnLoadComplete()
    {
        _eventSystem = EventSystem.current;
    }
    IEnumerator InitializeShopData()
    {
        _spells ??= new List<ShopItemSpell>();
        _maxSpellInput = 0;
        if (AvailableListing.Items.Count <= 0)
        {
            SpellIcon.color = Color.clear;
            SpellName.text = "";
            SpellDescription.text = "";
            SpellCooldown.text = "";
            SpellManaConsumption.text = "";
            Price.text = "0";
            yield break;
        }
        for (int i = 0; i < AvailableListing.Items.Count; i++)
        {
            var spell = Instantiate(SpellTemplate, ListingParent.transform);
            spell.name = AvailableListing.Items[i].SpellCombo.Spell.SpellName;
        }

        var spellArrays = ListingParent.GetComponentsInChildren<ShopItemSpell>();
        foreach (var spell in spellArrays)
        {
            _spells.Add(spell);
        }
        for (int i = 0; i < AvailableListing.Items.Count; i++)
        {

            _spells[i].shopCounter = this;
            _spells[i].shopItem = AvailableListing.Items[i];
            _spells[i].gameObject.GetComponent<Image>().sprite = _spells[i].shopItem.SpellCombo.Spell.Icon;
            var spell = _spells[i].shopItem.SpellCombo;
            if (spell.Command.Count > _maxSpellInput)
            {
                _maxSpellInput = spell.Command.Count;
            }
        }
        SpellInputIcon.InstantiateSpellInput(SpellInputParent.transform, _maxSpellInput);
        yield break;
    }

    public void CheckItemAvailability()
    {
        if (AllListing == null || AllListing.Items.Count <= 0) return;
        foreach (var Item in AllListing.Items)
        {
            if (Item.RequirementToListedFulfilled())
            {
                if (AvailableListing.Items.Count <= 0) AvailableListing.InitalizeListingData();
                if (Item.Sold) continue;
                if (!AvailableListing.Items.Contains(Item)) AvailableListing.Items.Add(Item);
            }
        }
        StartCoroutine(InitializeShopData());
    }
    [TriInspector.Button("Debug Raise : Open Shop")]
    public void OnShopOpen()
    {
        CheckItemAvailability();
        Debug.Log("<color=yellow>OPEN SHOP</color>");
        Actions.FindAction("Cancel").performed += CancelFunction;
        ChangeStateEvent.Raise(state);
        ShopUICanvas.SetActive(true);
        Reselect();
        ReadText();
    }
    [TriInspector.Button("Debug Raise : Reset Shop")]
    public void OnReturnToTitle()
    {
        foreach (var spell in AllListing.Items)
        {
            spell.Resale();
        }
        AvailableListing.ResetValue();
        _spells.Clear();
    }
    private void CancelFunction(InputAction.CallbackContext context)
    {
        if (BuyingPrompt.activeInHierarchy)
        {
            BuyingPrompt.SetActive(false);
            Reselect();
            return;
        }
        CloseShop();
    }
    public void CloseShop()
    {

        foreach (var item in _spells)
        {
            Destroy(item.gameObject);
        }

        AvailableListing.Items.Clear();
        _spells.Clear();

        Debug.Log("<color=yellow>CLOSE SHOP</color>");
        Actions.FindAction("Cancel").performed -= CancelFunction;
        Cursor.gameObject.SetActive(false);
        ShopUICanvas.SetActive(false);
        ExitShopEvent.Raise();
    }
    public void ReadText()
    {
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
    public void Confirm()
    {
        if (!ShopUICanvas.activeInHierarchy) return;
        var spell = _selectedItem.shopItem;
        if (BuyingPrompt.activeInHierarchy)
        {
            spell.SoldOut();
            var index = _spells.IndexOf(_selectedItem);
            Inventory.LearnSpell(spell.SpellCombo);
            Destroy(_spells[index].gameObject);
            _spells.RemoveAt(index);
            _lastSelectedObject = null;
            Reselect();
            BuyingPrompt.SetActive(false);
            Inventory.ReduceGold(spell.Price);
            return;
        }
    }
    public void Cancel()
    {
        if (BuyingPrompt.activeInHierarchy)
        {
            BuyingPrompt.SetActive(false);
            Reselect();
            return;
        }
    }
    public void Buy()
    {
        var spell = _selectedItem.shopItem;
        if (Inventory.Gold < spell.Price)
        {
            NotEnoughMoneyEvent.Raise();
            return;
        }
        if (!BuyingPrompt.activeInHierarchy)
        {
            BuyingPrompt.SetActive(true);
            Deselect();
            _eventSystem.SetSelectedGameObject(YesOption);
            return;
        }
    }
    public void ShowData(ShopItemSpell data)
    {
        if (data.shopItem.SpellCombo == null) return;
        _selectedItem = data;
        var spell = data.shopItem.SpellCombo.Spell;
        SpellIcon.color = Color.white;
        if (spell.Icon != null) SpellIcon.sprite = spell.Icon;
        SpellName.text = spell.SpellName;
        SpellDescription.text = spell.Description;
        SpellCooldown.text = spell.Cooldown.ToString();
        SpellManaConsumption.text = spell.Consumption.ToString();
        SpellInputIcon.ShowIcon(data.shopItem.SpellCombo);
        Price.text = data.shopItem.Price.ToString();
        Cursor.gameObject.SetActive(true);
        Cursor.position = _selectedItem.transform.position;
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
        if (!ShopUICanvas.activeInHierarchy) return;
        if(_spells.Count <= 0) return;
        if (_lastSelectedObject == null)
        {
            _eventSystem.SetSelectedGameObject(_spells[0].gameObject);
            Cursor.position = _eventSystem.currentSelectedGameObject.transform.position;
            return;
        }
        _eventSystem.SetSelectedGameObject(_lastSelectedObject);
        Cursor.position = _eventSystem.currentSelectedGameObject.transform.position;

    }

}
