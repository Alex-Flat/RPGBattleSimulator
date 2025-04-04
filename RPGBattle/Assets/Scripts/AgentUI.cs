using UnityEngine;
using UnityEngine.UI;
using System.Collections;
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
    private List<TMP_Text> floatingNumbers = new List<TMP_Text>();
    private float labelHeight = 14.0f; // Adjust based on prefab height
    private float labelOffset = 30.0f; // Space above health bar

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
        transform.position = Camera.main.WorldToScreenPoint(agent.transform.position + new Vector3(0, 0.85f, 0));
        healthBarRectTransform.position = transform.position;
        attackTimerText.rectTransform.position = Camera.main.WorldToScreenPoint(agent.transform.position);

        for (int i = 0; i < effectLabels.Count; i++)
        {
            if (effectLabels[i] != null)
            {
                effectLabels[i].rectTransform.position = attackTimerText.rectTransform.position - new Vector3(0, (i + 1) * labelHeight + labelOffset, 0);
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

     public void ShowFloatingNumber(float amount, bool isDamage)
    {
        GameObject numberObj = Instantiate(textPrefab, healthBarRectTransform.parent);
        TMP_Text numberText = numberObj.GetComponent<TMP_Text>();
        floatingNumbers.Add(numberText);
        
        // Set text and color
        numberText.text = isDamage ? $"-{amount:F0}" : $"+{amount:F0}";
        numberText.color = isDamage ? Color.red : Color.green;
        
        // Position at agent's location
        numberText.rectTransform.position = Camera.main.WorldToScreenPoint(agent.transform.position + new Vector3(0, 1, 0));
        
        // Add floating animation and destruction
        StartCoroutine(FloatAndFade(numberText));
    }

    private IEnumerator FloatAndFade(TMP_Text text)
    {
        float duration = 1f; // Lifespan of 1 second
        float elapsed = 0f;
        Vector3 startPos = text.rectTransform.position;
        float floatDistance = 50f; // Pixels to float upwards in screen space
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            // Move upwards
            text.rectTransform.position = Vector3.Lerp(startPos, startPos + new Vector3(0, floatDistance, 0), t);
            
            // Fade out
            Color color = text.color;
            color.a = 1f - t;
            text.color = color;
            
            yield return null;
        }
        Destroy(text.gameObject);
        floatingNumbers.Remove(text);
    }

    public void Die()
    {
        foreach (TMP_Text label in effectLabels)
        {
            if (label != null) Destroy(label.gameObject);
        }

        foreach (TMP_Text number in floatingNumbers)
        {
            if (number != null) Destroy(number.gameObject);
        }

        Destroy(attackTimerText.gameObject);
        Destroy(healthBar.gameObject);
        Destroy(gameObject);
    }
}