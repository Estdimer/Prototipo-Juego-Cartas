using UnityEngine;

public class CardPreviewManager : MonoBehaviour
{
    public static CardPreviewManager Instance;
    public GameObject cardPrefab; // El mismo prefab que usas en la lista
    public Transform previewContainer; // El panel donde se mostrará la carta clonada
    private GameObject currentPreviewCard;


    private void Awake()// Evitar errores al cargar
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // Mostrar una vista mas grande de la carga seleccionada 
    public void ShowCard(Card cardData)
    {
        if (currentPreviewCard != null)
            Destroy(currentPreviewCard);

        currentPreviewCard = Instantiate(cardPrefab, previewContainer);

        // Copiar escala original del prefab
        RectTransform prefabRect = cardPrefab.GetComponent<RectTransform>();
        RectTransform instanceRect = currentPreviewCard.GetComponent<RectTransform>();

        if (prefabRect != null && instanceRect != null)
        {
            // Copiar la escala original y agrandarla (por ejemplo, 1.5x)
            instanceRect.localScale = prefabRect.localScale * 1.5f;

            // Centrar dentro del contenedor
            instanceRect.anchoredPosition = Vector2.zero;
            instanceRect.localRotation = Quaternion.identity;
        }

        // Asignar información de carta
        DisplayCard display = currentPreviewCard.GetComponentInChildren<DisplayCard>();
        if (display != null)
        {
            display.card = cardData;
            display.UpdateView();
        }
    }
}
