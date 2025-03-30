using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class BattleUI : MonoBehaviour
{
    public BattleManager battleManager;
    public GameObject agentUIPrefab; // Prefab with Text for name, health, and effects
    private List<GameObject> uiElements = new List<GameObject>();

    void Start()
    {
        SetupUI();
    }

    void Update()
    {
        UpdateUI();
    }

    void SetupUI()
    {
        foreach (Agent agent in battleManager.playerTeam)
        {
            CreateUIElement(agent, new Vector2(50, -50 - uiElements.Count * 30));
        }
        foreach (Agent agent in battleManager.enemyTeam)
        {
            CreateUIElement(agent, new Vector2(300, -50 - (uiElements.Count - battleManager.playerTeam.Count) * 30));
        }
    }

    void CreateUIElement(Agent agent, Vector2 position)
    {
        GameObject uiElement = Instantiate(agentUIPrefab, transform);
        uiElement.GetComponent<RectTransform>().anchoredPosition = position;
        uiElements.Add(uiElement);
    }

    void UpdateUI()
    {
        for (int i = 0; i < uiElements.Count; i++)
        {
            if (i < battleManager.playerTeam.Count)
            {
                UpdateAgentUI(uiElements[i], battleManager.playerTeam[i]);
            }
            else if (i - battleManager.playerTeam.Count < battleManager.enemyTeam.Count)
            {
                UpdateAgentUI(uiElements[i], battleManager.enemyTeam[i - battleManager.playerTeam.Count]);
            }
            else
            {
                uiElements[i].SetActive(false);
            }
        }
    }

    void UpdateAgentUI(GameObject uiElement, Agent agent)
    {
        Text text = uiElement.GetComponent<Text>();
        string effects = string.Join(", ", battleManager.activeEffects[agent].Select(e => $"{e.name} ({e.duration - e.GetTimer():F1}s)"));
        text.text = $"{agent.name}: {agent.CurrHealth}/{agent.MaxHealth} HP\nEffects: {effects}";
    }
}
