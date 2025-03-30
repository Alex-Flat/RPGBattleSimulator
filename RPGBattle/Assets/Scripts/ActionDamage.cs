using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionDamage : Action
{
    private float damage;

    public ActionDamage(float damage) : base("Damage")
    {
        this.damage = damage;
    }

    public override void Execute(Agent source, Agent target)
    {
        target.TakeDamage(damage);
        Debug.Log($"{source.name} deals {damage} damage to {target.name}");
    }
}
