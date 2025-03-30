using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionHeal : Action
{
    private float healAmount;

    public ActionHeal(float healAmount) : base("Heal")
    {
        this.healAmount = healAmount;
    }

    public override void Execute(Agent source, Agent target)
    {
        target.Heal(healAmount);
        Debug.Log($"{source.name} heals {target.name} for {healAmount}");
    }
}
