using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionDoT : Action
{
    private float damagePerTick;

    public ActionDoT(float damagePerTick, float duration, float interval) 
        : base("DoT", duration, interval)
    {
        this.damagePerTick = damagePerTick;
    }

    public override void Execute(Agent source, Agent target)
    {
        target.TakeDamage(damagePerTick);
        Debug.Log($"{source.name} applies DoT to {target.name}");
    }

    public override void Update(Agent target, float deltaTime)
    {
        timer += deltaTime;
        if (timer % interval < deltaTime)
        {
            target.TakeDamage(damagePerTick);
            Debug.Log($"{target.name} takes {damagePerTick} DoT damage");
        }
    }
}
