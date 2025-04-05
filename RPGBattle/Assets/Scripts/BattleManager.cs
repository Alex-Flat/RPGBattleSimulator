using UnityEngine;
using System.Collections.Generic;
using System.Linq;


public class BattleManager : MonoBehaviour
{
    [SerializeField, Range(1, 15), Tooltip("Number of players in the party")] private int playerCount = 3; // Configurable in Inspector
    [SerializeField, Range(1, 15), Tooltip("Number of enemies in the party")] private int enemyCount = 3;  // Configurable in Inspector
    [SerializeField] private Sprite playerSprite; // Assign in the Inspector
    [SerializeField] private Sprite enemySprite;  // Assign in the Inspector
    [SerializeField] private GameObject healthBarPrefab; // Assign in the Inspector
    [SerializeField] private GameObject textPrefab; // Assign in the Inspector
    [SerializeField] private BattleUI battleUI; // Assign in the Inspector

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

            // Initialize agent stats with variance.
            float maxHealth = Constants.BASE_HEALTH + Random.Range(-Constants.STAT_VARIANCE * Constants.BASE_HEALTH, Constants.STAT_VARIANCE * Constants.BASE_HEALTH);
            float attack = Constants.AVG_ATTACK + Random.Range(-Constants.STAT_VARIANCE * Constants.AVG_ATTACK, Constants.STAT_VARIANCE * Constants.AVG_ATTACK);
            float defense = Constants.AVG_DEFENSE + Random.Range(-Constants.STAT_VARIANCE * Constants.AVG_DEFENSE, Constants.STAT_VARIANCE * Constants.AVG_DEFENSE);
            float speed = Constants.AVG_SPEED + Random.Range(-Constants.STAT_VARIANCE * Constants.AVG_SPEED, Constants.STAT_VARIANCE * Constants.AVG_SPEED);

            player.Initialize(Team.Player, maxHealth, attack, defense, speed);
            player.availableActions.Add(new ActionDamage(player));
            player.availableActions.Add(new ActionDoT(player));
            player.availableActions.Add(new ActionHeal(player));
            player.availableActions.Add(new ActionHoT(player));
            player.availableActions.Add(new ActionBuff(player));
            player.availableActions.Add(new ActionDebuff(player));

            playerObj.transform.position = new Vector3(-1.5f * (i / 3) - 1.0f, -2.5f * (i % 3) + 2.5f, i);
            playerTeam.Add(player);
            actionTimers[player] = Constants.START_COOLDOWN;
            activeEffects[player] = new List<Action>();

            player.SetupVisuals(playerSprite, healthBarPrefab, textPrefab);
        }

        // Spawn enemy agents
        for (int i = 0; i < enemyCount; i++)
        {
            GameObject enemyObj = new GameObject($"Enemy_{i}");
            Agent enemy = enemyObj.AddComponent<Agent>();

            // Initialize enemy stats with variance.
            float maxHealth = Constants.BASE_HEALTH + Random.Range(-Constants.STAT_VARIANCE * Constants.BASE_HEALTH, Constants.STAT_VARIANCE * Constants.BASE_HEALTH);
            float attack = Constants.AVG_ATTACK + Random.Range(-Constants.STAT_VARIANCE * Constants.AVG_ATTACK, Constants.STAT_VARIANCE * Constants.AVG_ATTACK);
            float defense = Constants.AVG_DEFENSE + Random.Range(-Constants.STAT_VARIANCE * Constants.AVG_DEFENSE, Constants.STAT_VARIANCE * Constants.AVG_DEFENSE);
            float speed = Constants.AVG_SPEED + Random.Range(-Constants.STAT_VARIANCE * Constants.AVG_SPEED, Constants.STAT_VARIANCE * Constants.AVG_SPEED);

            enemy.Initialize(Team.Enemy, maxHealth, attack, defense, speed);
            enemy.availableActions.Add(new ActionDamage(enemy));
            enemy.availableActions.Add(new ActionDoT(enemy));
            enemy.availableActions.Add(new ActionHeal(enemy));
            enemy.availableActions.Add(new ActionHoT(enemy));
            enemy.availableActions.Add(new ActionBuff(enemy, "defense"));
            enemy.availableActions.Add(new ActionDebuff(enemy, "attack"));

            enemyObj.transform.position = new Vector3(1.5f * (i / 3) + 1.0f, -2.5f * (i % 3) + 2.5f, i);
            enemyTeam.Add(enemy);
            actionTimers[enemy] = Constants.START_COOLDOWN;
            activeEffects[enemy] = new List<Action>();

            enemy.SetupVisuals(enemySprite, healthBarPrefab, textPrefab);
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
            GameOver(winner);
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
            bool needsHealing = validAllies.Any(a => a.CurrHealth / a.MaxHealth < Constants.CRIT_HEALTH_THRESHOLD);
            if (needsHealing)
            {
                // Randomly select a teammate below critical health threshold
                List<Agent> lowHealthAllies = validAllies.Where(a => a.CurrHealth / a.MaxHealth < Constants.CRIT_HEALTH_THRESHOLD).ToList();
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
            action.Execute(target);
            if (action.duration > 0)
            {
                activeEffects[target].Add(action);
            }
        }
        else
        {
            // If no valid target, retry and find a new action.
            PerformAction(agent);
        }
    }

    // Useful if wanting to clean up the battle when the game is over. For now just call GameOver() in the BattleUI script.
    public void GameOver(string winner)
    {
        battleUI.GameOver(winner);
    }
}