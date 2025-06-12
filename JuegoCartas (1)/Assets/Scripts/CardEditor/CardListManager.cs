using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardListManager : MonoBehaviour
{
    public GameObject buttonCardPrefab; // Prefab del botón que contiene la carta
    public Transform contentPanel; // El Content del ScrollView
    private List<Card> LoadedCards; // Lista generada desde Resources

    // Cargar y mostrar cartas al iniciar 
    void Start()
    {
        CargarCartasDesdeResources();
        DisplayCard();
    }

    
    void CargarCartasDesdeResources()
    {
        // Carga todos los assets de tipo Card dentro de la carpeta Resources/Cartas
        Card[] cards = Resources.LoadAll<Card>("Cartas");
        LoadedCards = new List<Card>(cards);

        if (LoadedCards.Count == 0)
            Debug.LogWarning("No se encontraron cartas en Resources/Cartas.");
    }

    // Crear un botón que contenga la carta para las cartas cargadas
    public void DisplayCard()
    {
        foreach (Card card in LoadedCards)
        {
            GameObject newButton = Instantiate(buttonCardPrefab, contentPanel);

            DisplayCard display = newButton.GetComponentInChildren<DisplayCard>();
            if (display != null)
            {
                display.card = card;
                display.UpdateView();
            }

            Button boton = newButton.GetComponent<Button>();
            boton.onClick.AddListener(() =>
            {
                Debug.Log("Carta seleccionada: " + card.NameCard);

                // Mostrar en CardPreview (si quieres mantenerlo)
                CardPreviewManager.Instance.ShowCard(card);

                // Mostrar en Panel de Edición
                CardEditorManager.Instance.SelectCardFromOutside(card);
            });
        }
    }
}
