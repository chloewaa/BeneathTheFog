using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 
using Ink.Runtime; 

public class DialogueEvents : MonoBehaviour {
    public event Action<string> onEnterDialogue;
    public void EnterDialogue(string knotName) {
        onEnterDialogue?.Invoke(knotName);
    }

    public event Action onDialogueStarted;
    public void DialogueStarted() {
        onDialogueStarted?.Invoke();
    }

    public event Action onDialogueFinished;
    public void DialogueFinished() {
        onDialogueFinished?.Invoke();
    }

    public event Action<string, List<Choice>> onDisplayDialogue;
    public void DisplayDialogue(string dialogueLine, List<Choice> dialogueChoices) {
        if (onDisplayDialogue != null) {
            onDisplayDialogue(dialogueLine, dialogueChoices);
        }
    }

    public event Action<int> onUpdateChoiceIndex;
    public void UpdateChoiceIndex(int choiceIndex) {
        onUpdateChoiceIndex?.Invoke(choiceIndex);
    }

    public event Action<string, Ink.Runtime.Object> onUpdateInkDialogueVariable;
    public void UpdateInkDialogueVariable(string name, Ink.Runtime.Object value) {
        if (onUpdateInkDialogueVariable != null) {
            onUpdateInkDialogueVariable(name, value);
        }
    }
}
