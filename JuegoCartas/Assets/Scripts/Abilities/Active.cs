using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
public class Active : MonoBehaviour
{

    private DeckManager deckManager;
    private GridManager gridManager;
    private DrawPileManager drawPileManager;
    private HandManager handManager;
    public CardSelectionManager selectionManager;
    public void Revive()
    {
        // Actualmente no esta siendo implementado
        // HACER QUE SAQUE LAS CARTAS DEL DESCARTE Y HACER UN SI NO HAY CARTAS EN EL DESCARTE
        // TAMBIEN TIENE QUE TENER SI SON CHARACTER 

        Discard discards = FindAnyObjectByType<Discard>();
        List<Card> cards = discards.getDiscardCards;

        CardSelectionManager selector = FindAnyObjectByType<CardSelectionManager>();
        selector.ShowCardSelection(cards, "SEleccione carta a revivir:", OnCardChosen);
    }
    public static void Predic()
    {
        // Mira las cartas de la baraja de descarte 
        GridManager gridManager = FindAnyObjectByType<GridManager>();
        DrawPileManager drawPileManager = FindAnyObjectByType<DrawPileManager>();

        List<Card> ListCardPrefab = drawPileManager.drawPile;
        if (ListCardPrefab.Count == 0)
        {
            return;
        }
        CardSelectionManager selector = FindAnyObjectByType<CardSelectionManager>();
        selector.ShowCardSelection(ListCardPrefab, "Seleccione carta a AGREGAR:", OnCardChosen);

        void OnCardChosen(Card selectedCard)
        {
            Debug.Log("Carta elegida: " + selectedCard.name);
        }
    }
    void OnCardChosen(Card selectedCard)
    {
        Debug.Log("Carta elegida: " + selectedCard.NameCard);
        // Aqui se puede hacer algo con la carta
    }
    public static void DrawOne()
    {
        // Roba una carta de la baraja de robo
        DrawPileManager drawPileManager = FindAnyObjectByType<DrawPileManager>();
        HandManager handManager = FindAnyObjectByType<HandManager>();  
        drawPileManager.DrawCard(handManager);
    }
    
}
