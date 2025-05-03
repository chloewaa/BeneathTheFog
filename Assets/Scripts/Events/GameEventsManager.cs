using UnityEngine;

public class GameEventsManager : MonoBehaviour {
    public static GameEventsManager instance { get; private set; }

    [Header("Event Scripts")]
    public InputEvents inputEvents;
    public GoldEvents goldEvents;
    public MiscEvents miscEvents;
    public DialogueEvents dialogueEvents;
    public QuestEvents questEvents;
    public PlayerEvents playerEvents;

    private void Awake() {
        // Correct singleton check
        if (instance != null && instance != this) {
            Debug.LogError("More than one Game Events Manager in the scene. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }
        instance = this;
        //persist this object across scenes:
        DontDestroyOnLoad(gameObject);

        // For MonoBehaviour components, use GetComponent instead of new.
        inputEvents = GetComponent<InputEvents>();
        if (inputEvents == null) {
            Debug.LogError("InputEvents component not found on GameEventsManager.");
        }

        // Initialize other events.
        goldEvents = new GoldEvents();
        miscEvents = new MiscEvents();
        dialogueEvents = new DialogueEvents();
        questEvents = new QuestEvents();
        playerEvents = new PlayerEvents();
    }
}
