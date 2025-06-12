using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HandManager : MonoBehaviour
{
    public GameObject cardPrefab; 
    public Transform handTransform; 
    public float fanSpread = 7.5f;
    public float cardSpacing = 100f;
    public float verticalSpacing = 100f;
    public int MaxHandSize;
    public List<GameObject> cardsInHand = new List<GameObject>(); 

    // Recivir y agregar carta a la mano 
    public void AddCardToHand(Card cardData)
    {
        // Generar GameObject de carta prefab
        GameObject newCard = Instantiate(cardPrefab, handTransform.position, Quaternion.identity, handTransform);
        newCard.name = cardData.name;
        cardsInHand.Add(newCard);
        // Asignar valores para que cambie dinamicamente 
        newCard.GetComponent<DisplayCard>().card = cardData;
        newCard.GetComponent<DisplayCard>().UpdateView();

        UpdateHandVisuals();
    }

    // Asignar Maximo de cartas en la mano
    public void BattleSetUp(int setMaxHandSize)
    {
        MaxHandSize = setMaxHandSize;
    }

    // Actualizar como se va a ver la mano, posicion de cartas y incinación 
    public void UpdateHandVisuals()
    {
        int cardCount = cardsInHand.Count;

        if (cardCount == 1)
        {
            cardsInHand[0].transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            cardsInHand[0].transform.localPosition = new Vector3(0f, 0f, 0f);
            return;
        }

        for (int i = 0; i < cardCount; i++)// Cuando hay mas de una carta 
        {
            // Calcular angulo
            float rotationAngle = (fanSpread * (i - (cardCount - 1) / 2f));
            cardsInHand[i].transform.localRotation = Quaternion.Euler(0f, 0f, rotationAngle);
            // Calcular espacio entre cartas 
            float horizontalOffset = (cardSpacing * (i - (cardCount - 1) / 2f));
            // Calcular posición
            float normalizedPosition = (2f * i / (cardCount - 1) - 1f); 
            float verticalOffset = verticalSpacing * (1 - normalizedPosition * normalizedPosition);
            cardsInHand[i].transform.localPosition = new Vector3(horizontalOffset, verticalOffset, 0f);
        }
    }
}
