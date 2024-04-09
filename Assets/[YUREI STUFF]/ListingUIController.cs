using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
[RequireComponent(typeof(EventListenerComponent))]
public class ListingUIController : MonoBehaviour
{
    [field: SerializeField] public SO_CompletionObserver Observer { get; private set; }
    [field: SerializeField] public SO_QuestListing AvailableQuests { get; private set; }
    [field: SerializeField] public QuestListingUI QuestUI { get; private set; }
    [field: SerializeField] public SO_MissionListing AvailableMissions { get; private set; }
    [field: SerializeField] public MissionListingUI MissionUI { get; private set; }
    [field:SerializeField] public TabGroup TabGroup { get; private set; }
    [field: SerializeField]public InputActionAsset Actions { get; private set; }
    private int pageIndex;
    public void Awake()
    {
        Actions.FindActionMap("Listing UI").Enable();
        Actions.FindActionMap("Listing UI").FindAction("Next Page").performed += NextPage;
        Actions.FindActionMap("Listing UI").FindAction("Previous Page").performed += PreviousPage;

    }

    private void OnDisable()
    {
        Actions.FindActionMap("Listing UI").FindAction("Next Page").performed -= NextPage;
        Actions.FindActionMap("Listing UI").FindAction("Previous Page").performed -= PreviousPage;
    }
    private void NextPage(InputAction.CallbackContext context)
    {
        
    }


    private void PreviousPage(InputAction.CallbackContext context)
    {
    }
    public void ShowPage()
    {
        switch (TabGroup.SelectedTab.TabType)
        {
            case TabType.Quest:
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