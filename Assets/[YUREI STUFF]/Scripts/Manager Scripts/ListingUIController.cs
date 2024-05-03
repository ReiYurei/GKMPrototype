using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
[RequireComponent(typeof(EventListenerComponent))]
public class ListingUIController : MonoBehaviour, IAudioSource
{
    [field: SerializeField] public SO_CompletionObserver Observer { get; private set; }
    [field: SerializeField] public SO_AudioFMODEventCollection AudioCollection { get; private set; }
    [field: Header("Event")]
    [field: SerializeField] public SO_ParameterGameEvent ChangeStateEvent { get; private set; }
    [field: SerializeField] public SO_VoidGameEvent ExitListingEvent { get; private set; }
    [field: Header("Quest")]
    [field: SerializeField] public SO_QuestListing AvailableQuests { get; private set; }
    [field: SerializeField] public GameObject QuestUICanvas { get; private set; }
    [field: SerializeField] public QuestListingUI QuestUI { get; private set; }
    [field: Header("Mission")]
    [field: SerializeField] public SO_MissionListing AvailableMissions { get; private set; }
    [field: SerializeField] public GameObject MissionUICanvas { get; private set; }
    [field: SerializeField] public MissionListingUI MissionUI { get; private set; }
    [field: Header("Canvas")]
    [field: Header("Page Canvas")]
    [field: SerializeField] public GameObject ListingUICanvas { get; private set; }
    [field: SerializeField] public GameObject ClearMark { get; private set; }

    [field:SerializeField] public TabGroup TabGroup { get; private set; }
    [field: SerializeField] public CustomTabButton NextPageButton { get; private set; }
    [field: SerializeField] public CustomTabButton PrevPageButton { get; private set; }
    [field: SerializeField] public GameObject PromptConfirmSelection { get; private set; }
    [field: SerializeField] public List<CustomTabButton> PromptButton { get; private set; }
    [field: SerializeField] public GameObject PromptRemoveSelection { get; private set; }
    [field: SerializeField] public List<CustomTabButton> PromptRemoveSelectionButton { get; private set; }


    [field: SerializeField] public TextMeshProUGUI PageText { get; private set; }
    [field: Header("Operator Canvas")]
    [field: SerializeField] public TextMeshProUGUI OperatorText { get; private set; }

    [field: Header("Other")]
    [field: SerializeField] public InputActionAsset Actions { get; private set; }
    [SerializeField] private ListingState state;

    private int pageIndex;
    private int promptIndex;
    [SerializeField]private string inputName = "Listing";

    private void OnEnable()
    {
        Actions.FindActionMap(inputName).FindAction("Next Page").started += NextPage;
        Actions.FindActionMap(inputName).FindAction("Previous Page").started += PreviousPage;

        Actions.FindActionMap(inputName).FindAction("Next Page").canceled += NextPage;
        Actions.FindActionMap(inputName).FindAction("Previous Page").canceled += PreviousPage;

        Actions.FindActionMap(inputName).FindAction("Next Page").performed += NextPage;
        Actions.FindActionMap(inputName).FindAction("Previous Page").performed += PreviousPage;

        Actions.FindActionMap(inputName).FindAction("Confirm").performed += Confirm;
        Actions.FindActionMap(inputName).FindAction("Return").performed += Cancel;
    }
    private void OnDisable()
    {
        Actions.FindActionMap(inputName).FindAction("Next Page").canceled -= NextPage;
        Actions.FindActionMap(inputName).FindAction("Previous Page").canceled -= PreviousPage;

        Actions.FindActionMap(inputName).FindAction("Next Page").started -= NextPage;
        Actions.FindActionMap(inputName).FindAction("Previous Page").started -= PreviousPage;

        Actions.FindActionMap(inputName).FindAction("Next Page").performed -= NextPage;
        Actions.FindActionMap(inputName).FindAction("Previous Page").performed -= PreviousPage;
        Actions.FindActionMap(inputName).FindAction("Confirm").performed -= Confirm;
        Actions.FindActionMap(inputName).FindAction("Return").performed -= Cancel;
    }
    private void Cancel(InputAction.CallbackContext context)
    {
        CancelFunction();
    }
    public void CancelFunction()
    {
        if (PromptConfirmSelection.activeInHierarchy || PromptRemoveSelection.activeInHierarchy)
        {
            promptIndex = 0;
 
            PromptConfirmSelection.SetActive(false);
            PromptRemoveSelection.SetActive(false);
            Actions.FindActionMap(inputName).FindAction("Next Tab").Enable();
            Actions.FindActionMap(inputName).FindAction("Previous Tab").Enable();
            return;
        }
        ListingUICanvas.SetActive(false);
        ExitListingEvent.Raise();
    }

    private void Confirm(InputAction.CallbackContext context)
    {
        Debug.Log("<color=yellow>CONFIRM</color>");
    
        switch (TabGroup.SelectedTab.TabType)
        {
            case TabType.Quest:
                Debug.Log("Quest");
                if (PromptRemoveSelection.activeInHierarchy || PromptConfirmSelection.activeInHierarchy) break;
                if (Observer.AssignedQuest != null && Observer.AssignedQuest == AvailableQuests.Quests[pageIndex])
                {
                    promptIndex = 0;
                    PromptRemoveSelection.SetActive(true);
                    for (int i = 0; i < PromptRemoveSelectionButton.Count; i++)
                    {
                        PromptRemoveSelectionButton[i].SetColor();
                    }
                    var button = PromptButton[promptIndex];
                    button.SetColor(button.hoverColor);
                    Actions.FindActionMap(inputName).FindAction("Next Tab").Disable();
                    Actions.FindActionMap(inputName).FindAction("Previous Tab").Disable();
                    return;
                }
                break;

            case TabType.Mission:
                Debug.Log("Mission");

                if (PromptRemoveSelection.activeInHierarchy || PromptConfirmSelection.activeInHierarchy) break;
                if (Observer.AssignedMission != null && Observer.AssignedMission == AvailableMissions.Missions[pageIndex])
                {
                    promptIndex = 0;
                    PromptRemoveSelection.SetActive(true);
                    for (int i = 0; i < PromptRemoveSelectionButton.Count; i++)
                    {
                        PromptRemoveSelectionButton[i].SetColor();
                    }
                    var button = PromptRemoveSelectionButton[promptIndex];
                    button.SetColor(button.hoverColor);
                    Actions.FindActionMap(inputName).FindAction("Next Tab").Disable();
                    Actions.FindActionMap(inputName).FindAction("Previous Tab").Disable();
                    return;
                }
                break;
        }
        if (!PromptConfirmSelection.activeInHierarchy && !PromptRemoveSelection.activeInHierarchy)
        {
            switch (TabGroup.SelectedTab.TabType)
            {
                case TabType.Quest:
                    if (AvailableQuests.Quests.Count <= 0) return;
                    else break;
                case TabType.Mission:
                    if (AvailableMissions.Missions.Count <= 0) return;
                    else break;

            }

            promptIndex = 0;
            PromptConfirmSelection.SetActive(true);
            for (int i = 0; i < PromptButton.Count; i++)
            {
                PromptButton[i].SetColor();
            }
            var button = PromptButton[promptIndex];
            button.SetColor(button.hoverColor);
            Actions.FindActionMap(inputName).FindAction("Next Tab").Disable();
            Actions.FindActionMap(inputName).FindAction("Previous Tab").Disable();
            return;
        }
        if (promptIndex == 0)
        {
            ConfirmFunction();
            return;
        }
        CancelFunction();

    }
    public void ConfirmFunction()
    {
        switch (TabGroup.SelectedTab.TabType)
        {
            case TabType.Quest:
                if (Observer.AssignedQuest != null && Observer.AssignedQuest == AvailableQuests.Quests[pageIndex])
                {                    
                    Debug.Log("<color=yellow>Removed Quest</color>");
                    Observer.ResetQuestTaken();
                    CancelFunction();
                    return;
                }
                Observer.AssignQuest(AvailableQuests.Quests[pageIndex]);
                Debug.Log("<color=purple>Assign Quest</color>");
                break;
            case TabType.Mission:
                if (Observer.AssignedMission != null && Observer.AssignedMission == AvailableMissions.Missions[pageIndex])
                {
                    Debug.Log("<color=yellow>Removed Mission</color>");
                    Observer.ResetMissionTaken();
                    CancelFunction();
                    return;
                }
                Observer.AssignMission(AvailableMissions.Missions[pageIndex]);
                Debug.Log("<color=purple>Assign Mission</color>");

                break;
        }
        CancelFunction();

    }
    public void NextPage()
    {
        StopCoroutine(PreviousPageFunction());
        StopCoroutine(NextPageFunction());
        StartCoroutine(NextPageFunction());

    }
    private void NextPage(InputAction.CallbackContext context)
    {

        if (context.started)
        {
            if (PromptConfirmSelection.activeInHierarchy)
            {
                promptIndex = PromptButton.Count - 1;
                var button = PromptButton[promptIndex];
                for (int i = 0; i < PromptButton.Count; i++)
                {
                    PromptButton[i].SetColor();
                }
                promptIndex = PromptButton.Count - 1;
                button.SetColor(button.hoverColor);
                return;
            }
            else if (PromptRemoveSelection.activeInHierarchy)
            {
                promptIndex = PromptRemoveSelectionButton.Count - 1;
                var button = PromptRemoveSelectionButton[promptIndex];
                for (int i = 0; i < PromptRemoveSelectionButton.Count; i++)
                {
                    PromptRemoveSelectionButton[i].SetColor();
                }
                promptIndex = PromptRemoveSelectionButton.Count - 1;
                button.SetColor(button.hoverColor);
                return;
            }
        }
        if (!PromptConfirmSelection.activeInHierarchy & !PromptRemoveSelection.activeInHierarchy)  
        {
            if (context.performed)
            {
                NextPageButton.SetColor(NextPageButton.hoverColor);
                StartCoroutine(NextPageFunction(context));
                return;
            }
            else if (context.started)
            {
                NextPageButton.SetColor(NextPageButton.hoverColor);
                StartCoroutine(NextPageFunction(context));
                return;
            }
        }
        NextPageButton.SetColor();
        StopCoroutine(NextPageFunction());


    }
    private IEnumerator NextPageFunction()
    {
        var quest = AvailableQuests.Quests;
        var mission = AvailableMissions.Missions;
        switch (TabGroup.SelectedTab.TabType)
        {
            case TabType.Quest:
                if (pageIndex == quest.Count - 1)
                {
                    pageIndex = 0;
                    break;
                }
                pageIndex++;
                break;
            case TabType.Mission:
                if (pageIndex == mission.Count - 1)
                {
                    pageIndex = 0;
                    break;
                }
                pageIndex++;
                break;
        }
        ShowPage();
        yield break;

    }

    private IEnumerator NextPageFunction(InputAction.CallbackContext context)
    {
        var quest = AvailableQuests.Quests;
        var mission = AvailableMissions.Missions;
        if (context.performed)
        {
            while (context.performed)
            {
             
                switch (TabGroup.SelectedTab.TabType)
                {
                    case TabType.Quest:
                        if (pageIndex == quest.Count - 1)
                        {
                            pageIndex = 0;
                            break;
                        }
                        pageIndex++;
                        break;
                    case TabType.Mission:
                        if (pageIndex == mission.Count - 1)
                        {
                            pageIndex = 0;
                            break;
                        }
                        pageIndex++;
                        break;
                }
                ShowPage();
                yield return new WaitForSeconds(0.25f);
            }
            yield break;
        }
        switch (TabGroup.SelectedTab.TabType)
        {
            case TabType.Quest:
                if (pageIndex >= quest.Count - 1)
                {
                    pageIndex = 0;
                    break;
                }
                pageIndex++;
                break;
            case TabType.Mission:
                if (pageIndex >= mission.Count - 1)
                {
                    pageIndex = 0;
                    break;
                }
                pageIndex++;
                break;
        }
        ShowPage();
        yield break;

    }
    public void PreviousPage()
    {
        StopCoroutine(PreviousPageFunction());
        StopCoroutine(NextPageFunction());
        StartCoroutine(PreviousPageFunction());
    }
    public void PreviousPage(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (PromptConfirmSelection.activeInHierarchy)
            {
                promptIndex = 0;
                var button = PromptButton[promptIndex];
                for (int i = 0; i < PromptButton.Count; i++)
                {
                    PromptButton[i].SetColor();
                }
                button.SetColor(button.hoverColor);
                return;
            }
            else if (PromptRemoveSelection.activeInHierarchy)
            {
                promptIndex = 0;
                var button = PromptRemoveSelectionButton[promptIndex];
                for (int i = 0; i < PromptRemoveSelectionButton.Count; i++)
                {
                    PromptRemoveSelectionButton[i].SetColor();
                }
                button.SetColor(button.hoverColor);
                return;
            }
        }

        if (!PromptConfirmSelection.activeInHierarchy & !PromptRemoveSelection.activeInHierarchy)
        {
            if (context.performed)
            {

                PrevPageButton.SetColor(PrevPageButton.hoverColor);
                StartCoroutine(PreviousPageFunction(context));
                return;
            }
            else if (context.started)
            {
                PrevPageButton.SetColor(PrevPageButton.hoverColor);
                StartCoroutine(PreviousPageFunction(context));
                return;
            }
        }  

        PrevPageButton.SetColor();
    }
    private IEnumerator PreviousPageFunction()
    {
        var quest = AvailableQuests.Quests;
        var mission = AvailableMissions.Missions;
        switch (TabGroup.SelectedTab.TabType)
        {
            case TabType.Quest:
                if (pageIndex <= 0)
                {
                    pageIndex = quest.Count - 1;
                    break;
                }
                pageIndex--;
                break;
            case TabType.Mission:
                if (pageIndex <= 0)
                {
                    pageIndex = mission.Count - 1;
                    break;
                }
                pageIndex--;
                break;
        }
        ShowPage();
        yield break;

    }

    private IEnumerator PreviousPageFunction(InputAction.CallbackContext context)
    {
        var quest = AvailableQuests.Quests;
        var mission = AvailableMissions.Missions;
        if (context.performed)
        {
            while (context.performed)
            {
                switch (TabGroup.SelectedTab.TabType)
                {
                    case TabType.Quest:
                        if (pageIndex <= 0)
                        {
                            pageIndex = quest.Count - 1;
                            break;
                        }
                        pageIndex--;
                        break;
                    case TabType.Mission:
                        if (pageIndex <= 0)
                        {
                            pageIndex = mission.Count - 1;
                            break;
                        }
                        pageIndex--;
                        break;
                }
                ShowPage();
                yield return new WaitForSeconds(0.25f);
            }
            yield break;
        }
        switch (TabGroup.SelectedTab.TabType)
        {
            case TabType.Quest:
                if (pageIndex <= 0)
                {
                    pageIndex = quest.Count - 1;
                    break;
                }
                pageIndex--;
                break;
            case TabType.Mission:
                if (pageIndex <= 0)
                {
                    pageIndex = mission.Count - 1;
                    break;
                }
                pageIndex--;
                break;
        }
        ShowPage();
        yield break;

    }

    public void ShowPage()
    {
        ClearMark.SetActive(false);
        switch (TabGroup.SelectedTab.TabType)
        {
            case TabType.Quest:
                PageText.text = $"{pageIndex+1} / {AvailableQuests.Quests.Count}";
                if (AvailableQuests.Quests.Count <= 0)
                {
                    QuestUICanvas.SetActive(false);
                    return;
                }
                QuestUICanvas.SetActive(true);

                var quest = AvailableQuests.Quests[pageIndex].QuestInfo;
                var questReward = AvailableQuests.Quests[pageIndex].Rewards;

                QuestUI.questName.text = quest.QuestName;
                QuestUI.clientName.text = quest.ClientName;
                QuestUI.questDescription.text = quest.QuestDesc;
                if (questReward.Count > 0)
                {
                    int gold;
                    for (int i = 0; i < questReward.Count; i++)
                    {
                        if (questReward.GetType() != typeof(SO_MoneyReward))
                        {
                            continue;
                        }
                        var goldReward = questReward[i] as SO_MoneyReward;
                        gold = goldReward.Amount;
                        QuestUI.goldReward.text = gold.ToString();
                    }
                }
                break;
            case TabType.Mission:
                PageText.text = $"{pageIndex+1} / {AvailableMissions.Missions.Count}";
                if (AvailableMissions.Missions.Count <= 0)
                {
                    MissionUICanvas.SetActive(false);
                    return;
                }
                MissionUICanvas.SetActive(true);

                var mission = AvailableMissions.Missions[pageIndex];
                if (Observer.MissionObserver.Completion.Contains(mission)) ClearMark.SetActive(true);
                if (mission.StageInfo.Icon != null)
                {
                    MissionUI.Icon.sprite = mission.StageInfo.Icon;
                    MissionUI.Icon2.sprite = mission.StageInfo.Icon;
                    MissionUI.Icon.color = Color.white;
                    MissionUI.Icon2.color = Color.white;

                }
                else
                {
                    MissionUI.Icon.color = Color.clear;
                    MissionUI.Icon2.color = Color.clear;
                }
                MissionUI.astralName.text = mission.AstralEntity.name;
                MissionUI.missionName.text = mission.MissionName;
                MissionUI.stageName.text = mission.StageInfo.Name;
                MissionUI.stageDescription.text = mission.StageInfo.Description;

                var missionReward = AvailableMissions.Missions[pageIndex].Rewards;
                if (missionReward.Count > 0)
                {
                    int gold;
                    for (int i = 0; i < missionReward.Count; i++)
                    {
                        if (missionReward.GetType() != typeof(SO_MoneyReward))
                        {
                            continue;
                        }
                        var goldReward = missionReward[i] as SO_MoneyReward;
                        gold = goldReward.Amount;
                        MissionUI.goldReward.text = gold.ToString();
                    }
                }
                break;
        }
    }
    public void OnChangeTab()
    {
        pageIndex = 0;
        ShowPage();
    }
    [TriInspector.Button("Debug Raise : Open Listing")]
    public void OnListingOpen()
    {
        ReadText();
        ChangeStateEvent.Raise(state);
        ListingUICanvas.SetActive(true);
        ShowPage();
    }
    public void ReadText()
    {
        OperatorText.maxVisibleCharacters = 0;
        StartCoroutine(Read());

        IEnumerator Read()
        {
            while(OperatorText.maxVisibleCharacters < OperatorText.text.Length)
            {
               OperatorText.maxVisibleCharacters++;
               yield return new WaitForSeconds(1f / 30);

            }
        }
    }
}
[System.Serializable]
public class QuestListingUI
{
    public TextMeshProUGUI questName;
    public TextMeshProUGUI clientName;
    public TextMeshProUGUI questDescription;
    public TextMeshProUGUI goldReward;
}
[System.Serializable]
public class MissionListingUI
{
    public Image Icon;
    public Image Icon2;

    public TextMeshProUGUI missionName;
    public TextMeshProUGUI astralName;
    public TextMeshProUGUI stageName;
    public TextMeshProUGUI stageDescription;
    public TextMeshProUGUI goldReward;
}
[System.Serializable]
public class AssignedMissionUI
{
    public TextMeshProUGUI missionName;
}
[System.Serializable]
public class AssignedQuestUI
{
    public TextMeshProUGUI questName;
}