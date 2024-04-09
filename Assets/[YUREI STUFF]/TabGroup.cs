using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class TabGroup : MonoBehaviour
{
    [field: SerializeField]public List<CustomTabButton> Buttons { get; private set; }
    [field: SerializeField] public List<GameObject> Tabs { get; private set; }
    [field: SerializeField] public SO_VoidGameEvent ChangeTabEvent { get; private set; }
    [field: SerializeField] public CustomTabButton SelectedTab { get; private set; }
    public Color hoverColor;
    public Color activeColor;
    [SerializeField] private InputActionAsset _actions;
    public int tabIndex;
    private void Awake()
    {
        _actions.FindActionMap("Listing UI").Enable();
        _actions.FindActionMap("Listing UI").FindAction("Next Tab").performed += NextTab;
        _actions.FindActionMap("Listing UI").FindAction("Previous Tab").performed += PreviousTab;

    }
    private void OnDisable()
    {
        _actions.FindActionMap("Listing UI").FindAction("Next Tab").performed -= NextTab;
        _actions.FindActionMap("Listing UI").FindAction("Previous Tab").performed -= PreviousTab;
    }
    private void Start()
    {
        tabIndex = 0;
        SelectedTab = Buttons[tabIndex];
        OnTabSelected(Buttons[tabIndex]);
    }
    public void NextTab(InputAction.CallbackContext context)
    {
        if(tabIndex >= Tabs.Count - 1)
        {
            tabIndex = 0;
            OnTabSelected(Buttons[tabIndex]);
            return;
        }
        tabIndex++;
        OnTabSelected(Buttons[tabIndex]);

    }
    public void PreviousTab(InputAction.CallbackContext context)
    {
        if (tabIndex <= 0)
        {
            tabIndex = Tabs.Count - 1;
            OnTabSelected(Buttons[tabIndex]);
            return;
        }
        tabIndex--;
        OnTabSelected(Buttons[tabIndex]);
    }
    public void Subscribe( CustomTabButton button)
    {
        if (Buttons == null) Buttons = new List<CustomTabButton>();
        Buttons.Add(button);    
    }
    public void OnTabEnter(CustomTabButton button) 
    {
        ResetTabs();
        if (button == SelectedTab) return;
        button.SetColor(hoverColor);
    }
    public void OnTabSelected(CustomTabButton button)
    {
        SelectedTab = button;
        tabIndex = Buttons.IndexOf(button);
        ResetTabs();
        button.SetColor(activeColor);
    }

    private void ChangeTab()
    {

        foreach (GameObject obj in Tabs)
        {
            obj.SetActive(false);
        }
        Tabs[tabIndex].SetActive(true);
        ChangeTabEvent?.Raise();
    }

    public void OnTabExit(CustomTabButton button)
    {
        ResetTabs();

    }
    private void ResetTabs()
    {
        foreach (CustomTabButton button in Buttons)
        {
            if (SelectedTab != null && button == SelectedTab) continue;
            button.SetColor();
        }
        ChangeTab();

    }
}