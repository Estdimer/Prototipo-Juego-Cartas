using UnityEngine;
using UnityEngine.UI;
using TMPro;
using JetBrains.Annotations;
using System.Runtime.CompilerServices; // Importar TextMeshPro
using static Card;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
public class DisplayCard : MonoBehaviour
{
    // todas las cartas
    public Card card; // Asigna el ScriptableObject aquí dinámicamente.

    public TMP_Text NameText;
    public TMP_Text DescriptionText;
    public TMP_Text ManaText;
    public TMP_Text GoldText;
    public Image Art;
    public TMP_Text TypeCardText;
    public Image displayImage;
    // Parte de la carta que se pueden activar o desactivar dependiendo si es necesario 
    public GameObject HealthImage;
    public GameObject AttackImage;
    public GameObject DefenseImage;
    public GameObject GoldImage;

    // characters
    public TMP_Text HealthText;
    public TMP_Text AttackText;
    public TMP_Text DefenseText;
    public Factions Faction;
    public SkillType Skill;

    public Crys Cry;

    // Spell
    public GameObject[] attributeTargetSymbols;
    public float attributeSymbolSpacing = 10f;
    public TMP_Text attributeChangeAmountText;


    void Start()
    {
        UpdateView();
    }

    // Cuando se actualize los datos de la carta cambiar visualmente
    public void UpdateView()
    {
        if (card != null)
        {
            NameText.text = card.NameCard;
            ManaText.text = card.ManaValue.ToString();
            GoldText.text = card.GoldValue.ToString();
            Art.sprite = card.SpriteCard;
            TypeCardText.text = card.typeCard.ToString();
            displayImage.sprite = card.SpriteCard;

            if (int.Parse(GoldText.text) == 0)
            {
                GoldText.text = "";
                GoldImage.SetActive(false);
            }

            // Verificar tipo y construir descripción dinámica
            if (card is Character characterCard)
            {
                UpdateDisplayCharacterCarta(characterCard);
                DescriptionText.text = GenerateCharacterDescription(characterCard);
            }
            else if (card is SpellCard spellCard)
            {
                UpdateDisplaySpellCarta(spellCard);
                DescriptionText.text = GenerateSpellDescription(spellCard);
            }
            else
            {
                DescriptionText.text = card.Description; // Por defecto
            }
        }
        else
        {
            Debug.LogError("No se ha asignado ninguna carta al script MostrarCarta.");
        }
    }

    // Si es character generar la descripción  segun las habilidades que tiene 
    private string GenerateCharacterDescription(Character characterCard)
    {
        if (characterCard.Skill == SkillType.none)
        {
            return ""; // No retorna descripción si no tiene habilidad
        }

        string inicio = characterCard.Skill switch
        {
            SkillType.battlecry => "Al entrar a la zona de juego ",
            SkillType.lastBreath => "Al morir ",
            SkillType.pasive => "Pasiva: ",
            SkillType.active => "Una vez por turno ",
            _ => ""
        };

        string efecto = characterCard.Cry switch
        {
            Crys.DrawOne => "roba una carta.",
            Crys.Health => "aumenta la vida.",
            Crys.Defense => "aumenta la defensa.",
            Crys.Return_LastBreath => "regresa a la mano.",
            Crys.Pasive_Crumble => "después de pelear muere.",
            Crys.Revive => "Agrega al campo uno de los Mercenarios del descarte.",
            Crys.Pasive_Armored => "si es objetivo de un hechizo,regresa 1 de maná.",
            Crys.Search => "todavía no hace nada.",
            Crys.Predic => "mustra cartas del mazo",
            _ => ""
        };

        return inicio + efecto;
    }

    // Si es SpellCard generar la descripción segun que hace 
    private string GenerateSpellDescription(SpellCard spellCard)
    {
        string inicio = spellCard.spellType switch
        {
            SpellType.buff => "Aumenta el atributo de ",
            SpellType.debuff => "Disminuye el atributo de ",
            _ => ""
        };

        List<string> efectos = new List<string>();

        for (int i = 0; i < spellCard.attributeTargets.Count; i++)
        {
            string atributo = spellCard.attributeTargets[i] switch
            {
                AttributeTarget.Attack => "Ataque en ",
                AttributeTarget.Health => "Vida en ",
                AttributeTarget.Defense => "Defensa en ",
                _ => ""
            };

            int cambio = (i < spellCard.attributeChangeAmount.Count) ? spellCard.attributeChangeAmount[i] : 0;
            efectos.Add(atributo + cambio);
        }

        return inicio + string.Join(", ", efectos);
    }


    // Actualiza datos en DisplayCharacterCarta para generar el tooltip
    private void UpdateDisplayCharacterCarta(Character charactercard)
    {
        HealthText.text = charactercard.Health.ToString();
        AttackText.text = charactercard.Attack.ToString();
        DefenseText.text = charactercard.Defense.ToString();
        Cry = charactercard.Cry;
        Faction = charactercard.Faction;
        Skill = charactercard.Skill;

        this.GetComponent<CharacterStats>().characterStartData = card as Character;
        this.GetComponent<CharacterStats>().Update();
    }

    // Si es SpellCard Actualiza visualmente para no mostrar lo que contiene Character
    private void UpdateDisplaySpellCarta(SpellCard spellCard)
    {
        HealthImage.SetActive(false);
        AttackImage.SetActive(false);
        DefenseImage.SetActive(false);
        HealthText.text = "";
        AttackText.text = "";
        DefenseText.text = "";
    }

    // Al hacer click en carta de el grid de  la tienda intentar comprar
    private void OnMouseDown()
    {
        GridCellShopManager shopManager = FindFirstObjectByType<GridCellShopManager>();
        if (shopManager != null)
        {
            shopManager.OnCardClicked(gameObject);
        }
    }
}