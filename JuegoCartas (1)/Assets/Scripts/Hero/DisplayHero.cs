using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class DisplayHero : MonoBehaviour
{
    public Hero hero;
    public TMP_Text TextoNombre;
    public Image displayImage;
    void Start()
    {
        updateHero();
    }
    void updateHero()
    {
        // Asigna valores UI a los prefab de Hero
        // Actualmente no esta siendo utilizado
        if (hero != null)
        {
            TextoNombre.text = hero.name;
            displayImage.sprite = hero.image;
        }
    }
}
