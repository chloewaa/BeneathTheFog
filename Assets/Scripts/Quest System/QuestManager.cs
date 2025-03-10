using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private bool loadQuestState = true;
    private Dictionary<string, Quest> questMap;

    //Quest start requirments
    private int currentPlayerLevel;

    private void Awake() {
        questMap = CreateQuestMap();

        // Example: Retrieve a specific quest (for testing or initialization)
        Quest quest = GetQuestById("CollectCoinsQuest"); 
    }
    
    private void OnEnable() {
        // Check that GameEventsManager and its questEvents are initialized
        if (GameEventsManager.instance == null)
        {
            Debug.LogError("GameEventsManager.instance is null! Make sure a GameEventsManager is in the scene.");
            return;
        }
        if (GameEventsManager.instance.questEvents == null)
        {
            Debug.LogError("GameEventsManager.instance.questEvents is null!");
            return;
        }

        // Subscribe to quest events
        GameEventsManager.instance.questEvents.onStartQuest += StartQuest;
        GameEventsManager.instance.questEvents.onAdvanceQuest += AdvanceQuest;
        GameEventsManager.instance.questEvents.onFinishQuest += FinishQuest;

        GameEventsManager.instance.questEvents.onQuestStepStateChange += QuestStepStateChange;

        GameEventsManager.instance.playerEvents.onPlayerLevelChange += PlayerLevelChange;
    }

    private void OnDisable() {
        GameEventsManager.instance.questEvents.onStartQuest -= StartQuest;
        GameEventsManager.instance.questEvents.onAdvanceQuest -= AdvanceQuest;
        GameEventsManager.instance.questEvents.onFinishQuest -= FinishQuest;
        GameEventsManager.instance.playerEvents.onPlayerLevelChange -= PlayerLevelChange;
        GameEventsManager.instance.questEvents.onQuestStepStateChange -= QuestStepStateChange;
    }

    private void Start() {
        // Broadcast the initial state of all quests
        if (GameEventsManager.instance == null || GameEventsManager.instance.questEvents == null)
        {
            Debug.LogError("Cannot broadcast quest states because GameEventsManager or questEvents is null.");
            return;
        }

        foreach (Quest quest in questMap.Values) 
        {
            if(quest.state == QuestState.IN_PROGRESS) {
                quest.InstantiateCurrentQuestStep(this.transform);
            }
            GameEventsManager.instance.questEvents.QuestStateChange(quest);
        }
    }

    private void ChangeQuestState(string id, QuestState state) {
        Quest quest = GetQuestById(id);
        quest.state = state;
        GameEventsManager.instance.questEvents.QuestStateChange(quest);
    }

    private void PlayerLevelChange(int level) {
        currentPlayerLevel = level;
    }

    private bool CheckRequirmentsMet(Quest quest) {
        bool meetsRequirements = true;

        //check level requirments
        if(currentPlayerLevel < quest.info.levelRequirement) {
            meetsRequirements = false;
        }

        // check quest prerequisites for completion
        foreach (QuestInfoSO prerequisiteQuestInfo in quest.info.questPrerequisites)
        {
            if (GetQuestById(prerequisiteQuestInfo.id).state != QuestState.FINISHED)
            {
                meetsRequirements = false;
                // add this break statement here so that we don't continue on to the next quest, since we've proven meetsRequirements to be false at this point.
                break;
            }
        }

        return meetsRequirements;
    }

    private void Update() {
        //loop through all quests
        foreach (Quest quest in questMap.Values) {
            //if we're now meeting the requirments, swith the state to CAN_START
            if(quest.state == QuestState.REQUIREMENTS_NOT_MET && CheckRequirmentsMet(quest)) {
                ChangeQuestState(quest.info.id, QuestState.CAN_START);
            }
        }
    }

    private void StartQuest(string id) { 
        Quest quest = GetQuestById(id);
        quest.InstantiateCurrentQuestStep(this.transform);
        ChangeQuestState(quest.info.id, QuestState.IN_PROGRESS);
    }

    private void AdvanceQuest(string id) {
        Quest quest = GetQuestById(id);

        //Move on to the next step
        quest.MoveToNextStep();

        //if there are any more steps, instantiate the next one
        if(quest.CurrentStepExists()) {
            quest.InstantiateCurrentQuestStep(this.transform);
        } else {
            ChangeQuestState(quest.info.id, QuestState.CAN_FINISH);
        }
    }

    private void FinishQuest(string id) {
        Quest quest = GetQuestById(id);
        ClaimRewards(quest);
        ChangeQuestState(quest.info.id, QuestState.FINISHED);
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
        // Loads all QuestInfoSO Scriptable Objects under Assets/Resources/Quests folder
        QuestInfoSO[] allQuests = Resources.LoadAll<QuestInfoSO>("Quests");
        
        // Create the quest map from the loaded scriptable objects.
        Dictionary<string, Quest> questMap = new Dictionary<string, Quest>();
        foreach (QuestInfoSO questInfo in allQuests) 
        {
            if (questMap.ContainsKey(questInfo.id)) 
            {
                Debug.LogWarning("Duplicate ID found when creating quest map: " + questInfo.id);
            } 
            else 
            {
                questMap.Add(questInfo.id, LoadQuest(questInfo));
            }
        }
        return questMap;
    }

    private Quest GetQuestById(string id) {
        if (questMap.TryGetValue(id, out Quest quest)) 
        {
            return quest;
        }
        Debug.LogError("ID not found in the Quest Map: " + id);
        return null;
    }

    private void OnApplicationQuit() {
        foreach (Quest quest in questMap.Values) {
            SaveQuest(quest);
        }
    }

    private void SaveQuest(Quest quest) {
        try {
            QuestData questData = quest.GetQuestData();
            string serializedData = JsonUtility.ToJson(questData);
            //Change this to save to a file or database in the future
            PlayerPrefs.SetString(quest.info.id, serializedData);

            Debug.Log("Saved quest: " + quest.info.id);
        }
        catch (System.Exception e) {
            Debug.LogError("Error saving quest: " + e.Message);
        }
    }

    private Quest LoadQuest(QuestInfoSO questInfo) {
        Quest quest = null;
        try {
            //load quest from save data
            if(PlayerPrefs.HasKey(questInfo.id) && loadQuestState) {
                string serializedData = PlayerPrefs.GetString(questInfo.id);
                QuestData questData = JsonUtility.FromJson<QuestData>(serializedData);
                quest = new Quest(questInfo, questData.state, questData.questStepIndex, questData.questStepStates);
            } else {
                //create a new quest
                quest = new Quest(questInfo);
            }
        }
        catch (System.Exception e) {
            Debug.LogError("Error loading quest: " + e.Message);
        }
        return quest;
    }
}
