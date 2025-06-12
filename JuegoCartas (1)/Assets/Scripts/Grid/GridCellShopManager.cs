using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GridCellShopManager : MonoBehaviour
{
    public int width = 3;
    public int height = 2;
    public GameObject gridCellPrefab;
    public List<Card> shopCards = new List<Card>(); // Lista visible en la jerarquía
    public List<Card> usedCards = new List<Card>(); // Lista de cartas utilizadas
    public string resourcePath = "Shop"; // Carpeta en Resources de donde cargar las cartas
    public GameObject[,] gridCells;
    public List<GameObject> gridObjects = new List<GameObject>();
    public Vector2 cellSize = new Vector2(1.30f, 1.80f); // Cambio: Añadido tamaño de celda configurable
    public Vector2 gridOffset = new Vector2(1.3f, 1.8f); // Permite mover la cuadrícula dinámicamente

    HandManager handManager;

    void Awake()
    {
        handManager = FindAnyObjectByType<HandManager>();
    }

    void Start()
    {
        CreateGrid();
        LoadShopCards();
        PopulateGridWithRandomCards();

    }

    void Update()
    {
        DetectHoverOverCell();
    }

    // Generar al comienzo el grid donde se guardan las cartas a comprar 
    void CreateGrid()
    {
        gridCells = new GameObject[width, height];
        Vector2 centerOffset = new Vector2((width * cellSize.x) / 2.0f - cellSize.x / 2.0f, (height * cellSize.y) / 2.0f - cellSize.y / 2.0f); // cambio: Ajustado para considerar el tamaño de celda

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 gridPosition = new Vector2(x * cellSize.x, y * cellSize.y);
                Vector3 spawnPosition = new Vector3(gridPosition.x - centerOffset.x + gridOffset.x, gridPosition.y - centerOffset.y + gridOffset.y, 1f); // cambio: Se agrega el offset para mover la cuadrícula

                GameObject gridCell = Instantiate(gridCellPrefab, spawnPosition, Quaternion.identity);
                gridCell.transform.SetParent(this.transform);

                // Asigna un nombre único a cada celda en la jerarquía
                gridCell.name = $"Grid Shop ({x}, {y})";

                gridCell.GetComponent<GridCell>().gridIndex = new Vector2(x, y);
                gridCells[x, y] = gridCell;

                gridCell.transform.localScale = new Vector3(cellSize.x, cellSize.y, 1f);
            }
        }
    }

    // Cargar cartas de Carpeta Resouces/Shop 
    void LoadShopCards()
    {
        Card[] loadedCards = Resources.LoadAll<Card>(resourcePath);
        if (loadedCards.Length > 0)
        {
            shopCards.Clear(); // Evitar duplicados
            shopCards.AddRange(loadedCards);
            usedCards.Clear(); // Limpiar lista de cartas usadas
        }
        else
        {
            Debug.LogWarning("No se encontraron cartas en " + resourcePath);
        }
    }

    // Agregar cartas al azar cada vez que exista un espacio disponible 
    void PopulateGridWithRandomCards()
    {
        if (shopCards.Count == 0)
        {
            Debug.LogWarning("GridManager no encontrado o no hay cartas disponibles");
            return;
        }

        // Se usa una lista temporal para evitar modificar la original.
        List<Card> tempShopCards = new List<Card>(shopCards);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (tempShopCards.Count == 0)
                {
                    Debug.LogWarning("Se agotaron las cartas para agregar al grid");
                    return;
                }

                int randomIndex = Random.Range(0, tempShopCards.Count);
                Card selectedCard = tempShopCards[randomIndex];
                tempShopCards.RemoveAt(randomIndex);
                usedCards.Add(selectedCard);

                AddObjectToGridShop(selectedCard, new Vector2(x, y));
            }
        }
        //Aqui quitar las cartas del tempShopCards
        shopCards.RemoveAll(card => !tempShopCards.Contains(card));
    }

    // Agregar  cartas a los botones de la grid 
    public void AddObjectToGridShop(Card obj, Vector2 gridPosition)
    {
        if (gridPosition.x >= 0 && gridPosition.x < width && gridPosition.y >= 0 && gridPosition.y < height)
        {
            GridCell cell = gridCells[(int)gridPosition.x, (int)gridPosition.y].GetComponent<GridCell>();

            if (cell.cellFull) return; //evitar errores 

            Transform buttonTransform = cell.transform.Find("ButtonCardPrefab");
            if (buttonTransform == null)
            {
                Debug.LogError("No se encontró el botón en la celda: " + cell.name);
                return;
            }
            GameObject newObj = Instantiate(obj.prefab, buttonTransform);
            newObj.transform.localPosition = Vector3.zero; // Asegura posición (0,0,0)
            newObj.GetComponent<DisplayCard>().card = obj; // Darle los datos a la cartas de la tienda

            newObj.transform.localScale = new Vector3(1f, 1f, 1f);
            newObj.layer = LayerMask.NameToLayer("CharactersShop");

            Collider2D collider = newObj.GetComponent<Collider2D>();
            if (collider != null)
            {
                collider.enabled = true;
            }
            else
            {
                Debug.LogWarning("No se encontró Collider2D en: " + newObj.name);
            }

            CardMovement cardMovement = newObj.GetComponent<CardMovement>();
            if (cardMovement != null)
            {
                cardMovement.enabled = false;
            }

            gridObjects.Add(newObj);
            cell.objectInCell = newObj;
            cell.cellFull = true;
        }
    }

    // Comprar y su logica
    // Al hacer click verifica si es posible comprar segun el oro del jugador 
    public void OnCardClicked(GameObject clickedCard)
    {

        if (clickedCard.layer != LayerMask.NameToLayer("CharactersShop"))
        {
            return;
        }

        if (GameManager.Instance.CurrentPhase != GameManager.GamePhase.Shop)
        {
            MessageManager.Instance.ShowMessage($"No estas en la fase de Compra");
            return;
        }

        Transform child = clickedCard.transform.Find("CartaPrefab(Clone)");
        if (child != null)
        {
            clickedCard = child.gameObject;
        }

        if (GameManager.Instance.PlayerGold >= clickedCard.GetComponent<DisplayCard>().card.GoldValue)
        {
            GameManager.Instance.PlayerGold -= clickedCard.GetComponent<DisplayCard>().card.GoldValue;

            if (handManager.cardsInHand.Count >= handManager.MaxHandSize)
            {
                Debug.LogWarning($"La mano está llena, no se puede agregar más cartas.(Cartas mmaximas = {handManager.MaxHandSize})");
                return;
            }
            // Buscar carta en la grid de la tienda
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    GridCell cell = gridCells[x, y].GetComponent<GridCell>();

                    if (cell.objectInCell == clickedCard)
                    {
                        handManager.AddCardToHand(clickedCard.GetComponent<DisplayCard>().card);
                        gridObjects.Remove(clickedCard);
                        Destroy(clickedCard);
                        cell.objectInCell = null;
                        cell.cellFull = false;

                        if (shopCards.Count > 0)
                        {
                            int randomIndex = Random.Range(0, shopCards.Count);
                            Card newCard = shopCards[randomIndex];
                            shopCards.RemoveAt(randomIndex); // Eliminar de shopCards
                            usedCards.Add(newCard); // Agregar a usedCards

                            // Instanciar la nueva carta en la misma posición
                            AddObjectToGridShop(newCard, new Vector2(x, y));
                        }
                        else
                        {
                            Debug.LogWarning("No hay más cartas en la tienda para reponer.");
                        }
                        return;
                    }
                }
            }
            Debug.LogWarning("No se encontró la carta en la cuadrícula.");
        }
        else
        {
            MessageManager.Instance.ShowMessage("No tienes oro suficiente ");
        }
    }

    // Detectar si se esta encima de la carta y mostrar el Color del vorde del grid 
    public int DetectHoverOverCell()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider != null)
        {
            GridCell cell = hit.collider.GetComponent<GridCell>();
            if (cell != null && cell.objectInCell != null)
            {
                DisplayCard cardComponent = cell.objectInCell.GetComponent<DisplayCard>();
                if (cardComponent != null)
                {
                    return cardComponent.card.GoldValue; // Devuelve el valor encontrado
                }
            }
        }

        return 1000; // Solo retorna 1000 si no se encuentra ninguna carta
    }
}

