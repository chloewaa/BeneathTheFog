using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectCoinsQuestStep : QuestStep
{
    private int coinsCollected = 0; 
    private int coinsToComplete = 5;

    private void Start() {
        UpdateState();
    }

    private void OnEnable() {
        GameEventsManager.instance.miscEvents.onCoinCollected += CoinCollected;
    }

    private void OnDisable() {
        GameEventsManager.instance.miscEvents.onCoinCollected -= CoinCollected;
    }

    private void CoinCollected() {
        if(coinsCollected < coinsToComplete) {
            coinsCollected++;
            UpdateState();
            Debug.Log("Coins Collected: " + coinsCollected);
        }

        if(coinsCollected >= coinsToComplete) {
            FinishQuestStep();
        }
    }

    private void UpdateState() {
        string state = coinsCollected.ToString();
        string status = coinsCollected + "/" + coinsToComplete + " coins.";
        ChangeState(state, status);
    }

    protected override void SetQuestStepState(string state) {
        this.coinsCollected = System.Int32.Parse(state);
        UpdateState();
    }

}
