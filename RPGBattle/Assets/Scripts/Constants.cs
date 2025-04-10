using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants : MonoBehaviour
{
    //Gameplay constants.
    public const float START_COOLDOWN = 1.0f; // Time before first action.

    // Agent stats.
    public const float BASE_HEALTH = 100.0f; // Base health for all agents.
    public const float AVG_ATTACK = 50.0f; // Average attack stat: Higher makes agent do more damage. Calibrated to work in 1-100 range, but still works in greater bounds.
    public const float AVG_DEFENSE = 50.0f; // Average defense stat: Higher makes agent take less damage. Calibrated to work in 1-100 range, but still works in greater bounds.
    public const float AVG_SPEED = 50.0f; // Average speed stat, Higher makes agent act faster. Calibrated to work in 1-100 range, but still works in greater bounds.
    public const float CRIT_HEALTH_THRESHOLD = 0.4f; // Percentage of health at which agent's attacks become critical.
    public const float CRIT_HEALTH_DAMAGE_MULTIPLIER = 2.0f; // Multiplier for crit damage.
    public const float DEFENSE_DAMAGE_MULTIPLIER = 0.0025f; // Multiplier for defense damage reduction. Increase this to make defense more effective.
    public const float STAT_VARIANCE = 0.2f; // Variance for stat distribution. 0 = no variance, 1 = full variance. 

    // Action stats.
    public const float BASE_DAMAGE = 25.0f; // Base damage for all agents. Increases with attack stat in damage calculation.
    public const float DAMAGE_INC = 0.01f; // Increases damage by this amount for each point of attack stat.
    public const float BASE_HEAL = 25.0f; // Base heal for all agents. Static heal amount.
    public const float DOT_DURATION = 7.0f; // Duration of dot.
    public const float DOT_INTERVAL = 1.0f; // Interval of dot.
    public const float HOT_DURATION = 7.0f; // Duration of hot.
    public const float HOT_INTERVAL = 1.0f; // Interval of hot.
    public const float BUFF_DURATION = 5.0f; // Duration of buffs.
    public const float DEBUFF_DURATION = 5.0f; // Duration of debuffs.

    //Action multipliers.
    public const float DOT_INCREASE_MULTIPLIER = 1.5f; // Multiplier for dot damage increase. Mutliply damage by this amount for total damage over time.
    public const float HOT_INCREASE_MULTIPLIER = 1.5f; // Multiplier for hot increase. Mutliply heal by this amount for total heal over time.
    public const float BUFF_MULTIPLIER = 1.2f; // Multiplier for buffs.
    public const float DEBUFF_MULTIPLIER = 0.8f; // Multiplier for debuffs.
}
