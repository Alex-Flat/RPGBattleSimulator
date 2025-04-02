using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentUI : MonoBehaviour
{
    public HealthBar healthBar;
    private RectTransform healthBarRectTransform;
    private Agent agent;

    public void Initialize(Agent agent, float maxHealth)
    {
        this.agent = agent;
        healthBar.Initialize(maxHealth);
        healthBarRectTransform = healthBar.GetComponent<RectTransform>();
    }

    void Update()
    {
        UpdateUI();
        UpdatePosition();
    }

    void UpdatePosition()
    {
        transform.position = Camera.main.WorldToScreenPoint(agent.transform.position + new Vector3(0, 1, 0));
        healthBarRectTransform.position = transform.position;
    }

    void UpdateUI()
    {
        healthBar.SetHealth(agent.CurrHealth);
    }

    public void Die()
    {
        Destroy(healthBar.gameObject);
        Destroy(gameObject);
    }
}
