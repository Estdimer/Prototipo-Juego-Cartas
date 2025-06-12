using TMPro;
using UnityEngine;

public class UIPlayerStats : MonoBehaviour
{
    public TextMeshProUGUI goldText;  // Referencia al texto de oro en la UI
    public TextMeshProUGUI manaText;  // Referencia al texto de maná en la UI

    void Start()
    {
        UpdateUI();
    }

    void Update()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        // Asigna valores de mana y oro a los elementos UI de PlayerStats
        if (GameManager.Instance != null)
        {
            goldText.text = "Gold: " + GameManager.Instance.PlayerGold;
            manaText.text = "Mana: " + GameManager.Instance.PlayerMana;
        }
    }
}
