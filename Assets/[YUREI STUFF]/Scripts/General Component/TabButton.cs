using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
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
    public Color defaultColor = Color.white;
    public Color hoverColor = Color.white;
    public Color activeColor = Color.white;
    [SerializeField] private int tabIndex;
    [SerializeField] private UnityEvent ClickEvent;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (tabGroup != null)
        {
            tabGroup.OnTabSelected(this);
            return;
        }
        clickArea.color = activeColor;
        ClickEvent?.Invoke();

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tabGroup != null)
        {
            tabGroup.OnTabEnter(this);
            return;
        } 
        clickArea.color = hoverColor;


    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tabGroup != null)
        {
            tabGroup.OnTabExit(this);
            return;

        }
        clickArea.color = defaultColor;

    }
    public void SetColor()
    {
        clickArea.color = defaultColor;
    }
    public void SetColor(Color color)
    {
        clickArea.color = color;
    }
     private void Awake()
     {
         clickArea = GetComponent<Image>();
         clickArea.color = defaultColor;
     }


}