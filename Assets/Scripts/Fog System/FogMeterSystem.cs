using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 

public class FogMeterSystem : MonoBehaviour {
    [Header("Fog Meter Settings")]
    public float maxFogControl = 100f;
    public float currentFogControl;
    public float drainRate = 5f;
    public float regenRate = 2f; 

    [Header("UI")]
    public Slider fogSlider;

    [Header("Respawn Settings")]
    public string respawnSceneName = "SafeZone";
    public Transform respawnPoint;
    public Transform companionTransform;

    [Header("Movement Impact")]
    public float maxPlayerSpeed = 5.0f;
    public float minPlayerSpeed = 1.0f; 

    private bool isDraining = false; 
    private PlayerController playerController; 

    void Start() {
        currentFogControl = maxFogControl;
        fogSlider.maxValue = maxFogControl;
        fogSlider.value = currentFogControl;
        playerController = GetComponent<PlayerController>(); 
    }

    void Update() {
        if (isDraining) {
            currentFogControl -= drainRate * Time.deltaTime;
        } else {
            currentFogControl = Mathf.Min(currentFogControl + regenRate * Time.deltaTime, maxFogControl);
        }

        fogSlider.value = currentFogControl;

        if (currentFogControl <= 0) {
            PlayerPassOut();
        }
        if (playerController != null) {
            float speedPercent = currentFogControl / maxFogControl;
            speedPercent = Mathf.Clamp01(speedPercent);

            playerController.moveSpeed = Mathf.Lerp(minPlayerSpeed, maxPlayerSpeed, speedPercent);
        }
    }

    void PlayerPassOut() {
        Debug.Log("Player passed out from fog exposure.");

        // Teleport player to respawn point
        if (respawnPoint != null) {
            transform.position = respawnPoint.position;

            // Teleport companion to match
        if (companionTransform != null) {
            companionTransform.position = respawnPoint.position;

            // Reset animator if it exists
            Animator compAnim = companionTransform.GetComponent<Animator>();
            if (compAnim != null) {
                compAnim.SetBool("IsMoving", false);
                compAnim.SetFloat("Horizontal", 0f);
                compAnim.SetFloat("Vertical", -1f); // Default facing direction, adjust if needed
            }
        }

        }
        // Restore fog meter
        currentFogControl = maxFogControl;
        fogSlider.value = currentFogControl;
    }

    public void SetFogDraining(bool draining) {
        isDraining = draining;
    }
}
