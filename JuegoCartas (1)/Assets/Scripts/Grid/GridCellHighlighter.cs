using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(GridCell))]
public class GridCellHighlighter : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public Color highlightColor = Color.cyan; // Hover sin carta
    public Color positiveColor = Color.green; // Se puede poner carta o hechizo
    public Color negativeColor = Color.red; // No se puede poner carta o hechizo
    public Color purchasableColor = Color.yellow; // Se puede comprar
    private Color originalColor;
    public GridCell gridCell;
    private int lastDetectedCardValue = -1;
    private GridCellShopManager gridShopManager;

    public LayerMask gridLayerMask; // Ignorar Characters (Layer 7)
    public LayerMask ignoreLayerMask; // Capas a ignorar (Characters)


    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        gridCell = GetComponent<GridCell>();
        gridShopManager = FindFirstObjectByType<GridCellShopManager>();
        originalColor = spriteRenderer.color;

        // Layers que quiero que sean afaectadas e ignorar las demas 
        gridLayerMask = (1 << 6) | (1 << 8);// Solo afecta la Grid (Layer 6)
        ignoreLayerMask = (1 << 7) | (1 << 9); ; // Ignora Characters (Layer 7)
    }

    void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        LayerMask layerToCheck = (1 << 6) | (1 << 8);

        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, layerToCheck);

        if (hit.collider != null && hit.collider.gameObject == gameObject)
        {
            if (gridShopManager.DetectHoverOverCell() != 1000 && gameObject.layer == 8)
            {
                lastDetectedCardValue = gridShopManager.DetectHoverOverCell();
            } // Solo actualizar cuando hay hover
            HighlightCell();
        }
        else
        {
            ResetHighlight();
        }
    }

    // Cambia el color o agrega borde a la celda para indicar el estado de la carta
    void HighlightCell()
    {
        // Si la celda es parte de la tienda, aplicar color sin necesidad de tener una carta en la mano
        if (gameObject.layer == 8) // Shop y CharactersShop
        {
            spriteRenderer.color = (GameManager.Instance.PlayerGold >= lastDetectedCardValue) ? purchasableColor : negativeColor;
            return;
        }

        if (!GameManager.Instance.PlayingCard)
        {
            spriteRenderer.color = highlightColor;
            return;
        }

        var selectedCard = GameManager.Instance.CurrentCard;
        if (selectedCard == null)
        {
            spriteRenderer.color = originalColor;
            return;
        }

        // Lógica para el Grid de Batalla
        if (selectedCard is SpellCard)
        {
            spriteRenderer.color = gridCell.cellFull ? positiveColor : negativeColor;
        }
        else
        {
            spriteRenderer.color = gridCell.cellFull ? negativeColor : positiveColor;
        }

    }

    void ResetHighlight()
    {
        spriteRenderer.color = originalColor;
    }


}
