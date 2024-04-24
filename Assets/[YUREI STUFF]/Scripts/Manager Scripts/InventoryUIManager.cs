using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(EventListenerComponent))]
public class InventoryUIController : MonoBehaviour
{
    [field: SerializeField] public static InventoryUIController Instance { get; private set; }

    [field: SerializeField] public SO_Inventory Inventory { get; private set; }

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
    }
    public void OnExitShop()
    {
        GoldCanvas.SetActive(false);
    }
    public void OnNotEnoughMoney()
    {
        if (!GoldCanvas.activeInHierarchy) return;
        StopCoroutine(NotEnoughMoney());
        StartCoroutine(NotEnoughMoney());
    }
    public void AnimateGold(int start, int target,ComparatorType compare)
    {
        if (!GoldCanvas.activeInHierarchy) return;
        StopCoroutine(AnimateGoldFunction(start, target, compare));
        StartCoroutine(AnimateGoldFunction(start, target, compare));

    }
    IEnumerator NotEnoughMoney()
    {
        float delay = 0.075f;
        WaitForSeconds Delay = new WaitForSeconds(delay);
        float time = 0f;
        float timeToBeep = 0.75f;
        while (time < timeToBeep)
        {
            time += Time.deltaTime;
            yield return Delay;
            GoldBar.color = Color.red;
            yield return Delay;
            GoldBar.color = Color.white;
            time += delay * 2;
            yield return null;
        }


    }
    IEnumerator AnimateGoldFunction(int start, int target, ComparatorType compare)
    {
        int count = start;
        switch (compare)
        {
            case ComparatorType.LessThan:
                while (count < target)
                {
                    count += Mathf.RoundToInt(Time.deltaTime * 500f);
                    GoldAmountText.text = count.ToString();
                    yield return null;
                }
                break;
            case ComparatorType.GreaterThan:
                while (count > target)
                {
                    count -= Mathf.RoundToInt(Time.deltaTime * 500f);
                    GoldAmountText.text = count.ToString();
                    yield return null;
                }
                break;
        } 

        GoldAmountText.text = Inventory.Gold.ToString();
        //yield return new WaitForSeconds(3f);

    }
}