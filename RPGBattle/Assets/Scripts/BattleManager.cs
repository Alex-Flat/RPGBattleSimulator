using UnityEngine;
using System.Collections.Generic;
using System.Linq;


public class BattleManager : MonoBehaviour
{
    // Configurable in Inspector, 
    public const float CRIT_HEALTH_THRESHOLD = 0.4f;
    public const float AVG_DAMAGE = 20.0f;
    public const float AVG_HEAL = 15.0f;
    public const float AVG_BUFF = 10.0f;
    public const float AVG_DEBUFF = 10.0f;
    public const float AVG_SPEED = 5.0f;

    [SerializeField] private int playerCount = 3; // Configurable in Inspector
    [SerializeField] private int enemyCount = 3;  // Configurable in Inspector
    [SerializeField] private Sprite playerSprite; // Assign circle sprite in Inspector
    [SerializeField] private Sprite enemySprite;  // Assign square sprite in Inspector
    [SerializeField] private GameObject healthBarPrefab;

    public List<Agent> playerTeam = new List<Agent>();
    public List<Agent> enemyTeam = new List<Agent>();
    private Dictionary<Agent, float> actionTimers = new Dictionary<Agent, float>();
    public Dictionary<Agent, List<Action>> activeEffects = new Dictionary<Agent, List<Action>>();


    void Start()
    {
        SetupBattle();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateBattle();
    }

    void SetupBattle()
    {
        // Spawn player agents
        for (int i = 0; i < playerCount; i++)
        {
            GameObject playerObj = new GameObject($"Player_{i}");
            Agent player = playerObj.AddComponent<Agent>();
            player.Initialize(Team.Player, 100f, 20f, 30f, 5f + i * 2); // Vary speed slightly
            player.availableActions.Add(new ActionDamage(20f));
            player.availableActions.Add(new ActionHeal(15f));
            player.availableActions.Add(new ActionBuff("attack", 10f, 5f));
            playerObj.transform.position = new Vector3(-2f * (playerCount - i), 0, i);
            playerTeam.Add(player);
            actionTimers[player] = 0f;
            activeEffects[player] = new List<Action>();

            player.SetupVisuals(playerSprite, healthBarPrefab);
        }

        // Spawn enemy agents
        for (int i = 0; i < enemyCount; i++)
        {
            GameObject enemyObj = new GameObject($"Enemy_{i}");
            Agent enemy = enemyObj.AddComponent<Agent>();
            enemy.Initialize(Team.Enemy, 80f, 15f, 20f, 4f + i * 2); // Vary speed slightly
            enemy.availableActions.Add(new ActionDamage(15f));
            enemy.availableActions.Add(new ActionDoT(5f, 6f, 2f));
            enemy.availableActions.Add(new ActionDebuff("defense", 10f, 5f));
            enemyObj.transform.position = new Vector3(2f * (i + 1), 0, i);
            enemyTeam.Add(enemy);
            actionTimers[enemy] = 0f;
            activeEffects[enemy] = new List<Action>();

            enemy.SetupVisuals(enemySprite, healthBarPrefab);
        }

        Debug.Log("Battle setup complete!");
    }

    void UpdateBattle()
    {
        // Clean up dead agents
        playerTeam.RemoveAll(a => a == null || a.CurrHealth <= 0);
        enemyTeam.RemoveAll(a => a == null || a.CurrHealth <= 0);
        actionTimers = actionTimers.Where(kvp => kvp.Key != null && kvp.Key.CurrHealth > 0)
                                   .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        // Update active effects
        foreach (Agent agent in activeEffects.Keys.ToList())
        {
            if (agent != null)
            {
                var effects = activeEffects[agent];
                for (int i = effects.Count - 1; i >= 0; i--)
                {
                    effects[i].Update(agent, Time.deltaTime);
                    if (effects[i].IsComplete())
                    {
                        effects.RemoveAt(i);
                    }
                }
                agent.activeEffects = effects;
            }
        }

        // Check battle end
        if (playerTeam.Count == 0 || enemyTeam.Count == 0)
        {
            string winner = playerTeam.Count == 0 ? "Enemies" : "Players";
            Debug.Log($"Battle ended! {winner} win!");
            enabled = false;
            return;
        }

        // Process agent actions
        foreach (Agent agent in actionTimers.Keys.ToList())
        {
            actionTimers[agent] -= Time.deltaTime;
            if (actionTimers[agent] <= 0.0f)
            {
                PerformAction(agent);
                actionTimers[agent] = agent.GetActionInterval();
            }
            agent.actionTimer = actionTimers[agent];
        }
    }

    void PerformAction(Agent agent)
    {
        if (agent.availableActions.Count == 0) return;

        // Randomly select an action
        Action action = agent.availableActions[Random.Range(0, agent.availableActions.Count)];
        List<Agent> targets = agent.Team == Team.Player ? enemyTeam : playerTeam;
        List<Agent> allies = agent.Team == Team.Player ? playerTeam : enemyTeam;

        Agent target = null;

        // Filter living targets/allies
        List<Agent> validTargets = targets.Where(t => t != null && t.CurrHealth > 0).ToList();
        List<Agent> validAllies = allies.Where(a => a != null && a.CurrHealth > 0).ToList();
        if (action is ActionDamage || action is ActionDoT || action is ActionDebuff)
        {
            // Randomly select a target from enemies
            if (validTargets.Count > 0)
            {
                target = validTargets[Random.Range(0, validTargets.Count)];
            }
        }
        else if (action is ActionHeal || action is ActionHoT)
        {
            // Check if agent or any teammate is below critical health threshold
            bool needsHealing = validAllies.Any(a => a.CurrHealth / a.MaxHealth < CRIT_HEALTH_THRESHOLD);
            if (needsHealing)
            {
                // Randomly select a teammate below critical health threshold
                List<Agent> lowHealthAllies = validAllies.Where(a => a.CurrHealth / a.MaxHealth < CRIT_HEALTH_THRESHOLD).ToList();
                if (lowHealthAllies.Count > 0)
                {
                    target = lowHealthAllies[Random.Range(0, lowHealthAllies.Count)];
                }
            }
        }
        else if (!agent.availableActions.Any(a => a is ActionDamage || a is ActionDoT || a is ActionDebuff))
        {
            // If no damage actions, heal the teammate with the lowest health percentage
            if (validAllies.Count > 0)
            {
                target = validAllies.OrderBy(a => a.CurrHealth / a.MaxHealth).First();
            }
        }
        else if (action is ActionBuff)
        {
            // Randomly select a teammate for buffs
            if (validAllies.Count > 0)
            {
                target = validAllies[Random.Range(0, validAllies.Count)];
            }
        }

        if (target != null)
        {
            action.Execute(agent, target);
            if (action.duration > 0)
            {
                activeEffects[target].Add(action);
            }
        }
    }
}