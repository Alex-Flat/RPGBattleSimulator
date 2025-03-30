using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private int playerCount = 3; // Configurable in Inspector
    [SerializeField] private int enemyCount = 3;  // Configurable in Inspector
    [SerializeField] private Sprite playerSprite; // Assign circle sprite in Inspector
    [SerializeField] private Sprite enemySprite;  // Assign square sprite in Inspector

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
            player.team = Team.Player;
            player.Initialize(100f, 20f, 30f, 5f + i * 2, playerSprite); // Vary speed slightly
            player.availableActions.Add(new ActionDamage(20f));
            player.availableActions.Add(new ActionHeal(15f));
            player.availableActions.Add(new ActionBuff("attack", 10f, 5f));
            playerObj.transform.position = new Vector3(-2f * (playerCount - i), 0, 0);
            playerTeam.Add(player);
            actionTimers[player] = 0f;
            activeEffects[player] = new List<Action>();
        }

        // Spawn enemy agents
        for (int i = 0; i < enemyCount; i++)
        {
            GameObject enemyObj = new GameObject($"Enemy_{i}");
            Agent enemy = enemyObj.AddComponent<Agent>();
            enemy.team = Team.Enemy;
            enemy.Initialize(80f, 15f, 20f, 4f + i * 2, enemySprite); // Vary speed slightly
            enemy.availableActions.Add(new ActionDamage(15f));
            enemy.availableActions.Add(new ActionDoT(5f, 6f, 2f));
            enemy.availableActions.Add(new ActionDebuff("defense", 10f, 5f));
            enemyObj.transform.position = new Vector3(2f * (i + 1), 0, 0);
            enemyTeam.Add(enemy);
            actionTimers[enemy] = 0f;
            activeEffects[enemy] = new List<Action>();
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
            actionTimers[agent] += Time.deltaTime;
            if (actionTimers[agent] >= agent.GetActionInterval())
            {
                PerformAction(agent);
                actionTimers[agent] = 0f;
            }
        }
    }

    void PerformAction(Agent agent)
    {
        if (agent.availableActions.Count == 0) return;

        // Randomly select an action
        Action action = agent.availableActions[Random.Range(0, agent.availableActions.Count)];
        List<Agent> targets = agent.team == Team.Player ? enemyTeam : playerTeam;
        List<Agent> allies = agent.team == Team.Player ? playerTeam : enemyTeam;

        Agent target = null;
        if (action is ActionDamage || action is ActionDoT || action is ActionDebuff)
        {
            target = targets.FirstOrDefault(t => t != null && t.CurrHealth > 0);
        }
        else if (action is ActionHeal || action is ActionHoT || action is ActionBuff)
        {
            target = allies.FirstOrDefault(a => a != null && a.CurrHealth > 0);
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