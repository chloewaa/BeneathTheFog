using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //Movement speed
    [SerializeField] private float moveSpeed = 5f; 
    //Current input vector
    private Vector2 moveInput;                       
    private Rigidbody2D rb;                          
    private PlayerInputActions inputActions;         
    //Reference to the Animator component
    private Animator animator;                       

    private void Awake() {
        //Initialize input actions, Rigidbody2D, and Animator
        inputActions = new PlayerInputActions();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void OnEnable() {
        //Enable the Player action map
        inputActions.Player.Enable();
    }

    private void OnDisable() {
        //Disable the Player action map
        inputActions.Player.Disable();
    }

    private void Update() {
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
}
