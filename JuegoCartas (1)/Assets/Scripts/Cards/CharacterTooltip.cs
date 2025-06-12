using UnityEngine;
using System;
using UnityEngine.EventSystems;


public class CharacterTooltip : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
{
    public CharacterStatsTooltip tooltip;
    public float fadeTime = 0.1f;
    public LayerMask CharaterLayerMask;
    public LayerMask CharacterShopLayerMask;
    GameManager gameManager;
    void Awake()
    {
        CharaterLayerMask = LayerMask.GetMask("Characters");
        CharacterShopLayerMask = LayerMask.GetMask("CharactersShop");
        tooltip = FindAnyObjectByType<CharacterStatsTooltip>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Esta funcion envia las estadisticas del character por el cual se esta pasando el mouse al tooltip y lo hace aparecer con FadeIn 
        if (tooltip != null && GameManager.Instance.PlayingCard == false)
        {
            if (GridCharacters())
            {
                tooltip.SetStatsText(GetComponent<CharacterStats>(), GetComponent<DisplayCard>());
                StartCoroutine(Utility.FadeIn(tooltip.canvasGroup, 1f, fadeTime));
            }
            if (ShopCharacters())
            {
                tooltip.SetStatsText(GetComponent<CharacterStats>(), GetComponent<DisplayCard>());
                StartCoroutine(Utility.FadeIn(tooltip.canvasGroup, 1f, fadeTime));
            }

        }
        else
        {
            tooltip = FindAnyObjectByType<CharacterStatsTooltip>();
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        // Hace desaparecer el tooltip llamando FadeOut al mover el mouse lejos del character 
        if (tooltip != null)
        {
            StartCoroutine(Utility.FadeOut(tooltip.canvasGroup, 0f, fadeTime));
        }
    }
    private bool GridCharacters()
    {
        // Comprueba que el personaje bajo el mouse este en la grid de campo de batalla
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, CharaterLayerMask);
        return hit.collider != null;
    }
    private bool ShopCharacters()
    {
        // Comprueba que el personaje bajo el mouse este en la grid de tienda
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos,Vector2.zero,Mathf.Infinity,CharacterShopLayerMask);

        if (hit.collider != null && hit.collider.GetComponent<DisplayCard>() != null)
        {
            if (hit.collider.GetComponent<DisplayCard>().card is SpellCard)
            {
                return false;
            }
        }
        return hit.collider != null;
    }
}

