using UnityEngine;
using System.Collections.Generic;

public enum Team { Player, Enemy }

public class Agent : MonoBehaviour
{
    private const float LOW_HEALTH_DAMAGE_MULTIPLIER = 2.0f;

    private Team team;
    protected float maxHealth;
    protected float currHealth;
    public float attack;
    public float defense;
    public float speed;
    private AgentUI agentUI;
    protected SpriteRenderer spriteRenderer;

    public float actionTimer;
    public List<Action> activeEffects = new List<Action>();

    public float MaxHealth => maxHealth;
    public float CurrHealth => currHealth;
    public float Attack => attack;
    public float Defense => defense;
    public float Speed => speed;
    public Team Team => team;

    public List<Action> availableActions = new List<Action>(); // Configured in BattleManager

    public virtual void Initialize(Team team, float maxHealth, float attack, float defense, float speed)
    {
        this.team = team;
        this.maxHealth = maxHealth;
        this.currHealth = maxHealth;
        this.attack = attack;
        this.defense = defense;
        this.speed = speed;
    }

    public float GetActionInterval()
    {
        return 10f - 0.08f * speed; // Speed 0 = 10s, Speed 100 = 2s
    }

    public virtual void TakeDamage(float damage)
    {
        float actualDamage = damage * (1 - (0.0025f * defense));
        actualDamage = Mathf.Max(0, actualDamage);

        if (currHealth - actualDamage < maxHealth * BattleManager.CRIT_HEALTH_THRESHOLD){
            attack *= LOW_HEALTH_DAMAGE_MULTIPLIER;
        }

        currHealth -= actualDamage;

        if (currHealth <= 0)
        {
            Die();
        }
    }

    public virtual void Heal(float amount)
    {
        if (currHealth < BattleManager.CRIT_HEALTH_THRESHOLD && currHealth + amount > BattleManager.CRIT_HEALTH_THRESHOLD){
            attack /= LOW_HEALTH_DAMAGE_MULTIPLIER;
        }
        currHealth += amount;
        currHealth = Mathf.Min(currHealth, maxHealth);
    }

    public virtual void Die()
    {
        if (agentUI != null)
        {
            agentUI.Die();
        }
        Destroy(gameObject);
    }

    public virtual void SetupVisuals(Sprite sprite, GameObject healthBarPrefab, GameObject textPrefab)
    {
        spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;

        GameObject uiObject = new GameObject($"{name}_UI");
        agentUI = uiObject.AddComponent<AgentUI>();
        agentUI.Initialize(this, maxHealth, healthBarPrefab, textPrefab);

        // Position UI above Agent
        uiObject.transform.position = transform.position + new Vector3(0, 1, 0); // Adjust offset as needed
    }
}