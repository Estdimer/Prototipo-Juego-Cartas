using UnityEngine;

public class Pasive : MonoBehaviour
{
    GameManager gameManager;
    public static void Crumble(DisplayCard character, CharacterStats stats)
    {
        // Cambia la vida de un character a 0
        character.HealthText.text = 0.ToString();
        stats.Health = 0;
    }
    public static void Armored()
    {
        // Al recibir un hechizo devuelve 1 de mana al jugador 
        GameManager gameManager = FindAnyObjectByType<GameManager>();
        GameManager.Instance.PlayerMana += 1;
    }
}
