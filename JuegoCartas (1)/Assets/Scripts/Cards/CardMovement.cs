using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
//using UnityEditor.Build.Content;
//using UnityEditor.U2D.Animation;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;

public class CardMovement : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private RectTransform canvasRectTranform;
    private Vector3 originalScale;
    private int currentState = 0;
    private Quaternion originalRotation;
    private Vector3 originalPosition;
    private GridManager gridManager;
    HandManager handManager;
    Discard discard;

    // Necesario par asignar posciones y efectos dentro de la mano 
    [SerializeField] private float selectScale = 1.1f;
    [SerializeField] private Vector2 cardPlay;
    [SerializeField] private Vector3 playPosition;
    [SerializeField] private GameObject glowEffect;
    [SerializeField] private GameObject playArrow;
    [SerializeField] private float lerpFactor = 0.1f;
    [SerializeField] private int cardPlayDivider = 4;
    [SerializeField] private float cardPlayMultiplier = 1f;
    [SerializeField] private bool needUpdateCardPlayPosition = false;
    [SerializeField] private int playPositionYDivider = 2;
    [SerializeField] private float playPositionYMultiplier = 1f;
    [SerializeField] private int playPositionXDivider = 4;
    [SerializeField] private float playPositionXMultiplier = 1f;
    [SerializeField] private bool needUpdatePlayPosition = false;

    // Cambios para spell y characters
    private LayerMask gridLayerMask;
    private LayerMask CharacterLayerMask;
    private Card CardData;
    private DisplayCard displayCard;
    private int ManaCard;

    void Awake()
    {
        // Recoger variables y llamar scritps necesarios para las funciones 
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        if (canvas != null)
        {
            canvasRectTranform = canvas.GetComponent<RectTransform>();
        }

        originalScale = rectTransform.localScale;
        originalPosition = rectTransform.localPosition;
        originalRotation = rectTransform.localRotation;

        updateCardPlayPostion();
        updatePlayPostion();


        gridManager = FindAnyObjectByType<GridManager>();
        handManager = FindAnyObjectByType<HandManager>();
        discard = FindAnyObjectByType<Discard>();
        displayCard = GetComponent<DisplayCard>();
        
        // Buscar las layers
        gridLayerMask = LayerMask.GetMask("Grid");
        CharacterLayerMask = LayerMask.GetMask("Characters");
        CardData = displayCard.card;

    }

    void Update()
    {
        // Cada vez que se interactua con una carta se modifica segun el estado en el que se encuentra la carta en el momento 
        if (needUpdateCardPlayPosition)
        {
            updateCardPlayPostion();
        }

        if (needUpdatePlayPosition)
        {
            updatePlayPostion();
        }
        if (CardData != displayCard.card)
        {
            CardData = displayCard.card;
        }


        switch (currentState)
        {
            case 1: // Hove over
                HandleHoverState();
                break;
            case 2: // Drag
                HandleDragState();
                if (!Input.GetMouseButton(0)) // Soltar el boton del mouse
                {
                    TransitionToState0();
                }
                break;
            case 3: // Jugar carta
                HandlePlayState(); // Soltar carta en grid
                break;
        }
    }

    private void TransitionToState0() // Reiniciar carta a estado original
    {
        currentState = 0;
        GameManager.Instance.PlayingCard = false; // Se deja de jugar la carta
        GameManager.Instance.CurrentCard = null; // Limpiar la carta actual cuando se suelta
        rectTransform.localScale = originalScale;
        rectTransform.localRotation = originalRotation;
        rectTransform.localPosition = originalPosition;
        glowEffect.SetActive(false);
        playArrow.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData) // Cambia el estado a 1 hover
    {
        if (currentState == 0 && !GameManager.Instance.PlayingCard && GameManager.Instance.CurrentCard == null)
        {
            originalPosition = rectTransform.localPosition;
            originalRotation = rectTransform.localRotation;
            originalScale = rectTransform.localScale;
            currentState = 1;
        }
    }

    public void OnPointerExit(PointerEventData eventData) // Regresa a estado original cuando deja de hacer hove over
    {
        if (currentState == 1)
        {
            TransitionToState0();
        }
    }

    public void OnPointerDown(PointerEventData eventData)// Cambia estado a estado 2 (drag - mover) cuando se mantiene click en carta 
    {
        if (GameManager.Instance.CurrentPhase != GameManager.GamePhase.Administration)
            return;
        if (currentState == 1)
        {
            currentState = 2;
            GameManager.Instance.CurrentCard = displayCard.card; // Guardar la carta actual
        }
    }

    public void OnDrag(PointerEventData eventData)// Si se sube la carta  aparece la flecha 
    {
        if (currentState == 2)
        {
            if (Input.mousePosition.y > cardPlay.y)
            {
                currentState = 3;
                playArrow.SetActive(true);
                rectTransform.localPosition = Vector3.Lerp(rectTransform.position, playPosition, lerpFactor);
            }
        }
    }

    private void HandleHoverState() // Hove over  brillo y agrandar carta en mano
    {
        glowEffect.SetActive(true);
        rectTransform.localScale = originalScale * selectScale;
    }

    private void HandleDragState()// Mover carta y que esta este en el estado correcto (sin inclinación)
    {
        rectTransform.localRotation = Quaternion.identity;
        rectTransform.position = Vector3.Lerp(rectTransform.position, Input.mousePosition, lerpFactor); //lerp
    }


    private void HandlePlayState()// Jugar carta en el campo
    {
        if (!GameManager.Instance.PlayingCard)
        {
            GameManager.Instance.PlayingCard = true;
        }

        if (GameManager.Instance != null && GameManager.Instance.CurrentCard != null)
        {
            ManaCard = GameManager.Instance.CurrentCard.ManaValue;
        }
        else
        {
            Debug.LogWarning("GameManager o CurrentCard es null en HandlePlayState");
        }



        rectTransform.localPosition = playPosition;
        rectTransform.localRotation = Quaternion.identity;

        if (GameManager.Instance.PlayerMana >= ManaCard) // Ver que tenga mana suficiente
        {
            if (!Input.GetMouseButton(0)) // Verificar si se solto el mouse 
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (CardData is Character characterCard)
                {
                    tryToPlayCharacter(ray, characterCard);
                }
                else if (CardData is SpellCard spellCarta)
                {
                    tryToPlaySpell(ray, spellCarta);
                }
                TransitionToState0();
            }



            if (Input.mousePosition.y < cardPlay.y)
            {
                currentState = 2;
                playArrow.SetActive(false);
            }
        }
        else // Si no tiene el mana suficiente 
        {
            MessageManager.Instance.ShowMessage("No tienes mana suficiente ");
            TransitionToState0();
            return;
        }
    }


    private void tryToPlayCharacter(Ray ray, Character character)// Invoca personaje en la grid
    {
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, gridLayerMask);

        GetComponent<DisplayCard>().card.prefab.GetComponent<DisplayCard>().card = GetComponent<DisplayCard>().card;

        if (hit.collider != null && hit.collider.TryGetComponent<GridCell>(out var cell))
        {

            Vector2 targetPos = cell.gridIndex;
            if (gridManager.AddObjectToGrid(character.prefab, targetPos))
            {
                GameManager.Instance.PlayerMana -= ManaCard;//Gastar mana
                cell.objectInCell.GetComponent<CharacterStats>().characterStartData = character;
                cell.objectInCell.GetComponent<CharacterStats>().Update();
                handManager.cardsInHand.Remove(gameObject);
                if (cell.objectInCell.GetComponent<DisplayCard>().Skill == Card.SkillType.battlecry)
                {
                    Skills.Trigger(cell.objectInCell.GetComponent<DisplayCard>(), cell.objectInCell.GetComponent<CharacterStats>());
                }

                handManager.UpdateHandVisuals();
                Destroy(gameObject);
            }
        }
    }
    private void tryToPlaySpell(Ray ray, SpellCard spell)// Aplica efecto hechizo
    {

        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, CharacterLayerMask);

        if (hit.collider != null && hit.collider.TryGetComponent<CharacterStats>(out var targetStats))
        {
            GameManager.Instance.PlayerMana -= ManaCard;// Gastar mana
            if (hit.collider.GetComponent<DisplayCard>().Skill == Card.SkillType.pasive)
            {
                Skills.Trigger(hit.collider.GetComponent<DisplayCard>(), hit.collider.GetComponent<CharacterStats>());
            }
            SpellEffectApplier.ApplySpell(spell, targetStats);
            handManager.cardsInHand.Remove(gameObject);
            discard.AddToDiscard(CardData);
            handManager.UpdateHandVisuals();
            Destroy(gameObject);
            // Cambiar partes del  clon  de la carta 
            hit.collider.GetComponent<DisplayCard>().HealthText.text = targetStats.Health.ToString();
            hit.collider.GetComponent<DisplayCard>().AttackText.text = targetStats.Attack.ToString();
            gridManager.removeDeads();
            if (spell.spellType == Card.SpellType.buff)
            {
                if (spell.attributeTargets[0] == Card.AttributeTarget.Health && targetStats.characterStartData.Health <= targetStats.Health)
                {
                    hit.collider.GetComponent<DisplayCard>().HealthText.color = Color.green;
                }
                if (spell.attributeTargets[0] == Card.AttributeTarget.Attack)
                {
                    hit.collider.GetComponent<DisplayCard>().AttackText.color = Color.green;
                }
            }
            else
            {
                if (spell.attributeTargets[0] == Card.AttributeTarget.Health && targetStats.characterStartData.Health > targetStats.Health)
                {
                    hit.collider.GetComponent<DisplayCard>().HealthText.color = Color.red;
                }
                if (spell.attributeTargets[0] == Card.AttributeTarget.Attack)
                {
                    hit.collider.GetComponent<DisplayCard>().AttackText.color = Color.red;
                }
            }

        }
    }
    private void updateCardPlayPostion()// Para cambiar el eje Y desde donde jugar carta 
    {
        if (cardPlayDivider != 0 && canvasRectTranform != null)
        {
            float segment = cardPlayMultiplier / cardPlayDivider;

            cardPlay.y = canvasRectTranform.rect.height * segment;
        }
    }

    private void updatePlayPostion()// Para cambiar el eje X e Y  desde donde jugar carta 
    {
        if (canvasRectTranform != null && playPositionYDivider != 0 && playPositionXDivider != 0)
        {
            float segmentX = playPositionXMultiplier / playPositionXDivider;
            float segmentY = playPositionYMultiplier / playPositionYDivider;

            playPosition.x = canvasRectTranform.rect.width * segmentX;
            playPosition.y = canvasRectTranform.rect.height * segmentY;
        }
    }

}
