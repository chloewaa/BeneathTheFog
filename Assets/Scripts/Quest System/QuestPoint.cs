using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class QuestPoint : MonoBehaviour {
    [Header("Dialogue")]
    [SerializeField] private string dialogueknotName;

    [Header("Quest")]
    [SerializeField] private QuestInfoSO questInfoForPoint;

    [Header("Config")]
    [SerializeField] private bool startPoint = true; 
    [SerializeField] private bool finishPoint = true;

    private bool playerIsNear = false; 
    private string questID;
    private QuestState currentQuestState;

    private void Awake() {
        if (questInfoForPoint == null) {
            Debug.LogError("questInfoForPoint is not assigned on " + gameObject.name);
            return;
        }
        questID = questInfoForPoint.id;
    }

    private void OnEnable() {
        if(GameEventsManager.instance == null) {
            Debug.LogError("GameEventsManager.instance is null! Make sure a GameEventsManager is in the scene.");
            return;
        }
        if(GameEventsManager.instance.questEvents == null) {
            Debug.LogError("GameEventsManager.instance.questEvents is null!");
        }
        if(GameEventsManager.instance.inputEvents == null) {
            Debug.LogError("GameEventsManager.instance.inputEvents is null!");
        }

        //Subscribe to the quest state change event.
        GameEventsManager.instance.questEvents.onQuestStateChange += QuestStateChange;
        //Subscribe to the input submit event.
        GameEventsManager.instance.inputEvents.onSubmitPressed += SubmitPressed;
    }

    private void OnDisable() {
        if(GameEventsManager.instance != null) {
            if(GameEventsManager.instance.questEvents != null)
                GameEventsManager.instance.questEvents.onQuestStateChange -= QuestStateChange;
            if(GameEventsManager.instance.inputEvents != null)
                GameEventsManager.instance.inputEvents.onSubmitPressed -= SubmitPressed;
        }
    }

    //This method is called when the input events script fires the submit event.
    private void SubmitPressed(InputEventContext inputEventContext) {
        if(!playerIsNear || !inputEventContext.Equals(InputEventContext.DEFAULT)) {
            return;
        }

        //if the knot name is defined, try to start the dialogue
        if(!dialogueknotName.Equals("")) {
            GameEventsManager.instance.dialogueEvents.EnterDialogue(dialogueknotName);
        }

        //Otherwise, start or finish the quest
        else{ 
            if(currentQuestState.Equals(QuestState.CAN_START) && startPoint) {
                GameEventsManager.instance.questEvents.StartQuest(questID);
            } else if(currentQuestState.Equals(QuestState.CAN_FINISH) && finishPoint) {
                GameEventsManager.instance.questEvents.FinishQuest(questID);
            }
        }
    }

    private void QuestStateChange(Quest quest) { 
        if(quest.info.id.Equals(questID)) {
            currentQuestState = quest.state;
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player"))
            playerIsNear = true;
    }

    private void OnTriggerExit2D(Collider2D other) {
        if(other.CompareTag("Player"))
            playerIsNear = false;
    }
}
