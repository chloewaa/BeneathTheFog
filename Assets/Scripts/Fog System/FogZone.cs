using UnityEngine;

public class FogZone : MonoBehaviour {
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            other.GetComponent<FogMeterSystem>().SetFogDraining(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            other.GetComponent<FogMeterSystem>().SetFogDraining(false);
        }
    }
}
