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
    [SerializeField] private TextMeshProUGUI experienceRewardsText;
    [SerializeField] private TextMeshProUGUI levelRequirementText;
    [SerializeField] private TextMeshProUGUI questRequirementsText;

    private Button firstSelectedButton;

    private void Awake() {
        // Start hidden
        if (contentParent != null)
            contentParent.SetActive(false);
    }

    private void OnEnable() {
        if (GameEventsManager.instance == null || GameEventsManager.instance.questEvents == null)
        {
            Debug.LogError("GameEventsManager or questEvents is null! Ensure both exist in the scene.");
            return;
        }

        // 1) Listen for any future state changes
        GameEventsManager.instance.questEvents.onQuestStateChange += QuestStateChange;

        // 2) Seed the UI with all quests you already have
        foreach (Quest q in QuestManager.instance.GetAllQuests())
        {
            QuestStateChange(q);
        }
    }

    private void OnDisable() {
        if (GameEventsManager.instance != null && GameEventsManager.instance.questEvents != null)
            GameEventsManager.instance.questEvents.onQuestStateChange -= QuestStateChange;
    }

    private void Update() {
        // Toggle on Q press (new Input System)
        if (Keyboard.current != null && Keyboard.current.qKey.wasPressedThisFrame)
            ToggleUI();
    }

    private void ToggleUI() {
        if (contentParent.activeInHierarchy)
            HideUI();
        else
            ShowUI();
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
        // Create a button if needed, then update its visual state
        QuestLogButton questLogButton = scrollingList.CreateButtonIfNotExists(quest, () => SetQuestLogInfo(quest));

        if (firstSelectedButton == null)
            firstSelectedButton = questLogButton.button;

        questLogButton.SetState(quest.state);
    }

    private void SetQuestLogInfo(Quest quest) {
        questDisplayName.text        = quest.info.displayName;
        questStatusText.text         = quest.GetFullStatusText();
        levelRequirementText.text    = "Level: " + quest.info.levelRequirement;

        // Prerequisites list
        questRequirementsText.text   = "Requirements:\n";
        foreach (QuestInfoSO prereq in quest.info.questPrerequisites)
        {
            questRequirementsText.text += "- " + prereq.displayName + "\n";
        }

        experienceRewardsText.text   = quest.info.experienceReward + " XP";
    }
}
