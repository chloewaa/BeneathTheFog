using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class QuestLogScrollingList : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private GameObject contentParent;

    [Header("Rect Transforms")]
    [SerializeField] private RectTransform scrollRectTransform;
    [SerializeField] private RectTransform contentRectTransform;

    [Header("Quest Log Button")]
    [SerializeField] private GameObject questLogButtonPrefab;

    private Dictionary<string, QuestLogButton> idToButtonMap = new Dictionary<string, QuestLogButton>();

    private void Start() {
        QuestLogButton firstButton = null;

        /*for (int i = 0; i < 20; i++) {
            // Create a test QuestInfoSO instance with a unique ID for each quest.
            QuestInfoSO questInfoTest = ScriptableObject.CreateInstance<QuestInfoSO>();
            questInfoTest.id = "test" + i;  // e.g., "test0", "test1", "test2"
            questInfoTest.displayName = "Test " + i;
            questInfoTest.questStepPrefab = new GameObject[0];  // Assuming no quest step prefabs for testing
            Quest quest = new Quest(questInfoTest);

            QuestLogButton questLogButton = CreateButtonIfNotExists(quest, () => Debug.Log("Selected " + quest.info.displayName));
            if (i == 0) {
                firstButton = questLogButton;
            }
        }

        if (firstButton != null) {
            firstButton.button.Select();
        }*/
    }

    public QuestLogButton CreateButtonIfNotExists(Quest quest, UnityAction selectAction) {
        QuestLogButton questLogButton = null;
        // If the quest's button is not already in the dictionary, create and add it.
        if (!idToButtonMap.ContainsKey(quest.info.id)) {
            questLogButton = InstantiateQuestLogButton(quest, selectAction);
        } else {
            questLogButton = idToButtonMap[quest.info.id];
        }
        return questLogButton;
    }

    private QuestLogButton InstantiateQuestLogButton(Quest quest, UnityAction selectAction) {
        // Instantiate the quest log button prefab under the content parent.
        QuestLogButton questLogButton = Instantiate(questLogButtonPrefab, contentParent.transform).GetComponent<QuestLogButton>();
        // Name the GameObject for clarity in the hierarchy.
        questLogButton.gameObject.name = quest.info.id + "_button";
        // Initialize the button with the quest's display name and the select action.
        RectTransform buttonRectTransform = questLogButton.GetComponent<RectTransform>();
        questLogButton.Initialize(quest.info.displayName, () => { selectAction(); UpdateScrolling(buttonRectTransform);});
        // Add the new button to the dictionary.
        idToButtonMap[quest.info.id] = questLogButton;
        return questLogButton;
    }

    private void UpdateScrolling(RectTransform buttonRectTransform) {
        //calculate the min and max for selected button
        float buttonYMin = Mathf.Abs(buttonRectTransform.anchoredPosition.y);
        float buttonYMax = buttonYMin + buttonRectTransform.rect.height;

        //calculate the min and max for the content area
        float contentYMin = Mathf.Abs(contentRectTransform.anchoredPosition.y);
        float contentYMax = contentYMin + contentRectTransform.rect.height;

        //handle scrolling down
        if (buttonYMax > contentYMax) {
            contentRectTransform.anchoredPosition = new Vector2(contentRectTransform.anchoredPosition.x, buttonYMax - contentRectTransform.rect.height);
        }
        //handle scrolling up
        else if (buttonYMin < contentYMin) {
            contentRectTransform.anchoredPosition = new Vector2(contentRectTransform.anchoredPosition.x, buttonYMin);
        }
    }
}
