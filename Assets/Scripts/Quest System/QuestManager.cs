using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    // Singleton instance
    public static QuestManager instance { get; private set; }

    [Header("Config")]
    [SerializeField] private bool loadQuestState = true;
    private Dictionary<string, Quest> questMap;

    // Quest start requirements
    private int currentPlayerLevel;

    private void Awake() {
        // Enforce singleton pattern
        if (instance != null && instance != this) {
            Destroy(gameObject);
            return;
        }
        instance = this;

        // Build the quest map
        questMap = CreateQuestMap();

        // (Optional) Example retrieval
        Quest quest = GetQuestById("CollectCoinsQuest");
    }

    private void OnEnable() {
        // Ensure GameEventsManager is ready
        if (GameEventsManager.instance == null || GameEventsManager.instance.questEvents == null) {
            Debug.LogError("GameEventsManager or its questEvents is null! Make sure it's in the scene.");
            return;
        }

        var qe = GameEventsManager.instance.questEvents;
        qe.onStartQuest           += StartQuest;
        qe.onAdvanceQuest         += AdvanceQuest;
        qe.onFinishQuest          += FinishQuest;
        qe.onQuestStepStateChange += QuestStepStateChange;

        GameEventsManager.instance.playerEvents.onPlayerLevelChange += PlayerLevelChange;
    }

    private void OnDisable() {
        if (GameEventsManager.instance != null && GameEventsManager.instance.questEvents != null) {
            var qe = GameEventsManager.instance.questEvents;
            qe.onStartQuest           -= StartQuest;
            qe.onAdvanceQuest         -= AdvanceQuest;
            qe.onFinishQuest          -= FinishQuest;
            qe.onQuestStepStateChange -= QuestStepStateChange;

            GameEventsManager.instance.playerEvents.onPlayerLevelChange -= PlayerLevelChange;
        }
    }

    private void Start() {
        // Broadcast initial state of all quests
        if (GameEventsManager.instance == null || GameEventsManager.instance.questEvents == null) {
            Debug.LogError("Cannot broadcast quest states; GameEventsManager or questEvents is null.");
            return;
        }

        foreach (Quest quest in questMap.Values) {
            if (quest.state == QuestState.IN_PROGRESS) {
                quest.InstantiateCurrentQuestStep(transform);
            }
            GameEventsManager.instance.questEvents.QuestStateChange(quest);
        }
    }

    private void Update() {
        // Auto-unlock quests when requirements are met
        foreach (Quest quest in questMap.Values) {
            if (quest.state == QuestState.REQUIREMENTS_NOT_MET && CheckRequirementsMet(quest)) {
                ChangeQuestState(quest.info.id, QuestState.CAN_START);
            }
        }
    }

    private void ChangeQuestState(string id, QuestState newState) {
        Quest quest = GetQuestById(id);
        quest.state = newState;
        GameEventsManager.instance.questEvents.QuestStateChange(quest);
    }

    private void PlayerLevelChange(int level) {
        currentPlayerLevel = level;
    }

    private bool CheckRequirementsMet(Quest quest) {
        if (currentPlayerLevel < quest.info.levelRequirement) {
            return false;
        }

        foreach (QuestInfoSO prereq in quest.info.questPrerequisites) {
            if (GetQuestById(prereq.id).state != QuestState.FINISHED) {
                return false;
            }
        }
        return true;
    }

    private void StartQuest(string id) {
        Quest quest = GetQuestById(id);
        quest.InstantiateCurrentQuestStep(transform);
        ChangeQuestState(id, QuestState.IN_PROGRESS);
    }

    private void AdvanceQuest(string id) {
        Quest quest = GetQuestById(id);
        quest.MoveToNextStep();

        if (quest.CurrentStepExists()) {
            quest.InstantiateCurrentQuestStep(transform);
        } else {
            ChangeQuestState(id, QuestState.CAN_FINISH);
        }
    }

    private void FinishQuest(string id) {
        Quest quest = GetQuestById(id);
        ClaimRewards(quest);
        ChangeQuestState(id, QuestState.FINISHED);
    }

    private void ClaimRewards(Quest quest) {
        GameEventsManager.instance.playerEvents.ExperienceGained(quest.info.experienceReward);
    }

    private void QuestStepStateChange(string questId, int stepIndex, QuestStepState questStepState) {
        Quest quest = GetQuestById(questId);
        quest.StoreQuestStepState(questStepState, stepIndex);
        ChangeQuestState(questId, quest.state);
    }

    private Dictionary<string, Quest> CreateQuestMap() {
        QuestInfoSO[] allQuests = Resources.LoadAll<QuestInfoSO>("Quests");
        var map = new Dictionary<string, Quest>();

        foreach (QuestInfoSO info in allQuests) {
            if (map.ContainsKey(info.id)) {
                Debug.LogWarning("Duplicate Quest ID: " + info.id);
            } else {
                map.Add(info.id, LoadQuest(info));
            }
        }

        return map;
    }

    private Quest LoadQuest(QuestInfoSO info) {
        if (PlayerPrefs.HasKey(info.id) && loadQuestState) {
            string data = PlayerPrefs.GetString(info.id);
            QuestData qd = JsonUtility.FromJson<QuestData>(data);
            return new Quest(info, qd.state, qd.questStepIndex, qd.questStepStates);
        } else {
            return new Quest(info);
        }
    }

    private void SaveQuest(Quest quest) {
        try {
            QuestData qd = quest.GetQuestData();
            string serialized = JsonUtility.ToJson(qd);
            PlayerPrefs.SetString(quest.info.id, serialized);
        } catch (System.Exception e) {
            Debug.LogError("Error saving quest: " + e.Message);
        }
    }

    private void OnApplicationQuit() {
        foreach (Quest q in questMap.Values) {
            SaveQuest(q);
        }
    }

    private Quest GetQuestById(string id) {
        if (questMap.TryGetValue(id, out Quest quest)) {
            return quest;
        }
        Debug.LogError("Quest ID not found: " + id);
        return null;
    }

    // Expose all quests for UI seeding
    public IEnumerable<Quest> GetAllQuests() {
        return questMap.Values;
    }
}
