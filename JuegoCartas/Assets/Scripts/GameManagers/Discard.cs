using System;
using System.Collections.Generic;
using TMPro;
using Unity.Properties;
using UnityEngine;
using UnityEngine.XR;

public class Discard : MonoBehaviour
{
    // Lista de cartas del descarte 
    [SerializeField] public List<Card> discardCards = new List<Card>();
    public TextMeshProUGUI discardCount;
    public int discardCardsCount;
    private HandManager handManager;

    // 
    void Awake()
    {
        handManager = FindFirstObjectByType<HandManager>();
        UpdateDiscardCount();
    }

    // Actualiza visualmente la cantidad descartada
    private void UpdateDiscardCount()
    {
        discardCount.text = discardCards.Count.ToString();
        discardCardsCount = discardCards.Count;
    }

    // Agrega carta a la lista de descartes
    public void AddToDiscard(Card card)
    {
        if (card != null)
        {
            discardCards.Add(card);
            UpdateDiscardCount();
        }
    }

    // Recive una carta, la busca en la lista de Descarte y la agrega a la mano 
    public bool PullSelectCardFromDiscard(Card card)
    {
        if (discardCards.Count > 0 && discardCards.Contains(card))
        {
            discardCards.Remove(card);
            handManager.AddCardToHand(card);
            UpdateDiscardCount();
            return true;
        }
        else
        {
            return false;
        }
    }

    // Envia la lista de cartas en el descarte 
    public List<Card> PullAllFromDiscard()
    {
        if (discardCards.Count > 0)
        {
            List<Card> cardToReturn = new List<Card>(discardCards);
            discardCards.Clear();
            UpdateDiscardCount();
            return cardToReturn;
        }
        else
        {
            return new List<Card>();
        }
    }

    // Regresa la lista de cartas en el descarte 
    // No confunidr con la anterior que tambien las saca 
    public List<Card> getDiscardCards
    {
        get { return discardCards; }
    }
}
