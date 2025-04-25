using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleTrigger : MonoBehaviour {
    [Header("Scene names")]
    //Battle scene and splash scene
    public string battleSceneName   = "BattleScene";
    public string splashScreenScene = "BattleSplash";

    [Header("References")]
    public GameObject orc;         
    private bool triggered;
    private Collider2D col;
    public Canvas canvasToHide;

    private void Awake() {
        //Grab & re‑enable the collider, reset trigger flag & orc
        triggered = false;
        col       = GetComponent<Collider2D>();
        if (col    != null) col.enabled   = true;
        if (orc    != null) orc.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (triggered || !other.CompareTag("Player")) return;
        triggered = true;

        //Disable just the collider so we can't retrigger,
        //and hide the orc for good when we return.
        if (col    != null) col.enabled   = false;
        if (orc    != null) orc.SetActive(false);

        StartCoroutine(TransitionToBattle());
    }

    private IEnumerator TransitionToBattle() {
        if(canvasToHide != null) {
            //Disable the canvas to hide the exploration UI
            canvasToHide.enabled = false;
        }
        
        //Splash screen 
        SceneManager.LoadScene(splashScreenScene, LoadSceneMode.Additive);
        yield return new WaitForSeconds(1.5f);
        SceneManager.UnloadSceneAsync(splashScreenScene);

        //Battle scene 
        SceneManager.LoadScene(battleSceneName, LoadSceneMode.Additive);
    }

    public void ReturnToExploration() {
        //Unload the battle scene, revealing exploration 
        SceneManager.UnloadSceneAsync(battleSceneName);

        if(canvasToHide != null) {
            //Re-enable the canvas to show the exploration UI
            canvasToHide.enabled = true;
        }
    }
}
