using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    private bool dialoguePlaying = false;

    private void OnEnable() {
        GameEventsManager.instance.dialogueEvents.onEnterDialogue += EnterDialogue;
    }

    private void OnDisable() {
        GameEventsManager.instance.dialogueEvents.onEnterDialogue -= EnterDialogue;
    }

    private void EnterDialogue(string knotName) {
        //don't enter dialogue if we've already entered
        if(dialoguePlaying) {
            return;
        }
        
        dialoguePlaying = true;

        Debug.Log("Entering dialogue: " + knotName);
    }
}
