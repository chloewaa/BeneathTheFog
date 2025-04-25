using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 
using UnityEngine.SceneManagement;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleSystem : MonoBehaviour
{
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
    Animator animator;
    SpriteRenderer spriteRenderer; 
    public Slider magicSlider;

    void Start()
    {
        state = BattleState.START;
        StartCoroutine(SetupBattle());
        //Get animator attached to this GameObject
        animator = GetComponent<Animator>();
    }

    IEnumerator SetupBattle() {
        //Get player unit information
        GameObject playerGO = Instantiate(playerPrefab, playerBattleStation);
        playerUnit = playerGO.GetComponent<Unit>();

        //Get enemy unit information
        GameObject enemyGO = Instantiate(enemyPrefab, enemyBattleStation); 
        enemyUnit = enemyGO.GetComponent<Unit>();

        //Display enemy name
        dialogueText.text = enemyUnit.unitName + " approaches!";

        //Buttons are disabled until player turn
        defendButton.interactable = false;
        healButton.interactable = false;
        magicButton.interactable = false;
        pushBackButton.gameObject.SetActive(false);
        magicAttackButton.gameObject.SetActive(false);

        playerHUD.SetHUD(playerUnit);
        enemyHUD.SetHUD(enemyUnit); 

        //initialize MP slider
        magicSlider.maxValue = playerUnit.maxMP;
        magicSlider.value = playerUnit.currentMP;

        yield return new WaitForSeconds(2f);

        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    IEnumerator PlayerAttack() {
        int baseDmg = playerUnit.damage;
        int finalDmg = baseDmg;
        if (fogBuffTurnsRemaining > 0) {
            finalDmg = Mathf.RoundToInt(baseDmg * fogDamageMultiplier);
            fogBuffTurnsRemaining--;
            if (fogBuffTurnsRemaining == 0) fogDamageMultiplier = 1f;
        }

        bool isDead = enemyUnit.TakeDamage(finalDmg);
        enemyHUD.SetHP(enemyUnit.currentHP);
        dialogueText.text = $"You strike for {finalDmg} damage!";
        yield return new WaitForSeconds(1.5f);

        if (isDead) {
            state = BattleState.WON;
            EndBattle();
        } else {
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }

        //Play the attack animation
        playerUnit.animator.SetTrigger("Attack");
    }

    IEnumerator EnemyTurn() {
        dialogueText.text = $"{enemyUnit.unitName} is attacking…";

        yield return new WaitForSeconds(1f);

        //Calculate damage, halving if defending
        int incoming = enemyUnit.damage;
        if(isPlayerDefending) {
            incoming = Mathf.Max(1, incoming / 2);  
            dialogueText.text = $"You defended! Damage reduced to {incoming}.";
            isPlayerDefending = false;
        }

        bool isDead = playerUnit.TakeDamage(incoming);
        playerHUD.SetHP(playerUnit.currentHP);

        yield return new WaitForSeconds(1.5f);

        if(isDead) {
            state = BattleState.LOST;
            EndBattle();
        } else {
            state = BattleState.PLAYERTURN;
            PlayerTurn();
        }

        //Play enemy attack animation
        enemyUnit.animator.SetTrigger("Melee");
    }

    void EndBattle() {
        //Disable buttons
        attackButton.interactable = false;

        if(state == BattleState.WON) {
            dialogueText.text = "You won the battle!";
        } else if(state == BattleState.LOST) {
            dialogueText.text = "You were defeated!";
        }

        // Start coroutine to exit the battle scene after a short delay
        StartCoroutine(ExitBattleAfterDelay(2f));
    }

    IEnumerator ExitBattleAfterDelay(float delay) {
        yield return new WaitForSeconds(delay);

        // Load the exploration scene (this will replace the current scene)
        SceneManager.UnloadSceneAsync(battleSceneName);
    }


    void PlayerTurn() {
        dialogueText.text = "Player turn.";
        attackButton.interactable = true;
        defendButton.interactable = true;
        healButton.interactable = true;

        //but hide the sub‑options until Magic is clicked
        pushBackButton.gameObject.SetActive(false);
        magicAttackButton.gameObject.SetActive(false);

        magicButton.interactable = playerUnit.currentMP >= Mathf.Min(pushBackCost, windStrikeCost);
    }

    IEnumerator PlayerHeal() {
        playerUnit.Heal(10);

        playerHUD.SetHP(playerUnit.currentHP); 
        dialogueText.text = "You've been healed!";

        yield return new WaitForSeconds(2f); 

        state = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());
    }

    IEnumerator PlayerDefend() {
        //Set flag so next hit is reduced
        isPlayerDefending = true;
        dialogueText.text = "You choose defend!";

        yield return new WaitForSeconds(1.5f);

        state = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());

        //Defend Animation 
        playerUnit.animator.SetTrigger("Defend");
    }

    public void OnAttackButton() {
        if(state != BattleState.PLAYERTURN) 
            return; 

        StartCoroutine(PlayerAttack());

        //disable all buttons
        attackButton.interactable = healButton.interactable = defendButton.interactable = false;
    }

    public void OnHealButton() {
        if(state != BattleState.PLAYERTURN) 
            return; 

        StartCoroutine(PlayerHeal());

        //disable all buttons
        attackButton.interactable = healButton.interactable = defendButton.interactable = false;
    }

    public void OnDefendButton() {
        if(state != BattleState.PLAYERTURN)
            return;

        StartCoroutine(PlayerDefend());
        
        //disable all buttons
        attackButton.interactable = healButton.interactable = defendButton.interactable = false;
    }

    public void OnMagicButton() {
        if (state != BattleState.PLAYERTURN) return;
        //disable the big buttons
        attackButton.interactable = healButton.interactable
                                = defendButton.interactable
                                = magicButton.interactable 
                                = false;
        //show the two choices
        pushBackButton.gameObject.SetActive(true);
        magicAttackButton.gameObject.SetActive(true);

        pushBackButton.interactable    = playerUnit.currentMP >= pushBackCost;
        magicAttackButton.interactable = playerUnit.currentMP >= windStrikeCost;
    }

    public void OnPushBackButton() {
        if (state != BattleState.PLAYERTURN) return;
        pushBackButton.interactable = magicAttackButton.interactable = false;
        StartCoroutine(PlayerPushBack());
    }

    public void OnMagicAttackButton() {
        if (state != BattleState.PLAYERTURN) return;
        pushBackButton.interactable = magicAttackButton.interactable = false;
        StartCoroutine(PlayerMagicAttack());
    }

    IEnumerator PlayerPushBack() {
        if (playerUnit.currentMP < pushBackCost) {
            dialogueText.text = "Not enough MP!";
            yield return new WaitForSeconds(1f);
            state = BattleState.PLAYERTURN;
            PlayerTurn();
            yield break;
        }

        //drain MP
        playerUnit.currentMP -= pushBackCost;
        magicSlider.value = playerUnit.currentMP;

        //apply fog buff
        fogBuffTurnsRemaining = 3;
        dialogueText.text = "Pushed into the fog! Enemy takes +50% damage for 3 turns.";
        yield return new WaitForSeconds(1.5f);

        state = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());

        //Play magic animation
        playerUnit.animator.SetTrigger("MagicAttack");
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

        // calculate damage (with fog buff)...
        int baseDmg = playerUnit.magicDamage;
        int finalDmg = (fogBuffTurnsRemaining>0)
            ? Mathf.RoundToInt(baseDmg * fogDamageMultiplier)
            : baseDmg;
        if (fogBuffTurnsRemaining>0) {
            fogBuffTurnsRemaining--;
            if (fogBuffTurnsRemaining==0) fogDamageMultiplier = 1f;
        }

        bool isDead = enemyUnit.TakeDamage(finalDmg);
        enemyHUD.SetHP(enemyUnit.currentHP);
        dialogueText.text = $"You cast Wind Strike for {finalDmg} damage!";
        yield return new WaitForSeconds(1.5f);

        if (isDead) {
            state = BattleState.WON;
            EndBattle();
        } else {
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }

        //Play magic animation
        playerUnit.animator.SetTrigger("MagicAttack");
    }
}
