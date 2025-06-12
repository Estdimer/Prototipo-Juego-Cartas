using UnityEngine;
using UnityEngine.UI;

public class ShopCardButtonHandler : MonoBehaviour
{
    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    // Asignar el onClick del boton que contiene las cartas del gridShop
    void OnClick()
    {
        GridCellShopManager shopManager = FindFirstObjectByType<GridCellShopManager>();
        if (shopManager != null)
        {
            shopManager.OnCardClicked(gameObject);
        }
        else
        {
            Debug.LogWarning("No se encontró el GridCellShopManager en la escena.");
        }
    }
}
