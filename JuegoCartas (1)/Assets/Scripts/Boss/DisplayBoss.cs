using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
public class DisplayBoss : MonoBehaviour
{
    public Boss boss;
    public TMP_Text NameText;
    public TMP_Text DescriptionText;
    public TMP_Text AttackText;
    public TMP_Text DificultyText;
    public Image Art;
    public Image displayImage;

    void Start()
    {
        UpdateBoss();
    }
    void UpdateBoss()
    {
        if (boss != null)
        {
            // Asignar valores del ScriptableObject a los elementos de UI.
            NameText.text = boss.NameText;
            DescriptionText.text = boss.DescriptionText;
            DificultyText.text = boss.DificultyText;
            AttackText.text = boss.DamageText.ToString();
            Art.sprite = boss.Art;
            displayImage.sprite = boss.Art;
        }
    }
    
}
