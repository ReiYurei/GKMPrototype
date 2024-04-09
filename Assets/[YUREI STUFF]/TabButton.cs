using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public enum TabType
{
    Quest,Mission, Other
}
[RequireComponent(typeof(Image))]
public class CustomTabButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    [field: SerializeField] public TabType TabType { get; private set; }
    [SerializeField] private TabGroup tabGroup;
    [SerializeField] private Image clickArea;
    [SerializeField] private Color defaultColor;
    [SerializeField] private int tabIndex;
    public void OnPointerClick(PointerEventData eventData)
    {
        tabGroup.OnTabSelected(this);

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tabGroup.OnTabEnter(this);


    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tabGroup.OnTabExit(this);

    }
    public void SetColor()
    {
        clickArea.color = defaultColor;
    }
    public void SetColor(Color color)
    {
        clickArea.color = color;
    }
    private void Start()
    {
        clickArea = GetComponent<Image>();
        clickArea.color = defaultColor;
        tabGroup.Subscribe(this);
    }
}