using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Action
{
    public string name;
    public float duration;
    public float interval;
    protected float timer;
    protected Agent source;

    public float GetTimer() => timer;

    public Action(string name, Agent source, float duration = 0f, float interval = 0f)
    {
        this.name = name;
        this.source = source;
        this.duration = duration;
        this.interval = interval;
        this.timer = 0f;
    }

    public virtual void Execute(Agent target)
    {
        timer = duration;
    }

    public virtual void Update(Agent target, float deltaTime)
    {
        if (timer >= 0.0f)
        {
            timer -= deltaTime;
        }
    }

    public virtual bool IsComplete() => timer <= 0.0f;
}

/**********************************************************************************************************************/

public class ActionDamage : Action
{
    public ActionDamage(Agent source) 
        : base("Damage", source)
    {
    }

    public override void Execute(Agent target)
    {
        float damage = source.GetDamage;
        if (source.CurrHealth / source.MaxHealth <= Constants.CRIT_HEALTH_THRESHOLD)
        {
            damage *= Constants.CRIT_HEALTH_DAMAGE_MULTIPLIER;
        }
        target.TakeDamage(damage);
        Debug.Log($"{source.name} deals {damage} damage to {target.name}");
    }
}

/**********************************************************************************************************************/

//Debuff class. If no stat is provided, will randomly select one debuff to apply.
public class ActionDebuff : Action
{
    private string stat;

    public ActionDebuff(Agent source, string stat = "none") 
        : base($"{stat}", source, Constants.DEBUFF_DURATION)
    {
        this.stat = stat;
    }

    public override void Execute(Agent target)
    {
        base.Execute(target);
        if (stat == "none")
        {
            int randomIndex = Random.Range(0, 3);
            switch (randomIndex)
            {
                case 0: stat = "attack"; break;
                case 1: stat = "defense"; break;
                case 2: stat = "speed"; break;
            }
        }
        switch (stat)
        {
            case "attack": target.attack *= Constants.DEBUFF_MULTIPLIER; break;
            case "defense": target.defense *= Constants.DEBUFF_MULTIPLIER; break;
            case "speed": target.speed *= Constants.DEBUFF_MULTIPLIER; break;
        }
        Debug.Log($"{source.name} debuffs {target.name}'s {stat} by {Constants.DEBUFF_MULTIPLIER}x");
    }

    public override void Update(Agent target, float deltaTime)
    {
        base.Update(target, deltaTime);
        if (IsComplete())
        {
            switch (stat)
            {
                case "attack": target.attack /= Constants.DEBUFF_MULTIPLIER; break;
                case "defense": target.defense /= Constants.DEBUFF_MULTIPLIER; break;
                case "speed": target.speed /= Constants.DEBUFF_MULTIPLIER; break;
            }
            Debug.Log($"{target.name}'s {stat} debuff expires");
        }
    }
}

/**********************************************************************************************************************/

public class ActionDoT : Action
{
    private float damagePerTick;
    private float nextTick;

    public ActionDoT(Agent source) 
        : base("DoT", source, Constants.DEBUFF_DURATION, 1f)
    {
        float baseDamage = source.GetDamage * Constants.DOT_INCREASE_MULTIPLIER;
        this.damagePerTick = baseDamage / (duration / interval);
        this.nextTick = duration;
    }

    public override void Execute(Agent target)
    {
        base.Execute(target);
        nextTick -= interval;
        Debug.Log($"{source.name} applies DoT to {target.name}");
    }

    public override void Update(Agent target, float deltaTime)
    {
        base.Update(target, deltaTime);
        if (timer - nextTick < 0.0f)
        {
            target.TakeDamage(damagePerTick);
            nextTick -= interval;
            Debug.Log($"{target.name} takes {damagePerTick} DoT damage");
        }
    }
}

/**********************************************************************************************************************/

public class ActionHeal : Action
{
    public ActionHeal(Agent source) 
        : base("Heal", source)
    {
    }

    public override void Execute(Agent target)
    {
        target.Heal(Constants.BASE_HEAL);
        Debug.Log($"{source.name} heals {target.name} for {Constants.BASE_HEAL}");
    }
}

/**********************************************************************************************************************/

public class ActionHoT : Action
{
    private float healPerTick;
    private float nextTick;

    public ActionHoT(Agent source) 
        : base("HoT", source, Constants.BUFF_DURATION, 1f)
    {
        this.healPerTick = (Constants.BASE_HEAL * Constants.HOT_INCREASE_MULTIPLIER) / (duration / interval);
        this.nextTick = duration;
    }

    public override void Execute(Agent target)
    {
        base.Execute(target);
        nextTick -= interval;
        Debug.Log($"{source.name} applies HoT to {target.name}");
    }

    public override void Update(Agent target, float deltaTime)
    {
        base.Update(target, deltaTime);
        if (timer - nextTick < 0.0f)
        {
            target.Heal(healPerTick);
            nextTick -= interval;
            Debug.Log($"{target.name} heals {healPerTick} from HoT");
        }
    }
}

/**********************************************************************************************************************/

public class ActionBuff : Action
{
    private string stat;

    public ActionBuff(Agent source, string stat) 
        : base($"{stat}", source, Constants.BUFF_DURATION)
    {
        this.stat = stat;
    }

    public override void Execute(Agent target)
    {
        base.Execute(target);
        
        switch (stat)
        {
            case "attack": target.attack *= Constants.BUFF_MULTIPLIER; break;
            case "defense": target.defense *= Constants.BUFF_MULTIPLIER; break;
            case "speed": target.speed *= Constants.BUFF_MULTIPLIER; break;
        }
        Debug.Log($"{source.name} buffs {target.name}'s {stat} by {Constants.BUFF_MULTIPLIER}x");
    }

    public override void Update(Agent target, float deltaTime)
    {
        base.Update(target, deltaTime);
        if (IsComplete())
        {
            switch (stat)
            {
                case "attack": target.attack /= Constants.BUFF_MULTIPLIER; break;
                case "defense": target.defense /= Constants.BUFF_MULTIPLIER; break;
                case "speed": target.speed /= Constants.BUFF_MULTIPLIER; break;
            }
            Debug.Log($"{target.name}'s {stat} buff expires");
        }
    }
}

/**********************************************************************************************************************/