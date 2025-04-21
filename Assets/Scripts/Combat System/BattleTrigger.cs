using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleTrigger : MonoBehaviour {
    public string battleSceneName   = "BattleScene";
    public string splashScreenScene = "BattleSplash";
    public GameObject orc;          // Assign your Orc GameObject in the Inspector

    private bool triggered;
    private Collider2D col;

    private void Awake() {
        // 1) Grab & re‑enable the collider, reset trigger flag & orc
        triggered = false;
        col       = GetComponent<Collider2D>();
        if (col    != null) col.enabled   = true;
        if (orc    != null) orc.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (triggered || !other.CompareTag("Player")) return;
        triggered = true;

        // 2) Disable just the collider so we can't retrigger,
        //    and hide the orc for good when we return.
        if (col    != null) col.enabled   = false;
        if (orc    != null) orc.SetActive(false);

        StartCoroutine(TransitionToBattle());
    }

    private IEnumerator TransitionToBattle() {
        // 3) Splash screen (additive so exploration stays alive)
        SceneManager.LoadScene(splashScreenScene, LoadSceneMode.Additive);
        yield return new WaitForSeconds(1.5f);
        SceneManager.UnloadSceneAsync(splashScreenScene);

        // 4) Battle scene (additive on top)
        SceneManager.LoadScene(battleSceneName, LoadSceneMode.Additive);
    }

    // 5) Call this from your BattleSystem when the fight ends:
    public void ReturnToExploration() {
        // Unload the battle scene, revealing exploration exactly as you left it
        SceneManager.UnloadSceneAsync(battleSceneName);
    }
}
