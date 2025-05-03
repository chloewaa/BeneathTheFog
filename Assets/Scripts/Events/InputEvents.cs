using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class InputEvents : MonoBehaviour {
    private InputEventContext _inputEventContext = InputEventContext.DEFAULT;
    
    // Add an event for context changes
    public event Action<InputEventContext> onInputContextChanged;
    
    public InputEventContext inputEventContext {
        get { return _inputEventContext; }
        private set {
            if (_inputEventContext != value) {
                _inputEventContext = value;
                onInputContextChanged?.Invoke(_inputEventContext);
            }
        }
    }

    public void ChangeInputEventContext(InputEventContext newContext) {
        this.inputEventContext = newContext;
    }

    // Event declarations
    public event Action<InputEventContext> onSubmitPressed;
    public event Action onQuestLogTogglePressed;

    private InputAction submitAction;
    private InputAction questLogToggleAction;

    private void Awake() {
        submitAction = new InputAction("Submit", binding: "<Keyboard>/e");
        questLogToggleAction = new InputAction("QuestLogToggle", binding: "<Keyboard>/q");
    }

    private void OnEnable() {
        submitAction.Enable();
        submitAction.performed += OnSubmitPerformed;

        questLogToggleAction.Enable();
        questLogToggleAction.performed += OnQuestLogTogglePerformed;
    }

    private void OnDisable() {
        submitAction.performed -= OnSubmitPerformed;
        submitAction.Disable();

        questLogToggleAction.performed -= OnQuestLogTogglePerformed;
        questLogToggleAction.Disable();
    }

    private void OnSubmitPerformed(InputAction.CallbackContext context) {
        if (context.performed) {
            // Invoke the event with current context
            onSubmitPressed?.Invoke(inputEventContext);
        }
    }

    // Public method for invoking submit without a CallbackContext
    public void SubmitPressed() {
        onSubmitPressed?.Invoke(inputEventContext);
    }

    private void OnQuestLogTogglePerformed(InputAction.CallbackContext context) {
        if (context.performed) {
            onQuestLogTogglePressed?.Invoke();
        }
    }
}
