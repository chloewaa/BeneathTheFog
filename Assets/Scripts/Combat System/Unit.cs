
using System.Collections; 
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [Header("Basic Stats")]
    public string unitName;
    public int unitLevel;

    [Header("Health")]
    public int maxHP;
    [HideInInspector] public int currentHP;

    [Header("Physical Attack")]
    public int damage;

    [Header("Magic")]
    public int maxMP;           
    [HideInInspector] public int currentMP;   
    public int magicDamage; 

    [Header("Animation")]
    public Animator animator;

    void Awake() {
        //initialize HP/MP on spawn
        currentHP = maxHP;
        currentMP = maxMP;

        animator = GetComponent<Animator>(); 
    }

    public bool TakeDamage(int dmg) {
        //Subtract HP and return true if unit dies.
        currentHP -= dmg;
        return currentHP <= 0;

        //Play hit animation
        animator.SetTrigger("Hit");
    }

    public void Heal(int amount) {
        //Heal up to maxHP.
        currentHP = Mathf.Min(currentHP + amount, maxHP);

        //Flash green to indicate healing
        StartCoroutine(FlashColor(Color.green, 0.2f));
    }

    public bool UseMP(int cost) {
        //Try to spend MP. Returns true if you had enough.
        if (currentMP >= cost)
        {
            currentMP -= cost;
            return true;
        }
        return false;
    }

    public IEnumerator FlashColor(Color flashColor, float duration) {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>(); 
        if(spriteRenderer == null) yield break;

        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = flashColor;

        yield return new WaitForSeconds(duration);

        spriteRenderer.color = originalColor; 
    }   
}
