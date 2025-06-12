using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
 
public class BattleCry : MonoBehaviour
{
    private DeckManager deckManager;
    private GridManager gridManager;
    private DrawPileManager drawPileManager;
    private HandManager handManager;

    // Script encargado de diferenciar que BattleCry se debe activar con un switch 
    public static void battlecry(CharacterStats character)
    {
        DrawPileManager drawPileManager = FindAnyObjectByType<DrawPileManager>();
        HandManager handManager = FindAnyObjectByType<HandManager>();   
        switch (character.Cry)
        {
            case Card.Crys.DrawOne:
                drawPileManager.DrawCard(handManager);
                break;
            case Card.Crys.Health:
                break;
            case Card.Crys.Defense:
                break;
        }
    }
    public static void Search()
    {
        // Funcion de prueba
        // Busca cartas y crea una lista de ellas
        // Luego muestra la lista en pantalla 
        Debug.Log("BattleCry 2");
        GridManager gridManager = FindAnyObjectByType<GridManager>();
        List<GameObject> ListCardPrefab = gridManager.GetCurrentGridObjects();
        CardSelectionManager selector = FindAnyObjectByType<CardSelectionManager>();
        selector.ShowCardSelection(ListCardPrefab, "Seleccione carta a AGREGAR:", OnCardChosen);

        void OnCardChosen(Card selectedCard)
        {
            Debug.Log("Carta elegida: " + selectedCard.name);
        }
        // //prueba  con card 
        // DeckManager deckManager = FindAnyObjectByType<DeckManager>();
        // List<Card> list = deckManager.allCards;
        // CardSelectionManager selector = FindAnyObjectByType<CardSelectionManager>();
        // selector.ShowCardSelection(list, OnCardChosen );
        // //prueba
        // void OnCardChosen(Card selectedCard)
        // {
        //     Debug.Log("Carta elegida: " + selectedCard.name);
        //     // Hacer algo con la carta
        // }
        //return list;
        //return ListCardPrefab;

    }
}

