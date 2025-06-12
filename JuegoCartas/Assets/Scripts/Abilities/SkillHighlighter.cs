using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(GridCell))]

public class SkillHighlighter : MonoBehaviour
{
    [SerializeField] private GameObject glowEffect;
    public bool Used=true; //boolean usado para llevar la cuenta de si se puede utilizar la habilidad o no 
    public void turnOn()
    {
        // Activa el highlight de la carta y deja el valor de used como true 
        glowEffect.SetActive(true);
        Used = true;

    }
    public void turnOff()
    {
        // Desactiva el Highlight de la carta y deja el valor de used como false
        glowEffect.SetActive(false);
        Used = false;
    }

}
