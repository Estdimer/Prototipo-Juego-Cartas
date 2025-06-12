using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public List<Card> allCards = new List<Card>();

    private int startingHandSize =6;
    public int maxHandSize =12;
    public int currentHandSize;
    private HandManager handManager;
    private DrawPileManager drawPileManager;
    private bool startBattle =true;
    private int currentIndex;

    void Start()
    {
        // Cargar cartas desde la carpeta Resources
        Card[] cards= Resources.LoadAll<Card>("Cartas");
        // Agregar las cartas cargadas a la lista AllCards
        allCards.AddRange(cards);

        handManager = FindFirstObjectByType<HandManager>();
        maxHandSize = handManager.MaxHandSize;
    }


    void Update(){
        if(startBattle)
        {
            BattleSetUp();
            HandManager hand = FindFirstObjectByType<HandManager>();
        }
    }

    // Obtener referencias a otros managers
    void Awake(){
        if(drawPileManager==null){
            drawPileManager=FindAnyObjectByType<DrawPileManager>();
        }
        if(handManager==null){
            handManager=FindAnyObjectByType<HandManager>();
        }
    }

    // Robar una carta y agregarla a la mano 
    public void DrawCard(HandManager handManager)
    {
        if (allCards.Count == 0)
            return;

        if (currentHandSize < maxHandSize)
        {
            Card nextCard = allCards[currentIndex];
            handManager.AddCardToHand(nextCard);
            currentIndex = (currentIndex + 1) % allCards.Count;
        }
    }
    
    // Guarda datos en varios managers
    public void BattleSetUp()
    {
        handManager.BattleSetUp(maxHandSize);
        drawPileManager.MakeDrawPile(allCards);
        drawPileManager.BattleSetup(startingHandSize, maxHandSize);
        startBattle = false;
    }
}
