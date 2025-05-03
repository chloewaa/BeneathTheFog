using UnityEngine;


public class InteractPrompt : MonoBehaviour {
    [Header("UI Prompt")]
    public GameObject promptUI;

    [Header("Bool")]
    private bool isPlayerInRange = false;

    void Start() {
        //Hide the prompt at launch 
        promptUI.SetActive(false);
    }

    void Update() {
        if(isPlayerInRange && Input.GetKeyDown(KeyCode.E)) {
            //Hide the prompt 
            promptUI.SetActive(false);
            isPlayerInRange = false;

            Activate();
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player")) {
            isPlayerInRange = true;
            promptUI.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if(other.CompareTag("Player")) {
            isPlayerInRange = false;
            promptUI.SetActive(false);
        }
    }

    private void Activate() {
        Debug.Log("UI prompt active"); 
    }
}
