using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectLeavesQuestStep : QuestStep
{
    private int leavesCollected = 0; 
    private int leavesToComplete = 5;

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
        if(leavesCollected < leavesToComplete) {
            leavesCollected++;
            UpdateState();
            Debug.Log("Coins Collected: " + leavesCollected);
        }

        if(leavesCollected >= leavesToComplete) {
            FinishQuestStep();
        }
    }

    private void UpdateState() {
        string state = leavesCollected.ToString();
        string status = leavesCollected + "/" + leavesToComplete + " leaves.";
        ChangeState(state, status);
    }

    protected override void SetQuestStepState(string state) {
        this.leavesCollected = System.Int32.Parse(state);
        UpdateState();
    }

}
