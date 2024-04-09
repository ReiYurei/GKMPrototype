using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
[RequireComponent(typeof(EventListenerComponent))]
public class ListingUIController : MonoBehaviour
{
    [field: SerializeField] public SO_CompletionObserver Observer { get; private set; }
    [field: SerializeField] public GameObject ListingUICanvas { get; private set; } 
    [field: SerializeField] public SO_ParameterGameEvent CutsceneStart { get; private set; }
    [field: SerializeField] public SO_StoryDialogue GoodByeDialogue { get; private set; }
    [field: SerializeField] public SO_QuestListing AvailableQuests { get; private set; }
    [field: SerializeField] public QuestListingUI QuestUI { get; private set; }
    [field: SerializeField] public SO_MissionListing AvailableMissions { get; private set; }
    [field: SerializeField] public MissionListingUI MissionUI { get; private set; }
    [field:SerializeField] public TabGroup TabGroup { get; private set; }
    [field: SerializeField] public CustomTabButton NextPageButton { get; private set; }
    [field: SerializeField] public CustomTabButton PrevPageButton { get; private set; }
    [field: SerializeField] public GameObject PromptTextBox { get; private set; }
    [field: SerializeField] public List<CustomTabButton> PromptButton { get; private set; }
    [field: SerializeField] public InputActionAsset Actions { get; private set; }
    [field: SerializeField] public TextMeshProUGUI PageText { get; private set; }

    private int pageIndex;
    private int promptIndex;
    public void Awake()
    {
        Actions.FindActionMap("Listing UI").Enable();

        Actions.FindActionMap("Listing UI").FindAction("Next Page").started += NextPage;
        Actions.FindActionMap("Listing UI").FindAction("Previous Page").started += PreviousPage;

        Actions.FindActionMap("Listing UI").FindAction("Next Page").canceled += NextPage;
        Actions.FindActionMap("Listing UI").FindAction("Previous Page").canceled += PreviousPage;

        Actions.FindActionMap("Listing UI").FindAction("Next Page").performed += NextPage;
        Actions.FindActionMap("Listing UI").FindAction("Previous Page").performed += PreviousPage;

        Actions.FindActionMap("Listing UI").FindAction("Confirm").performed += Confirm;
        Actions.FindActionMap("Listing UI").FindAction("Return").performed += Cancel;


    }



    private void OnDisable()
    {
        Actions.FindActionMap("Listing UI").FindAction("Next Page").canceled -= NextPage;
        Actions.FindActionMap("Listing UI").FindAction("Previous Page").canceled -= PreviousPage;

        Actions.FindActionMap("Listing UI").FindAction("Next Page").started -= NextPage;
        Actions.FindActionMap("Listing UI").FindAction("Previous Page").started -= PreviousPage;

        Actions.FindActionMap("Listing UI").FindAction("Next Page").performed -= NextPage;
        Actions.FindActionMap("Listing UI").FindAction("Previous Page").performed -= PreviousPage;
        Actions.FindActionMap("Listing UI").FindAction("Confirm").performed -= Confirm;
        Actions.FindActionMap("Listing UI").FindAction("Return").performed -= Cancel;
    }
    private void Cancel(InputAction.CallbackContext context)
    {
        CancelFunction();
    }
    public void CancelFunction()
    {
        if (PromptTextBox.activeInHierarchy)
        {
            promptIndex = 0;
            PromptTextBox.SetActive(false);
            Actions.FindActionMap("Listing UI").FindAction("Next Tab").Enable();
            Actions.FindActionMap("Listing UI").FindAction("Previous Tab").Enable();
            return;
        }
        Actions.FindActionMap("Listing UI").Disable();
        CutsceneStart.Raise(GoodByeDialogue);
        ListingUICanvas.SetActive(false);
    }

    private void Confirm(InputAction.CallbackContext context)
    {
        if (!PromptTextBox.activeInHierarchy)
        {
            promptIndex = 0;
            PromptTextBox.SetActive(true);
            Actions.FindActionMap("Listing UI").FindAction("Next Tab").Disable();
            Actions.FindActionMap("Listing UI").FindAction("Previous Tab").Disable();
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
                Observer.AssignQuest(AvailableQuests.Quests[pageIndex]);
                Debug.Log("Assign Quest");
                break;
            case TabType.Mission:
                Observer.AssignMission(AvailableMissions.Missions[pageIndex]);
                Debug.Log("Assign Mission");

                break;
        }
        CancelFunction();

    }
    public void NextPage()
    {
        StopAllCoroutines();
        StartCoroutine(NextPageFunction());
        Debug.Log("Next");

    }
    private void NextPage(InputAction.CallbackContext context)
    {

        if (PromptTextBox.activeInHierarchy && context.started)
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
        if (!PromptTextBox.activeInHierarchy && context.performed)
        {
            NextPageButton.SetColor(NextPageButton.hoverColor);
            Debug.Log("Performed");
            StartCoroutine(NextPageFunction(context));
            return;
        }
        else if (context.started)
        {
            Debug.Log("started");
            NextPageButton.SetColor(NextPageButton.hoverColor);
            StartCoroutine(NextPageFunction(context));
            return;
        }
        NextPageButton.SetColor();
        StopAllCoroutines();


    }
    private IEnumerator NextPageFunction()
    {
        var quest = AvailableQuests.Quests;
        var mission = AvailableMissions.Missions;
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
        StopAllCoroutines();
        StartCoroutine(PreviousPageFunction());
        Debug.Log("Prev");
    }
    public void PreviousPage(InputAction.CallbackContext context)
    {
        if (PromptTextBox.activeInHierarchy && context.started)
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

        if (!PromptTextBox.activeInHierarchy && context.performed)
        {
            PrevPageButton.SetColor(PrevPageButton.hoverColor);
            Debug.Log("Performed");
            StartCoroutine(PreviousPageFunction(context));
            return;
        }
        else if (context.started)
        {
            Debug.Log("started");
            PrevPageButton.SetColor(PrevPageButton.hoverColor);
            StartCoroutine(PreviousPageFunction(context));
            return;
        }
        PrevPageButton.SetColor();
        StopAllCoroutines();

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
        switch (TabGroup.SelectedTab.TabType)
        {
            case TabType.Quest:
                PageText.text = $"{pageIndex+1} / {AvailableQuests.Quests.Count}";
                if (AvailableQuests.Quests.Count <= 0) return;
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
                if (AvailableMissions.Missions.Count <= 0) return;
                var mission = AvailableMissions.Missions[pageIndex];
                MissionUI.astralName.text = mission.AstralEntity.name;
                MissionUI.missionName.text = mission.MissionName;
                MissionUI.stageName.text = mission.StageInfo.Name;
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
                        QuestUI.goldReward.text = gold.ToString();
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
    public void OnListingOpen()
    {
        ListingUICanvas.SetActive(true);
    }
}
[System.Serializable]
public class QuestListingUI
{
    public TextMeshProUGUI questName;
    public TextMeshProUGUI clientName;
    public TextMeshProUGUI questDescription;
    public TextMeshProUGUI expReward;
    public TextMeshProUGUI goldReward;
}
[System.Serializable]
public class MissionListingUI
{
    public Image Icon;
    public TextMeshProUGUI missionName;
    public TextMeshProUGUI astralName;
    public TextMeshProUGUI stageName;
    public TextMeshProUGUI expReward;
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