using UnityEngine;

public class UIObjectPositon : MonoBehaviour
{
    public RectTransform objectToPosition;
    public int widthDivider = 2;
    public int heightDivider = 2;
    public float widthMultiplier = 1f;
    public float heightMultiplier = 1f;
    public bool updatePosition = false; 

    // Llamar a calcular posición al iniciar 
    void Start()
    {
        SetUIObjectPosition();
    }

    // Para cambiar dinamicamente la posición 
    void Update()
    {
        if (updatePosition)
        {
            SetUIObjectPosition();
        }
    }

    public void SetUIObjectPosition()
    {
        if (objectToPosition != null && widthDivider != 0 && heightDivider != 0)
        {
            // Calcular posición en relacion a tamaño de pantalla
            float anchorX = widthMultiplier / widthDivider;
            float anchorY = heightMultiplier / heightDivider;

            // Guardar  posición
            objectToPosition.anchorMin = new Vector2(anchorX, anchorY);
            objectToPosition.anchorMax = new Vector2(anchorX, anchorY);
            objectToPosition.pivot = new Vector2(0.5f, 0.5f);

            // Guardar posicion en 0 para alinear con las nuevas posiciónes 
            objectToPosition.anchoredPosition = Vector2.zero;
        }
    }
}
