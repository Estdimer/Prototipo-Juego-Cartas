using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class DrawPileManager : MonoBehaviour
{
    public List<Card> drawPile = new List<Card>(); // Baraja de robo de cartas
    private int CurrentIndex = 0;
    public int maxHandSize;
    public int currentHandSize;
    private HandManager handManager;
    private Discard discardManager;
    public TextMeshProUGUI drawPileCounter;
    private bool isBattleStarting = false;
    void Start()
    {
        handManager = FindAnyObjectByType<HandManager>();
        discardManager = FindAnyObjectByType<Discard>();
    }
    void Update()
    {
        // Verifica que el tamaño de la mano sea el correcto en todo tiempo
        if (handManager != null)
        {
            currentHandSize = handManager.cardsInHand.Count;
        }
    }
    public void MakeDrawPile(List<Card> cardToAdd)
    {
        // Carga cartas de assets y las ordena de forma aleatorea en el mazo de donde se robaran las cartas
        drawPile.AddRange(cardToAdd);
        Utility.Shuffle(drawPile);
        UpdateDrawPileCount();
    }
    public void BattleSetup(int numberOfCardsToDraw, int setMaxHandSize)
    {
        // Inicia los valores como el tamaño maximo de la mano y roba las cartas necesarias al inicio de la partida
        maxHandSize = setMaxHandSize;
        isBattleStarting = true;
        for (int i = 0; i < numberOfCardsToDraw; i++)
        {
            DrawCard(handManager);
        }

        isBattleStarting = false;
    }
    public void DrawCard(HandManager handManager)
    {
        // Quita una carta de la baraja de robo y la asigna a la mano del jugador 
        // En caso de estar vacia la baraja de robo toma la lista de cartas de la baraja de descarte y las asigna luego de reodenarlas de froma aleatoria a la baraja de robo
        if (drawPile.Count == 0 && discardManager.discardCardsCount != 0)
        {
            RefillDeckFromDiscard();
        }
        if (drawPile.Count == 0 && discardManager.discardCardsCount == 0)
        {
            return;
        }
        if (currentHandSize < maxHandSize)
        {
            Card NextCard = drawPile[CurrentIndex];
            handManager.AddCardToHand(NextCard);
            if (!isBattleStarting)
            {
                MessageManager.Instance.ShowMessage($"Robaste una carta -> {NextCard.name}");
            }
            drawPile.RemoveAt(CurrentIndex);
            UpdateDrawPileCount();
            if (drawPile.Count > 0)
            {
                CurrentIndex %= drawPile.Count;
            }
        }
    }

    public void DrawCardShop(HandManager handManager) // Solo funciona con el boton para comprar 
    {
        // Quita una carta de la baraja de robo y la asigna a la mano del jugador 
        // En caso de estar vacia la baraja de robo toma la lista de cartas de la baraja de descarte y las asigna luego de reodenarlas de froma aleatoria a la baraja de robo

        if (drawPile.Count == 0 && discardManager.discardCardsCount != 0)
        {
            // Posible error si es que la baraja de robar es 0
            RefillDeckFromDiscard();
        }
        else if (drawPile.Count == 0 && discardManager.discardCardsCount == 0)
        {
            // Posible error si las barajas de robar y descarte son 0
            return;
        }

        if (GameManager.Instance.PlayerGold >= 1)// Asegurarse que se pueda comprar
        {
            GameManager.Instance.PlayerGold -= 1;
        }
        else
        {
            return;
        }

        if (currentHandSize < maxHandSize)
        {
            Card NextCard = drawPile[CurrentIndex];
            handManager.AddCardToHand(NextCard);

            if (!isBattleStarting)
            {
                MessageManager.Instance.ShowMessage($"Robaste una carta -> {NextCard.name}");
            }

            drawPile.RemoveAt(CurrentIndex);
            UpdateDrawPileCount();
            if (drawPile.Count > 0)
            {
                CurrentIndex %= drawPile.Count;
            }
        }
    }
    private void RefillDeckFromDiscard()
    {
        // Reordena la baraja de cartas del descarte y luego la envia a la baraja de robo
        if (discardManager != null && discardManager.discardCardsCount > 0)
        {
            drawPile = discardManager.PullAllFromDiscard();
            Utility.Shuffle(drawPile);
        }
        CurrentIndex = 0;
    }
    private void UpdateDrawPileCount()
    {
        // Actualiza la cantidad de cartas que quedan en la baraja de robo
        drawPileCounter.text = drawPile.Count.ToString();
    }
}
