using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using TMPro;
public class CardSelectionManager : MonoBehaviour
{
    public Transform cardContainer;
    public GameObject cardButtonPrefab;
    public GameObject cardPrefab;
    public GameObject cardSelectorPanel;
    public GameObject BossGridPrefab;
    public GameObject ShopGridPrefab;

    public Button confirmButton;
    public Card selectedCard;
    public TMP_Text messageText;


    private Action<Card> onCardSelectedCallback;


    // Recive lista de prefabs, un mensaje y una acción para activarse 
    // y muestra en pantall las cartas para elejir una 
    public void ShowCardSelection(List<GameObject> cardPrefabs, string message, Action<Card> onCardSelected)
    {
        ClearContainer();
        cardSelectorPanel.SetActive(true);
        if (messageText != null) messageText.text = message;
        onCardSelectedCallback = onCardSelected;
        selectedCard = null;
        confirmButton.interactable = false;


        foreach (GameObject cardPrefab in cardPrefabs)
        {
            GameObject newButton = Instantiate(cardButtonPrefab, cardContainer);
            GameObject cardInstance = Instantiate(cardPrefab, newButton.transform);
            ResetTransform(cardInstance);
            cardInstance.transform.localScale = new Vector3(100f, 100f, 1f);

            DisplayCard display = cardInstance.GetComponent<DisplayCard>();
            if (display != null)
            {
                Card cardData = display.card; // Obtener el ScriptableObject u objeto de datos
                Button btn = newButton.GetComponent<Button>();
                if (btn != null)
                {
                    btn.onClick.AddListener(() =>
                    {
                        selectedCard = cardData;
                        confirmButton.interactable = true;
                    });
                }
            }
        }

        // Al seleccionar regresa el seleccionado, limpia y oculta el panel
        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(() =>
        {
            if (selectedCard != null && onCardSelectedCallback != null)
            {
                onCardSelectedCallback.Invoke(selectedCard);
                cardSelectorPanel.SetActive(false);
            }
        });
    }

    // Recive lista de tipo Cards, un mensaje y una acción para activarse 
    // y muestra en pantall las cartas para elejir una 
    public void ShowCardSelection(List<Card> cards, string message, Action<Card> onCardSelected)
    {
        ClearContainer();
        cardSelectorPanel.SetActive(true);
        if (messageText != null) messageText.text = message;


        onCardSelectedCallback = onCardSelected;
        selectedCard = null;
        confirmButton.interactable = false;

        foreach (Card card in cards)
        {
            GameObject newButton = Instantiate(cardButtonPrefab, cardContainer);
            DisplayCard display = newButton.GetComponentInChildren<DisplayCard>();
            if (display != null)
            {
                display.card = card;
                display.UpdateView();
            }

            Button btn = newButton.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(() =>
                {
                    selectedCard = card;
                    confirmButton.interactable = true;
                });
            }
        }
        confirmButton.onClick.RemoveAllListeners(); // Limpia listeners anteriores
        confirmButton.onClick.AddListener(() =>
        {
            if (selectedCard != null && onCardSelectedCallback != null)
            {
                onCardSelectedCallback.Invoke(selectedCard); // Llama al callback con la carta
                cardSelectorPanel.SetActive(false); // Cierra el panel
            }
        });
    }

    // Auxiliar para limpiar antes de mostrar nuevas cartas
    private void ClearContainer()
    {
        foreach (Transform child in cardContainer)
        {
            Destroy(child.gameObject);
        }
    }

    // Auxiliar para transformar correctamente la carta dentro del botón
    private void ResetTransform(GameObject go)
    {
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;
    }
}
