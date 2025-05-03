using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest
{
    [Header("Static Info")]
    public QuestInfoSO info;

    [Header("State Info")]
    public QuestState state;
    private int currentQuestStepIndex;
    private QuestStepState[] questStepStates;

    // Constructor for a new quest.
    public Quest(QuestInfoSO questInfo) {
        this.info = questInfo;
        this.state = QuestState.REQUIREMENTS_NOT_MET;
        this.currentQuestStepIndex = 0;
        this.questStepStates = new QuestStepState[info.questStepPrefab.Length];
        for (int i = 0; i < questStepStates.Length; i++)
        {
            questStepStates[i] = new QuestStepState();
        }
    }

    // Constructor to load a quest from saved data.
    public Quest(QuestInfoSO questInfo, QuestState questState, int currentQuestStepIndex, QuestStepState[] questStepStates) {
        this.info = questInfo;
        this.state = questState;
        this.currentQuestStepIndex = currentQuestStepIndex;
        this.questStepStates = questStepStates;

        // Warn if the saved quest step states don't match the current number of step prefabs.
        if (this.questStepStates.Length != this.info.questStepPrefab.Length)
        {
            Debug.LogWarning("Quest step states and prefabs are different lengths for quest: " + info.id);
        }
    }

    public void MoveToNextStep() {
        currentQuestStepIndex++;
    }

    public bool CurrentStepExists() {
        return (currentQuestStepIndex < info.questStepPrefab.Length);
    }

    public void InstantiateCurrentQuestStep(Transform parentTransform) {
        GameObject questStepPrefab = GetCurrentQuestStepPrefab();
        if (questStepPrefab != null) {
            QuestStep questStep = UnityEngine.Object.Instantiate(questStepPrefab, parentTransform)
                .GetComponent<QuestStep>();
            questStep.InitializeQuestStep(info.id, currentQuestStepIndex, questStepStates[currentQuestStepIndex].state);
        }
    }

    private GameObject GetCurrentQuestStepPrefab() {
        GameObject questStepPrefab = null;
        if (CurrentStepExists()) {
            questStepPrefab = info.questStepPrefab[currentQuestStepIndex];
        } else {
            Debug.LogWarning("Tried to get quest step prefab but stepIndex was out of range indicating that " +
                "there's no current quest step: QuestID: " + info.id + ", StepIndex: " + currentQuestStepIndex);
        }
        return questStepPrefab;
    }

    public void StoreQuestStepState(QuestStepState questStepState, int stepIndex) {
        if (stepIndex < questStepStates.Length) {
            questStepStates[stepIndex].state = questStepState.state;
            questStepStates[stepIndex].status = questStepState.status;
        } else {
            Debug.LogWarning("Tried to store quest step state but stepIndex was out of range: QuestID: " + info.id + ", StepIndex: " + stepIndex);
        }
    }

    public QuestData GetQuestData() {
        return new QuestData(state, currentQuestStepIndex, questStepStates);
    }

    public string GetFullStatusText() {
        string fullStatus = "";

        if (state == QuestState.REQUIREMENTS_NOT_MET) {
            fullStatus = "Requirements not met";
        } else if (state == QuestState.CAN_START) {
            fullStatus = "This quest can be started";
        } else {
            // Display the status of all quest steps up to the current step.
            for (int i = 0; i < currentQuestStepIndex && i < questStepStates.Length; i++) {
                fullStatus += "<s>" + questStepStates[i].status + "</s>\n";
            }
            // Display the status of the current quest step.
            if (CurrentStepExists() && currentQuestStepIndex < questStepStates.Length) {
                fullStatus += questStepStates[currentQuestStepIndex].status;
            }
            // When the quest is ready to finish or has been finished.
            if (state == QuestState.CAN_FINISH) {
                fullStatus += " The quest is ready to be turned in";
            } else if (state == QuestState.FINISHED) {
                fullStatus += " The quest has been completed";
            }
        }
        return fullStatus;
    }
}
