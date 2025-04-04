using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Action
{
    public string name;
    public float duration; // For timed effects (0 for instant)
    public float interval; // For periodic effects (0 for single execution)
    protected float timer;

    public float GetTimer()
    {
        return timer;
    }

    public Action(string name, float duration = 0f, float interval = 0f)
    {
        this.name = name;
        this.duration = duration;
        this.interval = interval;
        this.timer = 0f;
    }

    public virtual void Execute(Agent source, Agent target)
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
    public virtual bool IsComplete()
    {
        return timer <= 0.0f;
    }
}
