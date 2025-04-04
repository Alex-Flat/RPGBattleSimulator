using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionDoT : Action
{
    private float damagePerTick;
    private float nextTick;

    public ActionDoT(float damage, float duration, float interval) 
        : base("DoT", duration, interval)
    {
        this.damagePerTick = (damage / (duration / interval)) * 1.5f;
        this.nextTick = duration;
    }

    public override void Execute(Agent source, Agent target)
    {
        base.Execute(source, target);
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
