using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    [SerializeField] private GameObject battleTextPrefab;
    private TMP_Text battleText;

    void Start()
    {
        GameObject battleTextInstance = Instantiate(battleTextPrefab, transform);
        battleText = battleTextInstance.GetComponent<TMP_Text>();
    }
    public void GameOver(string winner)
    {
        if (battleText != null)
        {
            battleText.text = $"Battle Over,\n{winner} wins!";
            battleText.transform.SetAsLastSibling();
        }
    }
}
