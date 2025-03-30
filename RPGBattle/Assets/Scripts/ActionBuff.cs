using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionBuff : Action
{
    private string stat;
    private float amount;

    public ActionBuff(string stat, float amount, float duration) 
        : base($"Buff {stat}", duration)
    {
        this.stat = stat;
        this.amount = amount;
    }

    public override void Execute(Agent source, Agent target)
    {
        switch (stat)
        {
            case "attack": target.attack += amount; break;
            case "defense": target.defense += amount; break;
            case "speed": target.speed += amount; break;
        }
        Debug.Log($"{source.name} buffs {target.name}'s {stat} by {amount}");
    }

    public override void Update(Agent target, float deltaTime)
    {
        timer += deltaTime;
        if (IsComplete())
        {
            switch (stat)
            {
                case "attack": target.attack -= amount; break;
                case "defense": target.defense -= amount; break;
                case "speed": target.speed -= amount; break;
            }
            Debug.Log($"{target.name}'s {stat} buff expires");
        }
    }
}
