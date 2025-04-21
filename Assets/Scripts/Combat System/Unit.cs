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
    public int maxMP;           // ← total MP
    [HideInInspector] public int currentMP;   // ← current MP
    public int magicDamage;     // ← base magic‑attack power

    void Awake()
    {
        // initialize HP/MP on spawn
        currentHP = maxHP;
        currentMP = maxMP;
    }

    /// <summary>
    /// Subtract HP and return true if unit dies.
    /// </summary>
    public bool TakeDamage(int dmg)
    {
        currentHP -= dmg;
        return currentHP <= 0;
    }

    /// <summary>
    /// Heal up to maxHP.
    /// </summary>
    public void Heal(int amount)
    {
        currentHP = Mathf.Min(currentHP + amount, maxHP);
    }

    /// <summary>
    /// Try to spend MP. Returns true if you had enough.
    /// </summary>
    public bool UseMP(int cost)
    {
        if (currentMP >= cost)
        {
            currentMP -= cost;
            return true;
        }
        return false;
    }
}
