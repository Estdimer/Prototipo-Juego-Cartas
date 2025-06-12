using JetBrains.Annotations;
using UnityEngine;

public class LastBreath : MonoBehaviour
{
    Discard discards;
    public static void Return(CharacterStats character)
    {
        // Ser destruido el character se extrae del descarte y es agregado a la mano del jugador 
        Discard discards = FindAnyObjectByType<Discard>();
        discards.PullSelectCardFromDiscard(character.characterStartData);
    }
    public static void DrawOne()
    {
        // Roba una carta de la baraja de robo
        DrawPileManager drawPileManager = FindAnyObjectByType<DrawPileManager>();
        HandManager handManager = FindAnyObjectByType<HandManager>();  
        drawPileManager.DrawCard(handManager);
    }
}
