using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class TurnManager : MonoBehaviour
{
    public TextMeshProUGUI TurnText;
    private int TurnNumber = 1;
    private GridManager gridManager;
    private HeroManager heroManager;
    public HandManager handManager;
    public Discard discardManager;
    private BotonBatalla battleButton;
    public CardSelectionManager cardSelectionManager;
    public GameObject DrawButton;


    void Start()
    {
        gridManager = FindAnyObjectByType<GridManager>();
        heroManager = FindAnyObjectByType<HeroManager>();
        battleButton = FindAnyObjectByType<BotonBatalla>();
        UpdateTextTurn("Fase de Compra");

    }

    public void NextTurn()
    {
        // Avanzar fase
        switch (GameManager.Instance.CurrentPhase)
        {
            case GameManager.GamePhase.Shop:
                MessageManager.Instance.ShowMessage($"Fase de Administracion", 2);
                UpdateTextTurn("Fase de Administracion");
                DrawButton.SetActive(true);
                GameManager.Instance.SetPhase(GameManager.GamePhase.Administration);
                gridManager.SearchActive();
                break;

            case GameManager.GamePhase.Administration:
                MessageManager.Instance.ShowMessage($"Fase de Combate", 2);
                UpdateTextTurn("Fase de Combate");
                DrawButton.SetActive(false);
                GameManager.Instance.SetPhase(GameManager.GamePhase.Figth);
                gridManager.SearchActive();
                break;

            case GameManager.GamePhase.Figth:
                // Solo al final del ciclo de fases se incrementa el turno
                if (battleButton.passTurn == true)
                {
                    MessageManager.Instance.ShowMessage($"Fase de Compra", 2);
                }
                battleButton.passTurn = true;
                DrawButton.SetActive(false);
                TurnNumber++;
                UpdateTextTurn("Fase de Compra");
                GameManager.Instance.PlayerGold += 3;
                GameManager.Instance.PlayerMana += 3;
                heroManager.HeroSkill();
                GameManager.Instance.PlayerGold = Mathf.Min(GameManager.Instance.PlayerGold, 10);
                GameManager.Instance.PlayerMana = Mathf.Min(GameManager.Instance.PlayerMana, 10);
                // Volver a la primera fase
                StartCoroutine(DiscardUntilLimit(7));
                gridManager.EverythingOff();
                battleButton.ClearFighters();
                GameManager.Instance.SetPhase(GameManager.GamePhase.Shop);
                break;
        }
    }


    void UpdateTextTurn(string fase)
    {
        // Aumenta en 1 el contador de turnos 
        TurnText.text = "Turno: " + TurnNumber +" " +fase;
    }


    private IEnumerator DiscardUntilLimit(int maxHandSize)
    {
        while (handManager.cardsInHand.Count > maxHandSize)
        {
            bool selectionMade = false;
            Card selected = null;

            List<Card> cardDataList = new List<Card>();
            foreach (GameObject cardObj in handManager.cardsInHand)
            {
                DisplayCard display = cardObj.GetComponent<DisplayCard>();
                if (display != null)
                {
                    cardDataList.Add(display.card);
                }
            }

            // Si tiene mas de 7 cartas en la mano seleccionar y descartar
            cardSelectionManager.ShowCardSelection(cardDataList, "Seleccione Cartas a descartar hasta tener 7:", (Card selectedCard) =>
            {
                selected = selectedCard;
                discardManager.AddToDiscard(selected);
                selectionMade = true;
            });

            // Esperar hasta que el jugador seleccione
            yield return new WaitUntil(() => selectionMade);

            // Buscar y eliminar la carta visual
            GameObject toRemove = null;
            foreach (GameObject cardObj in handManager.cardsInHand)
            {
                DisplayCard display = cardObj.GetComponent<DisplayCard>();
                if (display != null && display.card == selected)
                {
                    toRemove = cardObj;
                    break;
                }
            }

            if (toRemove != null)
            {
                handManager.cardsInHand.Remove(toRemove);
                Destroy(toRemove);
                handManager.UpdateHandVisuals();
            }

            // Esperar un frame antes de volver al ciclo
            yield return null;
        }

        // Avanzar de fase una vez completado
        GameManager.Instance.SetPhase(GameManager.GamePhase.Administration);
    }
}
