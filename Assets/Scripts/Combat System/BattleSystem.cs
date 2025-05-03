using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleSystem : MonoBehaviour {
    [Header("UI Buttons")]
    public Button attackButton;
    public Button defendButton;
    public Button healButton;
    public Button magicButton;
    public Button pushBackButton;
    public Button magicAttackButton;

    [Header("Prefabs")]
    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    [Header("Stations")]
    public Transform playerBattleStation;
    public Transform enemyBattleStation;

    [Header("Scene name")]
    public string explorationSceneName = "Scene 01";
    public string battleSceneName = "BattleScene";

    [Header("Units")]
    Unit playerUnit;
    Unit enemyUnit;

    [Header("Text Mesh Pro")]
    public TextMeshProUGUI dialogueText;

    [Header("HUDs")]
    public BattleHUD playerHUD;
    public BattleHUD enemyHUD;
    public BattleState state;

    [Header("Variables")]
    private bool isPlayerDefending = false;
    public int pushBackCost = 5;
    public int windStrikeCost = 8;
    private int fogBuffTurnsRemaining = 0;
    private float fogDamageMultiplier = 1.5f;

    [Header("References")]
    public Slider magicSlider;

    void Start() {
        state = BattleState.START;
        StartCoroutine(SetupBattle());
    }

    IEnumerator SetupBattle() {
        GameObject playerGO = Instantiate(playerPrefab, playerBattleStation);
        playerUnit = playerGO.GetComponent<Unit>();

        GameObject enemyGO = Instantiate(enemyPrefab, enemyBattleStation);
        enemyUnit = enemyGO.GetComponent<Unit>();

        dialogueText.text = enemyUnit.unitName + " approaches!";

        SetActionButtonsActive(false);
        pushBackButton.gameObject.SetActive(false);
        magicAttackButton.gameObject.SetActive(false);

        playerHUD.SetHUD(playerUnit);
        enemyHUD.SetHUD(enemyUnit);

        magicSlider.maxValue = playerUnit.maxMP;
        magicSlider.value = playerUnit.currentMP;

        yield return new WaitForSeconds(2f);

        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    void SetActionButtonsActive(bool isActive) {
        attackButton.interactable = isActive;
        defendButton.interactable = isActive;
        healButton.interactable = isActive;
        magicButton.interactable = isActive;
    }

    void PlayerTurn() {
        dialogueText.text = "Player turn.";
        SetActionButtonsActive(true);
        pushBackButton.gameObject.SetActive(false);
        magicAttackButton.gameObject.SetActive(false);
        magicButton.interactable = playerUnit.currentMP >= Mathf.Min(pushBackCost, windStrikeCost);
    }

    public void OnAttackButton() {
        if (state != BattleState.PLAYERTURN) return;
        SetActionButtonsActive(false);
        StartCoroutine(PlayerAttack());
    }

    IEnumerator PlayerAttack() {
        playerUnit.animator.SetTrigger("Attack");
        yield return new WaitForSeconds(0.5f); // windup

        int baseDmg = playerUnit.damage;
        int finalDmg = (fogBuffTurnsRemaining > 0)
            ? Mathf.RoundToInt(baseDmg * fogDamageMultiplier)
            : baseDmg;

        if (fogBuffTurnsRemaining > 0) {
            fogBuffTurnsRemaining--;
            if (fogBuffTurnsRemaining == 0)
                fogDamageMultiplier = 1f;
        }

        bool isDead = enemyUnit.TakeDamage(finalDmg);
        enemyUnit.animator.SetTrigger("Hit");

        enemyHUD.SetHP(enemyUnit.currentHP);
        dialogueText.text = $"You strike for {finalDmg} damage!";
        yield return new WaitForSeconds(1f);

        if (isDead) {
            enemyUnit.animator.SetTrigger("Die");
            state = BattleState.WON;
            yield return new WaitForSeconds(1f); 
            EndBattle();
        } else {
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }

    IEnumerator EnemyTurn() {
        dialogueText.text = $"{enemyUnit.unitName} is attacking...";
        enemyUnit.animator.SetTrigger("Melee");
        yield return new WaitForSeconds(0.4f);

        int incoming = enemyUnit.damage;
        if (isPlayerDefending) {
            incoming = Mathf.Max(1, incoming / 2);
            dialogueText.text = $"You defended! Damage reduced to {incoming}.";
            isPlayerDefending = false;
        }

        bool isDead = playerUnit.TakeDamage(incoming);
        playerHUD.SetHP(playerUnit.currentHP);

        playerUnit.animator.SetTrigger("Hit");

        yield return new WaitForSeconds(1f);

        if (isDead) {
            playerUnit.animator.SetTrigger("Die");
            state = BattleState.LOST;
            yield return new WaitForSeconds(1f);
            EndBattle();
        } else {
            state = BattleState.PLAYERTURN;
            PlayerTurn();
        }
    }

    void EndBattle() {
        SetActionButtonsActive(false);
        dialogueText.text = state == BattleState.WON ? "You won the battle!" : "You were defeated!";
        StartCoroutine(ExitBattleAfterDelay(2f));
    }

    IEnumerator ExitBattleAfterDelay(float delay) {
        yield return new WaitForSeconds(delay);
        SceneManager.UnloadSceneAsync(battleSceneName);
    }

    public void OnHealButton() {
        if (state != BattleState.PLAYERTURN) return;
        SetActionButtonsActive(false);
        StartCoroutine(PlayerHeal());
    }

    IEnumerator PlayerHeal() {
        playerUnit.Heal(10);
        playerHUD.SetHP(playerUnit.currentHP);
        dialogueText.text = "You've been healed!";
        yield return new WaitForSeconds(2f);
        state = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());
    }

    public void OnDefendButton() {
        if (state != BattleState.PLAYERTURN) return;
        SetActionButtonsActive(false);
        StartCoroutine(PlayerDefend());
    }

    IEnumerator PlayerDefend() {
        isPlayerDefending = true;
        playerUnit.animator.SetTrigger("Defend");
        dialogueText.text = "You choose defend!";
        yield return new WaitForSeconds(1.5f);
        state = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());
    }

    public void OnMagicButton() {
        if (state != BattleState.PLAYERTURN) return;
        SetActionButtonsActive(false);
        pushBackButton.gameObject.SetActive(true);
        magicAttackButton.gameObject.SetActive(true);
        pushBackButton.interactable = playerUnit.currentMP >= pushBackCost;
        magicAttackButton.interactable = playerUnit.currentMP >= windStrikeCost;
    }

    public void OnPushBackButton() {
        if (state != BattleState.PLAYERTURN) return;
        pushBackButton.interactable = magicAttackButton.interactable = false;
        StartCoroutine(PlayerPushBack());
    }

    IEnumerator PlayerPushBack() {
        if (playerUnit.currentMP < pushBackCost) {
            dialogueText.text = "Not enough MP!";
            yield return new WaitForSeconds(1f);
            state = BattleState.PLAYERTURN;
            PlayerTurn();
            yield break;
        }

        playerUnit.currentMP -= pushBackCost;
        magicSlider.value = playerUnit.currentMP;

        playerUnit.animator.SetTrigger("MagicAttack");
        yield return new WaitForSeconds(0.5f);

        fogBuffTurnsRemaining = 3;
        dialogueText.text = "Pushed into the fog! Enemy takes +50% damage for 3 turns.";
        yield return new WaitForSeconds(1.5f);

        state = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());
    }

    public void OnMagicAttackButton() {
        if (state != BattleState.PLAYERTURN) return;
        pushBackButton.interactable = magicAttackButton.interactable = false;
        StartCoroutine(PlayerMagicAttack());
    }

    IEnumerator PlayerMagicAttack() {
        if (playerUnit.currentMP < windStrikeCost) {
            dialogueText.text = "Not enough MP!";
            yield return new WaitForSeconds(1f);
            state = BattleState.PLAYERTURN;
            PlayerTurn();
            yield break;
        }

        playerUnit.currentMP -= windStrikeCost;
        magicSlider.value = playerUnit.currentMP;

        playerUnit.animator.SetTrigger("MagicAttack");
        yield return new WaitForSeconds(0.5f);

        int baseDmg = playerUnit.magicDamage;
        int finalDmg = (fogBuffTurnsRemaining > 0)
            ? Mathf.RoundToInt(baseDmg * fogDamageMultiplier)
            : baseDmg;

        if (fogBuffTurnsRemaining > 0) {
            fogBuffTurnsRemaining--;
            if (fogBuffTurnsRemaining == 0)
                fogDamageMultiplier = 1f;
        }

        bool isDead = enemyUnit.TakeDamage(finalDmg);
        enemyUnit.animator.SetTrigger("Hit");

        enemyHUD.SetHP(enemyUnit.currentHP);
        dialogueText.text = $"You cast Wind Strike for {finalDmg} damage!";
        yield return new WaitForSeconds(1f);

        if (isDead) {
            state = BattleState.WON;
            EndBattle();
        } else {
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }
}
