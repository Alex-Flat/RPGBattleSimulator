using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public Image fill;
    private float maxHealth;
    private float critHealth;

    public void Initialize(float maxHealth)
    {
        this.maxHealth = maxHealth;
        this.critHealth = BattleManager.CRIT_HEALTH_THRESHOLD;
    }
    public void SetHealth(float health)
    {
        slider.value = health / maxHealth;
        if (health <= critHealth * maxHealth)
        {
            fill.color = Color.red;
        }
        else
        {
            fill.color = Color.green;
        }
    }
}
