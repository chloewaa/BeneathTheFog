using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class InputEvents : MonoBehaviour
{
    // This event is fired when the submit key is pressed.
    public event Action onSubmitPressed;
    // This event is fired when the quest log toggle key is pressed.
    public event Action onQuestLogTogglePressed;

    private InputAction submitAction;
    private InputAction questLogToggleAction;

    private void Awake()
    {
        // Bind the E key to the submit action.
        submitAction = new InputAction("Submit", binding: "<Keyboard>/e");
        // Bind the Q key (for example) to toggle the quest log.
        questLogToggleAction = new InputAction("QuestLogToggle", binding: "<Keyboard>/q");
    }

    private void OnEnable()
    {
        submitAction.Enable();
        submitAction.performed += OnSubmitPerformed;

        questLogToggleAction.Enable();
        questLogToggleAction.performed += OnQuestLogTogglePerformed;
    }

    private void OnDisable()
    {
        submitAction.performed -= OnSubmitPerformed;
        submitAction.Disable();

        questLogToggleAction.performed -= OnQuestLogTogglePerformed;
        questLogToggleAction.Disable();
    }

    private void OnSubmitPerformed(InputAction.CallbackContext context)
    {
        onSubmitPressed?.Invoke();
    }

    private void OnQuestLogTogglePerformed(InputAction.CallbackContext context)
    {
        onQuestLogTogglePressed?.Invoke();
    }
}
