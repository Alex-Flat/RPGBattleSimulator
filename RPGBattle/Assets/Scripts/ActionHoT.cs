using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionHoT : Action
{
   private float healPerTick;

    public ActionHoT(float healPerTick, float duration, float interval) 
        : base("HoT", duration, interval)
    {
        this.healPerTick = healPerTick;
    }

    public override void Execute(Agent source, Agent target)
    {
        target.Heal(healPerTick);
        Debug.Log($"{source.name} applies HoT to {target.name}");
    }

    public override void Update(Agent target, float deltaTime)
    {
        timer += deltaTime;
        if (timer % interval < deltaTime)
        {
            target.Heal(healPerTick);
            Debug.Log($"{target.name} heals {healPerTick} from HoT");
        }
    }
}
