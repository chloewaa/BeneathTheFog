using UnityEngine;

public class CompanionFollow : MonoBehaviour
{
    public Transform player;
    public float speed = 3.0f;
    public float stoppingDistance = 2.0f;

    private Animator animator;
    private Vector2 lastMovementDirection = Vector2.right; 

    void Start() {
        animator = GetComponent<Animator>();
    }

    void Update() {
        if (player != null) {
            //calculate the distance between the companion and the player
            float distance = Vector2.Distance(transform.position, player.position);

            //Determines the direction of the player
            Vector2 direction = (player.position - transform.position).normalized;

            if (distance > stoppingDistance) {
                //Move the companion towards the player
                transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);

                //Update movement direction
                if (direction != Vector2.zero) {
                    lastMovementDirection = direction;
                }

                //Update aniamtor parameters
                if (animator != null) {
                    animator.SetFloat("Horizontal", lastMovementDirection.x);
                    animator.SetFloat("Vertical", lastMovementDirection.y);
                    animator.SetBool("IsMoving", true);
                }
            } 
            else {
                //When the companion is close, stop moving
                if (animator != null) {
                    animator.SetBool("IsMoving", false);
                    //Force idle
                    animator.SetFloat("Horizontal", 0.0f);
                    animator.SetFloat("Vertical", 0.0f);
                }
            }
        }
    }
}
