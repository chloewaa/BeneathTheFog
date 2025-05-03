using UnityEngine;

public class CompanionCollider : MonoBehaviour {
    void Start() {
        //Find the player's collider 
        var player = GameObject.FindGameObjectWithTag("Player");

        if(player == null) {
            Debug.LogWarning("No GameObject with the tag Player found");
            return;
        }

        //Player and companion colliders
        Collider2D playerCollider = player.GetComponent<Collider2D>();
        Collider2D companionCollider = GetComponent<Collider2D>();

        if(playerCollider == null || companionCollider == null) {
            Debug.LogWarning("Missing Collider on either player or companion");
            return;
        }

        //ignore collisions between them
        Physics2D.IgnoreCollision(playerCollider, companionCollider);
    }
}
