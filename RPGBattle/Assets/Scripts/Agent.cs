using UnityEngine;
using System.Collections.Generic;

public enum Team { Player, Enemy }

public class Agent : MonoBehaviour
{
    public Team team; // Set in BattleManager or Inspector
    protected float maxHealth;
    protected float currHealth;
    public float attack;
    public float defense;
    public float speed;
    protected SpriteRenderer spriteRenderer;

    public float MaxHealth => maxHealth;
    public float CurrHealth => currHealth;
    public float Attack => attack;
    public float Defense => defense;
    public float Speed => speed;

    public List<Action> availableActions = new List<Action>(); // Configured in BattleManager

    public virtual void Initialize(float maxHealth, float attack, float defense, float speed, Sprite sprite)
    {
        this.maxHealth = maxHealth;
        this.currHealth = maxHealth;
        this.attack = attack;
        this.defense = defense;
        this.speed = speed;

        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }
        spriteRenderer.sprite = sprite;
    }

    public float GetActionInterval()
    {
        return 10f - 0.08f * speed; // Speed 0 = 10s, Speed 100 = 2s
    }

    public virtual void TakeDamage(float damage)
    {
        float actualDamage = damage - (defense * 0.1f);
        actualDamage = Mathf.Max(0, actualDamage);
        currHealth -= actualDamage;

        if (currHealth <= 0)
        {
            Die();
        }
    }

    public virtual void Heal(float amount)
    {
        currHealth += amount;
        currHealth = Mathf.Min(currHealth, maxHealth);
    }

    public virtual void Die()
    {
        Destroy(gameObject);
    }
}