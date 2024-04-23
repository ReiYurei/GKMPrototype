using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
[RequireComponent(typeof(EventListenerComponent))]
public class AssignedUIController : MonoBehaviour 
{
    [field: SerializeField] public SO_CompletionObserver Observer { get; private set; }
    public static AssignedUIController Instance { get; private set; }
    [field: SerializeField] public GameObject Canvas { get; private set; }

    [field: Header("Quest")]
    [field: SerializeField]public GameObject AssignedQuestCanvas { get; private set; }
    [field: SerializeField]public TextMeshProUGUI QuestName { get; private set; }

    [field: Header("Mission")]
    [field: SerializeField] public GameObject AssignedMissionCanvas { get; private set; }
    [field: SerializeField]public TextMeshProUGUI MissionName { get; private set; }
    private void Awake()
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
    public void OnHubEnter()
    {
        if (Observer.AssignedMission != null) AssignedMissionCanvas.SetActive(true);
        if (Observer.AssignedQuest != null) AssignedQuestCanvas.SetActive(true);
        Canvas.SetActive(true);

    }
    public void DisableCanvas()
    {
        AssignedMissionCanvas.SetActive(false);
        AssignedQuestCanvas.SetActive(false);
        Canvas.SetActive(false);
    }
    public void OnAssignedQuest(ScriptableObject quest)
    {
        var data = quest as SO_QuestData;
        QuestName.text = data.QuestInfo.QuestName;
        AssignedQuestCanvas.SetActive(true);
        Canvas.SetActive(true);

    }
    public void OnAssignedMission(ScriptableObject mission)
    {
        var data = mission as SO_MissionData;
        MissionName.text = data.MissionName;
        AssignedMissionCanvas.SetActive(true);
        Canvas.SetActive(true);

    }
    public void OnAssignedQuestCancel()
    {
        AssignedQuestCanvas.SetActive(false);

    }
    public void OnAssignedMissionCancel()
    {
        AssignedMissionCanvas.SetActive(false);
    }
}
