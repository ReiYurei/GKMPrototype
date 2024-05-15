using System.Collections;
using System.Collections.Generic;
using TMPro;
using TriInspector;
using UnityEngine;
using UnityEngine.UI;
using YansaFork;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(EventListenerComponent))]
public class InventoryUIController : MonoBehaviour, IAudioSource
{
    [field: SerializeField] public SO_AudioFMODEventCollection AudioCollection  { get; private set; }
    public static InventoryUIController Instance { get; private set; }
    [field: SerializeField] public StateObserver CurrentState { get; private set; }
    [field: SerializeField] public SO_Inventory Inventory { get; private set; }
    [field: SerializeField] public RectTransform Cursor { get; private set; }
    [field: SerializeField] public GameObject NextButton { get; private set; }
    [field: SerializeField] public GameObject PrevButton { get; private set; }
    [field: SerializeField] public InputActionAsset Actions { get; private set; }
    [field: SerializeField] public SO_ParameterGameEvent ChangeStateEvent { get; private set; }
    [SerializeField] private GenericUIState _uiState;
    [SerializeField] private LoadingScreenState _loadingState;

    [SerializeField] private AnimationCurve _speedCurve = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] private float _timeToShowInventory = 1f;


    private EventSystem _eventSystem;
    private GameObject _lastSelectedObject;
    private bool _isShopOpen;

    [field: Header("Inventory Canvas")]
    [field: SerializeField] public GameObject InventoryCanvas { get; private set; }
    [field: SerializeField] public TextMeshProUGUI MoneyText { get; private set; }
    [field: Header("Character Image Canvas")]
    [field: SerializeField] public RectMask2D CharacterImageMask { get; private set; }
    [SerializeField] private int _maskYSoftness = 200;
    [SerializeField] private float _unmaskDuration = 1.5f;

    [field: Header("Status Canvas")]
    [field: SerializeField] public RectTransform StatusCanvas { get; private set; }

    [field: Header("Tab Canvas")]
    [field: SerializeField] public RectTransform TabCanvas { get; private set; }
    [field: SerializeField] public List<RectTransform> TabAreaContentObjects { get; private set; }
    private int _tabIndex = 0;


    [field: Header("View Canvas")]
    [field: SerializeField] public RectTransform ViewCanvas { get; private set; }
    [field: SerializeField] public List<RectTransform> ViewAreaContentObjects { get; private set; }
    [field: Header("View Quest Canvas")]
    [SerializeField] private List<InventoryItemQuest> _questItems;
    [field: SerializeField] public GameObject QuestItemTemplate { get; private set; }
    [field: SerializeField] public GameObject QuestItemParent { get; private set; }
    [field: SerializeField] public Image QuestItemIcon { get; private set; }
    [field: SerializeField] public TextMeshProUGUI QuestItemName  { get; private set; }
    [field: SerializeField] public TextMeshProUGUI QuestItemDescription { get; private set; }

    [field: Header("View Spell Canvas")]
    [SerializeField] private List<InventoryItemSpell> _spells;
    [field: SerializeField] public GameObject SpellTemplate { get; private set; }
    [field: SerializeField] public GameObject SpellParent { get; private set; }
    [field: SerializeField] public Image SpellIcon { get; private set; }
    [field: SerializeField] public TextMeshProUGUI SpellName { get; private set; }
    [field: SerializeField] public TextMeshProUGUI SpellDescription { get; private set; }
    [field: SerializeField] public TextMeshProUGUI SpellCooldown { get; private set; }
    [field: SerializeField] public TextMeshProUGUI SpellManaConsumption { get; private set; }
    [field: SerializeField] public GameObject SpellInputParent { get; private set; }
    [field: SerializeField] public ShopUIController.SpellInputShopUI SpellInputIcon { get; private set; }
    private int _maxSpellInput = 0;


    [field: Header("Gold")]
    [field: SerializeField] public GameObject GoldCanvas { get; private set; }
    [field: SerializeField] public Image GoldBar { get; private set; }
    [field: SerializeField] public TextMeshProUGUI GoldAmountText { get; private set; }


    public void Awake()
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
    private void Start()
    {
        AudioCollection.InitializeStartData();
    }
    [Button("Debug Raise :On Load Complete")]
    public void OnLoadComplete()
    {
        _eventSystem = EventSystem.current;
    }
    IEnumerator InitializeQuestItemData()
    {
        _questItems ??= new List<InventoryItemQuest>();
        QuestItemIcon.color = Color.clear;
        QuestItemName.text = "";
        QuestItemDescription.text =  "";
        if (Inventory.QuestItemInventory.Count <= 0) yield break;
        foreach (var item in Inventory.QuestItemInventory)
        {
            var obj = Instantiate(QuestItemTemplate, QuestItemParent.transform);
            obj.name = item.Name;
        }
        var itemArrays = QuestItemParent.GetComponentsInChildren<InventoryItemQuest>();
        foreach (var item in itemArrays)
        {
            _questItems.Add(item);
        }
        int index = 0;
        foreach (var item in Inventory.QuestItemInventory)
        {
            _questItems[index].inventoryController = this;
            _questItems[index].questItem = item;
            _questItems[index].gameObject.GetComponent<Image>().sprite = _questItems[index].questItem.Icon;

            index++;
        }
        yield return null;
    }
    IEnumerator InitializeSpellData()
    {
        _spells ??= new List<InventoryItemSpell>();
        _maxSpellInput = 0;
        MoneyText.text = Inventory.Gold.ToString();
        SpellIcon.color = Color.clear;
        SpellName.text = "";
        SpellDescription.text = "";
        SpellCooldown.text = "";
        SpellManaConsumption.text = "";
        if (Inventory.LearnedSpells.Count <= 0) yield break;       

        foreach (var spellCombo in Inventory.LearnedSpells)
        {
            var obj = Instantiate(SpellTemplate, SpellParent.transform);
            obj.name = spellCombo.Spell.SpellName;
        }
        var spellArrays = SpellParent.GetComponentsInChildren<InventoryItemSpell>();
        foreach (var spell in spellArrays)
        {
            _spells.Add(spell);
        }
        int index = 0;
        foreach (var spellCombo in Inventory.LearnedSpells)
        {
            _spells[index].inventoryController = this;
            _spells[index].combo = spellCombo;
            _spells[index].gameObject.GetComponent<Image>().sprite = _spells[index].combo.Spell.Icon;

            if (spellCombo.Command.Count > _maxSpellInput)
            {
                _maxSpellInput = spellCombo.Command.Count;
            }
            index++;
        }
        SpellInputIcon.InstantiateSpellInput(SpellInputParent.transform, _maxSpellInput);
        yield break;
    }
    private void OnEnable()
    {
        Inventory.InitializeData();

    }
    public void OnApplicationQuit()
    {
        Inventory.ClearData();
    }
    public void OnReturnToTitle()
    {
        Inventory.ClearData();
    }
    public void OnOpenShop()
    {
        GoldCanvas.SetActive(true);
        GoldAmountText.text = Inventory.Gold.ToString();
        _isShopOpen = true;
    }
    public void OnExitShop()
    {
        GoldCanvas.SetActive(false);
        _isShopOpen = false;
    }
    public void OnNotEnoughMoney()
    {
        if (!GoldCanvas.activeInHierarchy) return;
        StopCoroutine(NotEnoughMoney());
        StartCoroutine(NotEnoughMoney());
    }
    public void AnimateGold(int start, int target, ComparatorType compare)
    {
        StopCoroutine(AnimateGoldFunction(start, target, compare));
        StartCoroutine(AnimateGoldFunction(start, target, compare));

    }
    [Button("Debug Raise : Initialize Data ")]
    public void OnInventoryOpen()
    {
        StopAllCoroutines();
        StartCoroutine(InitializeSpellData());
        StartCoroutine(InitializeQuestItemData());
        StartCoroutine(InventoryInitializationAnimation());
        AudioCollection.Play_OneShot("Open Inventory");

        GoldCanvas.SetActive(false);
        _tabIndex = 0;
        Debug.Log("<color=yellow>open INVENTORY</color>");
        Actions.FindAction("Cancel").performed += CancelFunction;
        InventoryCanvas.SetActive(true);
        HideAllTab();
        ShowTab();
  
        Time.timeScale = 0f;
    }
    public void InventoryExit()
    {
        foreach (var item in _spells)
        {
            Destroy(item.gameObject);
        }
        foreach (var item in _questItems)
        {
            Destroy(item.gameObject);
        }

        _spells.Clear();
        _questItems.Clear();
        AudioCollection.Play_OneShot("Cancel");
        Debug.Log("<color=yellow>CLOSE INVENTORY</color>");
        Actions.FindAction("Cancel").performed -= CancelFunction;

        InventoryCanvas.SetActive(false);
        ChangeStateEvent.Raise(CurrentState.OverallState);

        Time.timeScale = 1f;

    }
    private void HideAllTab()
    {
        foreach(var tab in TabAreaContentObjects)
        {
            tab.gameObject.SetActive(false);
        }
        foreach (var content in ViewAreaContentObjects)
        {
            content.gameObject.SetActive(false);
        }
        Cursor.gameObject.SetActive(false);

    }
    private void ShowTab()
    {
        TabAreaContentObjects[_tabIndex].gameObject.SetActive(true);
        ViewAreaContentObjects[_tabIndex].gameObject.SetActive(true);
        AudioCollection.Play_OneShot("Confirm");
        Reselect();
    }
    private void CancelFunction(InputAction.CallbackContext context)
    {

        InventoryExit();
    }

    public void ShowQuestData(InventoryItemQuest data)
    {
        if (data.questItem == null) return;
        AudioCollection.Play_OneShot("Navigate");
        var item = data.questItem;
        if (item.Icon != null) QuestItemIcon.sprite = item.Icon;
        QuestItemIcon.color = Color.white;

        QuestItemName.text = item.Name;
        QuestItemDescription.text = item.Description;
        Cursor.gameObject.SetActive(true);
        Cursor.position = _eventSystem.currentSelectedGameObject.transform.position;

    }
    public void ShowSpellData(InventoryItemSpell data)
    {
        if (data.combo == null) return;
        AudioCollection.Play_OneShot("Navigate");
        var spell = data.combo.Spell;
        SpellIcon.color = Color.white;
        if (spell.Icon != null) SpellIcon.sprite = spell.Icon;
        SpellName.text = spell.SpellName;
        SpellDescription.text = spell.Description;
        SpellCooldown.text = spell.Cooldown.ToString();
        SpellManaConsumption.text = spell.Consumption.ToString();
        SpellInputIcon.ShowIcon(data.combo);
        Cursor.gameObject.SetActive(true);
        Cursor.position = _eventSystem.currentSelectedGameObject.transform.position;

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
        if (!InventoryCanvas.activeInHierarchy) return;
        if (_lastSelectedObject == null)
        {
            _eventSystem.SetSelectedGameObject(NextButton);
            return;
        }
        if (!_lastSelectedObject.activeInHierarchy)
        {
            _eventSystem.SetSelectedGameObject(NextButton);
            return;
        }
        _eventSystem.SetSelectedGameObject(_lastSelectedObject);


    }
    public void NextPage()
    {
        _lastSelectedObject = NextButton;
        HideAllTab();
        if (_tabIndex >= TabAreaContentObjects.Count -1)
        {
            _tabIndex = 0;
            ShowTab();
            return;
        }
        _tabIndex++;
        ShowTab();
    }
    public void PrevPage()
    {
        _lastSelectedObject = PrevButton ;
        HideAllTab();
        if (_tabIndex <= 0)
        {
            _tabIndex = TabAreaContentObjects.Count - 1;
            ShowTab();
            return;
        }
        _tabIndex--;
        ShowTab();
    }
    [Button("Test")]
    public void Test()
    {
        StopAllCoroutines();
        StartCoroutine(InventoryInitializationAnimation());
    }
    IEnumerator InventoryInitializationAnimation()
    {
        CurrentState.SetPreviousState(CurrentState.State);
        ChangeStateEvent.Raise(_loadingState);
        StartCoroutine(ResetPosition(StatusCanvas));
        StartCoroutine(ResetPosition(TabCanvas));
        StartCoroutine(ResetPosition(ViewCanvas));
        StartCoroutine(Unmask());
        StartCoroutine(MoveArea(StatusCanvas));
        yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(0.045f));
        StartCoroutine(MoveArea(TabCanvas));
        yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(0.045f));
        yield return StartCoroutine(MoveArea(ViewCanvas));
        ChangeStateEvent.Raise(_uiState);
        Reselect();

        IEnumerator Unmask()
        {
            float time = 0f;
            float speed;
            int value;
            CharacterImageMask.softness = new Vector2Int(0, 10000);
            while (CharacterImageMask.softness.y > _maskYSoftness)
            {
                time += Time.unscaledDeltaTime;
                speed = _speedCurve.Evaluate(time / _unmaskDuration);
                value = Mathf.RoundToInt(Mathf.Lerp(10000, _maskYSoftness, speed));
                CharacterImageMask.softness = new Vector2Int(0, value);
                yield return null;
            }
            yield break;

        }
        IEnumerator ResetPosition(RectTransform moveObject)
        {
            Vector2 initialPos = new Vector2(moveObject.anchoredPosition.x, -moveObject.rect.height);
            moveObject.anchoredPosition = initialPos;
            yield break;
        }
        IEnumerator MoveArea(RectTransform moveObject)
        {
            float time = 0f;
            float speed;
            
            Vector2 initialPos = new Vector2(moveObject.anchoredPosition.x, -moveObject.rect.height);
            Vector2 targetPos = new Vector2(moveObject.anchoredPosition.x, 0f);
            moveObject.anchoredPosition = initialPos;
            while (moveObject.anchoredPosition != targetPos)
            {
                time += Time.unscaledDeltaTime;
                speed = _speedCurve.Evaluate(time /_timeToShowInventory);
                moveObject.anchoredPosition = Vector2.Lerp(initialPos, targetPos, speed);
                yield return null;
            }
        }
    }
    IEnumerator NotEnoughMoney()
    {
        float delay = 0.075f;
        WaitForSeconds Delay = new WaitForSeconds(delay);
        float time = 0f;
        float timeToBeep = 0.75f;
        while (time < timeToBeep)
        {
            time += Time.unscaledDeltaTime;
            yield return Delay;
            GoldBar.color = Color.red;
            AudioCollection.Play_OneShot("Not Enough Gold");
            yield return Delay;
            GoldBar.color = Color.white;
            time += delay * 2;
            yield return null;
        }


    }
    IEnumerator AnimateGoldFunction(int start, int target, ComparatorType compare)
    {
        GoldCanvas.SetActive(true);
        int count = start;
        switch (compare)
        {
            case ComparatorType.LessThan:
                while (count < target)
                {
                    count += Mathf.RoundToInt(Time.unscaledDeltaTime * 1000f);
                    GoldAmountText.text = count.ToString();
                    AudioCollection.Play_OneShot("Add Gold");

                    yield return null;
                }
                break;
            case ComparatorType.GreaterThan:
                AudioCollection.Play_OneShot("Reduce Gold"); 
                while (count > target)
                {
                    count -= Mathf.RoundToInt(Time.unscaledDeltaTime * 1500f);
                    GoldAmountText.text = count.ToString();

                    yield return null;
                }
                break;
        }

        GoldAmountText.text = Inventory.Gold.ToString();
        if (_isShopOpen) yield break;
        yield return new WaitForSeconds(2.5f);
        GoldCanvas.gameObject.SetActive(false);

    }
}
public static class CoroutineUtil
{
    public static IEnumerator WaitForRealSeconds(float time)
    {
        float start = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup < start + time)
        {
            yield return null;
        }
    }
}