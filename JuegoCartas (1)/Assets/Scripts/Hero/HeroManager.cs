using System.Collections;
using UnityEngine;

public class HeroManager : MonoBehaviour
{
    GridManager gridManager;
    HandManager handManager;
    void Start()
    {
        gridManager = FindFirstObjectByType<GridManager>();
        handManager = FindFirstObjectByType<HandManager>();
    }
    public void HeroSkill()
    {
        // Carga el nombre del heroe desde PlayerPrefs
        // PlayerPrefs son variables globales que se guardan entre escenas 
        string heroName = PlayerPrefs.GetString("SelectedHero", "Cupid");
        switch (heroName)
        {
            case "Cupid":
                Cupid();
                break;
            case "San Patric":
                SanPatric();
                break;
        }
    }
    private void Cupid()
    {
        // Si el nombre es cupido activa habilidades relacionadas con numeros pares de cartas en la mano o el grid de characters
        int characterCount = gridManager.NumberOfCharacters;
        int cardCount = handManager.cardsInHand.Count;
        if (characterCount != 0)
        {
            GameManager.Instance.PlayerMana += characterCount / 2;
        }

        if (cardCount % 2 == 0)
        {
            GameManager.Instance.PlayerMana += 1;
        }
    }
    private void SanPatric()
    {
        // Si el nombre es San Patric al conseguir un numero especifico se consigue uno extra de mana por turno 
        int dice = Utility.Diceroll();
        if (dice == 7)
        {
            GameManager.Instance.PlayerMana += 1;
        }
    }
}
