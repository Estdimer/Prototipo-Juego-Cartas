using UnityEngine;
using UnityEngine.EventSystems;

public class SkillTrigger : MonoBehaviour
{
    private LayerMask CharacterLayerMask;
    // Este script maneja la activacion de habilidades tipo Active
    void Awake()
    {
        CharacterLayerMask = LayerMask.GetMask("Characters");
    }

    void Update()
    {
        // Al hacer click se activa Activate
        if (Input.GetMouseButtonDown(0))
        {
            Activate();
        }
    }

    void Activate()
    {
        // Comprueba si la fase es la de administracion 
        if (GameManager.Instance.CurrentPhase != GameManager.GamePhase.Administration)
        { 
            return;
        }
        // Verifica que la fase sea de Aministracio, se haya hecho click a una carta con SkillHighlighter y si en SkillHighlighter used es true 
        Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mouse, Vector2.zero, Mathf.Infinity, CharacterLayerMask);
        if (hit.collider != null && GameManager.Instance.CurrentPhase == GameManager.GamePhase.Administration && hit.collider.GetComponent<SkillHighlighter>()!=null && hit.collider.GetComponent<SkillHighlighter>().Used== true)
        {
            // En caso de cumplir todas las condiciones verifica si el personaje posee una habilidad tipo active 
            var display = hit.collider.GetComponent<DisplayCard>();
            var character = hit.collider.GetComponent<CharacterStats>();
            if (display != null && character != null && display.Skill == Card.SkillType.active)
            {
                // Activa la habilidad con al funcion ApplySkill y luego llama la funcion turnoff para apagar el Highlighter y cambiar el valor de used a false
                ApplySkill(display, character);
                hit.collider.GetComponent<SkillHighlighter>().turnOff();
            }
            
        }
    }

    void ApplySkill(DisplayCard source, CharacterStats stats)
    {
        // Envia los datos de la carta al script Skills 
        Skills.Trigger(source, stats);
    }
}
