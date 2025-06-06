using UnityEngine;
using TMPro;
using UnityEngine.UI; 

public class BattleHUD : MonoBehaviour {
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI levelText; 
    public Slider hpSlider; 

    public void SetHUD(Unit unit) {
        nameText.text = unit.unitName; 
        levelText.text = "LVL " + unit.unitLevel;
        hpSlider.maxValue = unit.maxHP;
        hpSlider.value =  unit.currentHP;
    }

    public void SetHP(int hp) {
        hpSlider.value = hp; 
    }
}
