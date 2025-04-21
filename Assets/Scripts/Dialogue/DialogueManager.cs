using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;

public class DialogueManager : MonoBehaviour
{
    [Header ("Ink Story")]
    [SerializeField] private TextAsset inkJson;
    private Story story;
    private int currentChoiceIndex = -1;
    
    private bool dialoguePlaying = false;

    private InkExternalFunctions inkExternalFunctions;
    private InkDialogueVariables inkDialogueVariables;

    [Header("UI References")]
    [SerializeField] private Transform choiceContainer;
    [SerializeField] private DialogueChoiceButton choiceButtonPrefab;
    
    private List<DialogueChoiceButton> activeChoiceButtons = new List<DialogueChoiceButton>();

    private void Awake() {
        story = new Story(inkJson.text);
        inkExternalFunctions = new InkExternalFunctions();
        inkExternalFunctions.Bind(story);
        inkDialogueVariables = new InkDialogueVariables(story);
    }

    private void OnDestroy() {
        inkExternalFunctions.Unbind(story);
    }

    private void OnEnable() {
        GameEventsManager.instance.dialogueEvents.onEnterDialogue += EnterDialogue;
        GameEventsManager.instance.inputEvents.onSubmitPressed += SubmitPressed;
        GameEventsManager.instance.dialogueEvents.onUpdateChoiceIndex += UpdateChoiceIndex;
        GameEventsManager.instance.dialogueEvents.onUpdateInkDialogueVariable += UpdateInkDialogueVariable;
        GameEventsManager.instance.questEvents.onQuestStateChange += QuestStateChange;
    }

    private void OnDisable() {
        GameEventsManager.instance.dialogueEvents.onEnterDialogue -= EnterDialogue;
        GameEventsManager.instance.inputEvents.onSubmitPressed -= SubmitPressed;
        GameEventsManager.instance.dialogueEvents.onUpdateChoiceIndex -= UpdateChoiceIndex;
        GameEventsManager.instance.dialogueEvents.onUpdateInkDialogueVariable -= UpdateInkDialogueVariable;
        GameEventsManager.instance.questEvents.onQuestStateChange -= QuestStateChange;
    }

    private void QuestStateChange(Quest quest) {
        GameEventsManager.instance.dialogueEvents.UpdateInkDialogueVariable(quest.info.id + "State", (Ink.Runtime.Object) new StringValue(quest.state.ToString()));
    }

    private void UpdateInkDialogueVariable(string name, Ink.Runtime.Object value) {
        inkDialogueVariables.UpdateVariableState(name, value);
    }

    private void UpdateChoiceIndex(int choiceIndex) {
        currentChoiceIndex = choiceIndex;
        Debug.Log($"Choice selected: {choiceIndex}");
        
        // Update visual state of buttons
        for (int i = 0; i < activeChoiceButtons.Count; i++) {
            activeChoiceButtons[i].UpdateVisualState(i == choiceIndex);
        }
    }

    private void SubmitPressed(InputEventContext inputEventContext) {
        //if the context is not dialogue, we wont register the submit event
        if(inputEventContext != InputEventContext.DIALOGUE) {
            return;
        }

        ContinueOrExitStory();
    }

    private void EnterDialogue(string knotName) {
        //don't enter dialogue if we've already entered
        if(dialoguePlaying) {
            return;
        }
    
        dialoguePlaying = true;

        //inform other parts of the system that dialogue has started
        GameEventsManager.instance.dialogueEvents.DialogueStarted();

        //input event context
        GameEventsManager.instance.inputEvents.ChangeInputEventContext(InputEventContext.DIALOGUE);

        //jump to the knot
        if(!knotName.Equals("")) {
            story.ChoosePathString(knotName);
        }
        else {
            Debug.LogWarning("knot name was the empty string when entering dialogue.");
        }

        //start listening for variables 
        inkDialogueVariables.SyncVariablesAndStartListening(story);

        //Start the story or exit it 
        ContinueOrExitStory();
    }

    private void ContinueOrExitStory() {
        // Clear any existing choice buttons
        ClearChoices();
        
        // Make a choice if we have one
        if(story.currentChoices.Count > 0 && currentChoiceIndex != -1) {
            Debug.Log($"Selecting choice {currentChoiceIndex} of {story.currentChoices.Count}");
            story.ChooseChoiceIndex(currentChoiceIndex);
            // Reset choice index for next time
            currentChoiceIndex = -1;
        }
        
        if(story.canContinue) {
            string dialogueLine = story.Continue();
            // Display the new dialogue line
            GameEventsManager.instance.dialogueEvents.DisplayDialogue(dialogueLine, story.currentChoices);
            Debug.Log(dialogueLine);
            
            // Create choice buttons if needed
            if(story.currentChoices.Count > 0) {
                CreateChoiceButtons(story.currentChoices);
            }
        }
        else if(story.currentChoices.Count == 0) {
            ExitDialogue();
        }
        else if(story.currentChoices.Count > 0) {
            // We're at a choice point but no choice has been made yet
            CreateChoiceButtons(story.currentChoices);
        }
    }
    
    private void CreateChoiceButtons(List<Choice> choices) {
        // Check if the prefab is assigned
        if (choiceButtonPrefab == null) {
            Debug.LogError("Error: Choice Button Prefab is not assigned in the Inspector! Please assign a DialogueChoiceButton prefab.");
            return;
        }
        
        // Check if the container is assigned
        if (choiceContainer == null) {
            Debug.LogError("Error: Choice Container is not assigned in the Inspector! Please assign a Transform to hold the choice buttons.");
            return;
        }
        
        // Clear any existing buttons
        ClearChoices();
        
        // Create new buttons
        for (int i = 0; i < choices.Count; i++) {
            DialogueChoiceButton choiceButton = Instantiate(choiceButtonPrefab, choiceContainer);
            choiceButton.SetChoiceText(choices[i].text);
            choiceButton.SetChoiceIndex(i);
            activeChoiceButtons.Add(choiceButton);
            
            // Select the first button by default
            if (i == 0) {
                choiceButton.SelectButton();
            }
        }
    }
    
    private void ClearChoices() {
        foreach (var button in activeChoiceButtons) {
            Destroy(button.gameObject);
        }
        activeChoiceButtons.Clear();
    }

    private void ExitDialogue() {
        dialoguePlaying = false;

        //inform other parts of the system that dialogue has finished
        GameEventsManager.instance.dialogueEvents.DialogueFinished();

        //input event context
        GameEventsManager.instance.inputEvents.ChangeInputEventContext(InputEventContext.DEFAULT);

        //Stop listening
        inkDialogueVariables.StopListening(story); 

        //reset the story
        story.ResetState();
    }
}
