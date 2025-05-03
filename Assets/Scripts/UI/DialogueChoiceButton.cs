using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DialogueChoiceButton : MonoBehaviour {
    [Header("Components")]
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI choiceText;
    [SerializeField] private Image background;
    [SerializeField] private Color normalColor = Color.black;
    [SerializeField] private Color selectedColor = new Color(0.2f, 0.2f, 0.2f);

    private int choiceIndex = -1;

    private void Awake() {
        //Make sure button click is properly wired up
        button.onClick.AddListener(OnButtonClick);
    }

    public void SetChoiceText(string choiceTextString) {
        choiceText.text = choiceTextString;
    }

    public void SetChoiceIndex(int choiceIndex) {
        this.choiceIndex = choiceIndex;
    }

    public void SelectButton() {
        button.Select();
    }

    public void OnSelect(BaseEventData eventData) {
        GameEventsManager.instance.dialogueEvents.UpdateChoiceIndex(choiceIndex);
        //Visual feedback when selected
        UpdateVisualState(true);
    }
    
    public void OnButtonClick() {
        //Update the choice index
        GameEventsManager.instance.dialogueEvents.UpdateChoiceIndex(choiceIndex);
        //Simulate submit press to continue story with this choice
        GameEventsManager.instance.inputEvents.SubmitPressed();
    }
    
    public void UpdateVisualState(bool selected) {
        if (background != null) {
            background.color = selected ? selectedColor : normalColor;
        }
    }
    
    public void OnDeselect(BaseEventData eventData) {
        UpdateVisualState(false);
    }
}
