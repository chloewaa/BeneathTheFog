using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //Movement speed
    public float moveSpeed = 5f; 
    //Current input vector
    private Vector2 moveInput;                       
    private Rigidbody2D rb;                          
    private PlayerInputActions inputActions;         
    //Reference to the Animator component
    private Animator animator;                       
    private bool movementEnabled = true;

    private void Awake() {
        //Initialize input actions, Rigidbody2D, and Animator
        inputActions = new PlayerInputActions();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void OnEnable() {
        //Enable the Player action map
        inputActions.Player.Enable();

        // Subscribe to dialogue events
        GameEventsManager.instance.dialogueEvents.onDialogueStarted += DisableMovement;
        GameEventsManager.instance.dialogueEvents.onDialogueFinished += EnableMovement;
        
        GameEventsManager.instance.inputEvents.onInputContextChanged += OnInputContextChanged;
    }

    private void OnDisable() {
        //Disable the Player action map
        inputActions.Player.Disable();

        // Unsubscribe from dialogue events
        GameEventsManager.instance.dialogueEvents.onDialogueStarted -= DisableMovement;
        GameEventsManager.instance.dialogueEvents.onDialogueFinished -= EnableMovement;
        
        // Optional: Also stop listening to input context changes
        GameEventsManager.instance.inputEvents.onInputContextChanged -= OnInputContextChanged;
    }

    private void Update() {
        // Only process movement if enabled
        if (!movementEnabled) return;

        //Poll the input every frame to capture all simultaneous key presses.
        moveInput = inputActions.Player.Move.ReadValue<Vector2>();

        //Update animator parameters based on the current input.
        if (animator != null) {
            animator.SetFloat("Horizontal", moveInput.x);
            animator.SetFloat("Vertical", moveInput.y);
        }
    }

    private void FixedUpdate() {
        //Normalize the input to ensure diagonal movement isn't faster
        rb.linearVelocity = moveInput.normalized * moveSpeed;
    }

    private void DisableMovement()
    {
        movementEnabled = false;
    }
    
    private void EnableMovement()
    {
        movementEnabled = true;
    }
    
    private void OnInputContextChanged(InputEventContext newContext)
    {
        movementEnabled = (newContext == InputEventContext.DEFAULT);
    }
}
