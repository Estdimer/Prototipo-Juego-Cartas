using TMPro;
using UnityEngine;

public class cargarHabilidad : MonoBehaviour
{

    private LayerMask bossLayer;
    private DisplayHero mostrarHero;
    void Awake()
    {
        bossLayer = LayerMask.GetMask("Jefes");
        mostrarHero = GetComponent<DisplayHero>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))// click izquierdo
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null)
            {
                Debug.Log("Objeto impactado: " + hit.collider.name);
            }
            else
            {
                Debug.Log("Nada impactado");
            }
        }
    }

    void Seleccionar()
    {
        // Funcion para seleccionar un heroe, actualemnte no esta siendo utilizado 
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, bossLayer);
        hit = Physics2D.Raycast(mousePos, Vector2.zero, 0f, bossLayer);
        if (hit.collider != null)
        {
            var displayHero = hit.collider.GetComponent<DisplayHero>();
            if (displayHero != null)
            {
                Debug.Log("¡Impactó un héroe!");
            }
        }
    }


    
}
