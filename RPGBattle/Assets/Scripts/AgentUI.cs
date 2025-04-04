using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class AgentUI : MonoBehaviour
{
    public HealthBar healthBar;
    private RectTransform healthBarRectTransform;
    private Agent agent;
    private GameObject healthBarPrefab;
    private GameObject textPrefab; // Store the prefab reference
    private TMP_Text attackTimerText;
    private List<TMP_Text> effectLabels = new List<TMP_Text>();
    private float labelHeight = 14.0f; // Adjust based on prefab height
    private float labelOffset = 3.0f; // Space above health bar

    public void Initialize(Agent agent, float maxHealth, GameObject healthBarPrefab, GameObject textPrefab)
    {
        this.agent = agent;
        this.healthBarPrefab = healthBarPrefab;
        this.textPrefab = textPrefab;

        GameObject healthBarInstance = Instantiate(healthBarPrefab, FindObjectOfType<Canvas>().transform);
        healthBar = healthBarInstance.GetComponent<HealthBar>();

        GameObject attackTimerInstance = Instantiate(textPrefab, FindObjectOfType<Canvas>().transform);
        attackTimerText = attackTimerInstance.GetComponent<TMP_Text>();
        attackTimerText.color = Color.black;

        healthBar.Initialize(maxHealth);
        healthBarRectTransform = healthBar.GetComponent<RectTransform>();
    }

    void Update()
    {
        UpdateHealthBarUI();
        UpdatePosition();
        UpdateEffectsUI();
        UpdateAttackTimerUI();
    }

    void UpdatePosition()
    {
        transform.position = Camera.main.WorldToScreenPoint(agent.transform.position + new Vector3(0, 1, 0));
        healthBarRectTransform.position = transform.position;
        attackTimerText.rectTransform.position = Camera.main.WorldToScreenPoint(agent.transform.position);

        for (int i = 0; i < effectLabels.Count; i++)
        {
            if (effectLabels[i] != null)
            {
                effectLabels[i].rectTransform.position = transform.position + new Vector3(0, i * labelHeight + labelOffset, 0);
            }
        }
    }

    void UpdateHealthBarUI()
    {
        healthBar.SetHealth(agent.CurrHealth);
    }

    void UpdateEffectsUI()
    {
        List<Action> activeEffects = agent.activeEffects;

        // Remove excess labels
        while (effectLabels.Count > activeEffects.Count)
        {
            Destroy(effectLabels[effectLabels.Count - 1].gameObject);
            effectLabels.RemoveAt(effectLabels.Count - 1);
        }

        // Add new labels using the prefab
        while (effectLabels.Count < activeEffects.Count)
        {
            GameObject labelObj = Instantiate(textPrefab, healthBarRectTransform.parent);
            TMP_Text label = labelObj.GetComponent<TMP_Text>();
            label.rectTransform.anchorMin = new Vector2(0.5f, 0);
            label.rectTransform.anchorMax = new Vector2(0.5f, 0);
            label.rectTransform.pivot = new Vector2(0.5f, 0);
            effectLabels.Add(label);
        }

        // Update label text and color
        for (int i = 0; i < activeEffects.Count; i++)
        {
            Action effect = activeEffects[i];
            TMP_Text label = effectLabels[i];
            bool isDebuff = effect is ActionDebuff; // Adjust based on your class names
            float remainingTime = effect.GetTimer();

            if (effect is ActionDebuff || effect is ActionDoT)
            {
                label.text = $"- {effect.name} {remainingTime:F1}";
                label.color = Color.red;
            }
            else
            {
                label.text = $"+ {effect.name} {remainingTime:F1}";
                label.color = Color.green;
            }
        }
    }

    void UpdateAttackTimerUI()
    {
        attackTimerText.text = $"{agent.actionTimer:F1}";
    }

    public void Die()
    {
        foreach (TMP_Text label in effectLabels)
        {
            if (label != null) Destroy(label.gameObject);
        }
        Destroy(attackTimerText.gameObject);
        Destroy(healthBar.gameObject);
        Destroy(gameObject);
    }
}