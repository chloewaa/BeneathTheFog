using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class QuestLogUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private GameObject contentParent;
    [SerializeField] private QuestLogScrollingList scrollingList;
    [SerializeField] private TextMeshProUGUI questDisplayName;
    [SerializeField] private TextMeshProUGUI questStatusText;
    [SerializeField] private TextMeshProUGUI exprienceRewardsText;
    [SerializeField] private TextMeshProUGUI levelRequirementText;
    [SerializeField] private TextMeshProUGUI questRequirementsText;

    private Button firstSelectedButton;

    private void Awake()
    {
        // Start with the quest log UI disabled.
        if (contentParent != null)
            contentParent.SetActive(false);
    }

    private void OnEnable() {
        if(GameEventsManager.instance == null)
        {
            Debug.LogError("GameEventsManager.instance is null! Ensure a GameEventsManager is in the scene.");
            return;
        }

        GameEventsManager.instance.questEvents.onQuestStateChange += QuestStateChange;
    }

    private void OnDisable() {
        if(GameEventsManager.instance != null && GameEventsManager.instance.questEvents != null)
            GameEventsManager.instance.questEvents.onQuestStateChange -= QuestStateChange;
    }

    private void Update()
    {
        // Poll for Q key press using the new Input System.
        if (Keyboard.current != null && Keyboard.current.qKey.wasPressedThisFrame)
        {
            ToggleUI();
        }
    }

    private void ToggleUI() {
        if (contentParent.activeInHierarchy)
        {
            HideUI();
        }
        else {
            ShowUI();
        }
    }

    private void ShowUI() {
        contentParent.SetActive(true);
        GameEventsManager.instance.playerEvents.DisablePlayerMovement();
        if (firstSelectedButton != null)
            firstSelectedButton.Select();
    }

    private void HideUI() {
        contentParent.SetActive(false);
        GameEventsManager.instance.playerEvents.EnablePlayerMovement();
        EventSystem.current.SetSelectedGameObject(null);
    }

    private void QuestStateChange(Quest quest) {
        // Add or update the quest button in the scrolling list.
        QuestLogButton questLogButton = scrollingList.CreateButtonIfNotExists(quest, () => SetQuestLogInfo(quest));

        // Set the first selected button if it hasn't been set yet.
        if (firstSelectedButton == null) {
            firstSelectedButton = questLogButton.button;
        }

        questLogButton.SetState(quest.state); 
    }

    private void SetQuestLogInfo(Quest quest) {
        //Quest Name 
        questDisplayName.text = quest.info.displayName;
        //Status
        questStatusText.text = quest.GetFullStatusText();
        //Level Requirement
        levelRequirementText.text = "Level: " + quest.info.levelRequirement;
        questRequirementsText.text = "Requirements: ";
        foreach (QuestInfoSO prerequisiteQuestInfo in quest.info.questPrerequisites)
        {
            questRequirementsText.text += prerequisiteQuestInfo.displayName + "\n ";
        }
        exprienceRewardsText.text = quest.info.experienceReward + " XP";
        // Optionally update questStatusText here if needed.
    }
}
