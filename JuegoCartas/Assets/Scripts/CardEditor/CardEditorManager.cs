using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Collections.Generic;

public class CardEditorManager : MonoBehaviour
{
    public Transform cardContainer;
    public GameObject cardButtonPrefab;
    public GameObject editorPanel;

    public TMP_InputField manaInput;
    public TMP_InputField goldInput;
    public TMP_InputField healthInput;
    public TMP_InputField attackInput;
    public TMP_InputField defenseInput;

    private Card selectedCard;
    public Card predefinedCard;

    public TMP_Dropdown spellTypeDropdown;
    public TMP_Dropdown attributeTargetDropdown;

    public TMP_Dropdown skillTypeDropdown;
    public TMP_Dropdown crysDropdown;
    public TMP_InputField attributeAmountInput;


    public static CardEditorManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }


    void Start()
    {
        editorPanel.SetActive(false);// Ocultar el panel del editor al iniciar
    }

    // Cargar carta seleccionada del panel de edicion y actualizar los campos
    public void SelectCardFromOutside(Card card)
    {
        selectedCard = card;
        editorPanel.SetActive(true);

        manaInput.text = card.ManaValue.ToString();
        goldInput.text = card.GoldValue.ToString();

        // Verificar si es character y mostrar campos 
        if (card is Character character)
        {
            healthInput.gameObject.SetActive(true);
            attackInput.gameObject.SetActive(true);
            defenseInput.gameObject.SetActive(true);
            attributeAmountInput.gameObject.SetActive(false);


            healthInput.text = character.Health.ToString();
            attackInput.text = character.Attack.ToString();
            defenseInput.text = character.Defense.ToString();
        }
        else
        {
            healthInput.gameObject.SetActive(false);
            attackInput.gameObject.SetActive(false);
            defenseInput.gameObject.SetActive(false);
        }

        // Verificamos si es un Spell y mostrar campos 
        if (selectedCard is SpellCard spellCard)
        {
            spellTypeDropdown.gameObject.SetActive(true);
            attributeTargetDropdown.gameObject.SetActive(true);
            skillTypeDropdown.gameObject.SetActive(false);
            crysDropdown.gameObject.SetActive(false);
            attributeAmountInput.gameObject.SetActive(true);

            spellTypeDropdown.ClearOptions();
            attributeTargetDropdown.ClearOptions();

            // Desplegar los dropdown
            spellTypeDropdown.AddOptions(System.Enum.GetNames(typeof(Card.SpellType)).ToList());
            attributeTargetDropdown.AddOptions(System.Enum.GetNames(typeof(Card.AttributeTarget)).ToList());
            // Setear valores actuales
            spellTypeDropdown.value = (int)spellCard.spellType;

            if (spellCard.attributeTargets != null && spellCard.attributeTargets.Count > 0)
            {
                attributeTargetDropdown.value = (int)spellCard.attributeTargets[0];
            }

            if (spellCard.attributeChangeAmount != null && spellCard.attributeChangeAmount.Count > 0)
            {
                attributeAmountInput.text = spellCard.attributeChangeAmount[0].ToString();
            }
            else
            {
                attributeAmountInput.text = "0";
            }
        }
        else if (selectedCard is Character)
        {
            spellTypeDropdown.gameObject.SetActive(false);
            attributeTargetDropdown.gameObject.SetActive(false);
            skillTypeDropdown.gameObject.SetActive(true);
            crysDropdown.gameObject.SetActive(true);

            skillTypeDropdown.ClearOptions();
            skillTypeDropdown.AddOptions(System.Enum.GetNames(typeof(Card.SkillType)).ToList());
            skillTypeDropdown.onValueChanged.RemoveAllListeners();
            skillTypeDropdown.onValueChanged.AddListener(delegate { UpdateCrysDropdown(); });

            UpdateCrysDropdown();
        }
        else
        {
            spellTypeDropdown.gameObject.SetActive(false);
            attributeTargetDropdown.gameObject.SetActive(false);
            skillTypeDropdown.gameObject.SetActive(false);
            crysDropdown.gameObject.SetActive(false);
        }
    }

    // Actualiza las opciones  de dropdown segun el tipo de habilidad 
    private void UpdateCrysDropdown()
    {
        string selectedSkillType = skillTypeDropdown.options[skillTypeDropdown.value].text;

        crysDropdown.ClearOptions();

        List<string> options = new();

        if (selectedSkillType == "pasive")
        {
            options.Add("Pasive_Crumble");
            options.Add("Pasive_Armored");
        }
        else
        {
            foreach (string name in System.Enum.GetNames(typeof(Card.Crys)))
            {
                if (name != "Pasive_Crumble" && name != "Pasive_Armored")
                {
                    options.Add(name);
                }
            }
        }

        crysDropdown.AddOptions(options);
    }

    // Guardar los cambios en las cartas
    public void SaveChanges()
    {
        if (selectedCard == null) return;

        if (int.TryParse(manaInput.text, out int mana)) selectedCard.ManaValue = mana;
        if (int.TryParse(goldInput.text, out int gold)) selectedCard.GoldValue = gold;

        // Si es un personaje
        if (selectedCard is Character character)
        {
            if (int.TryParse(healthInput.text, out int health)) character.Health = health;
            if (int.TryParse(attackInput.text, out int attack)) character.Attack = attack;
            if (int.TryParse(defenseInput.text, out int defense)) character.Defense = defense;

            // Guardar valores seleccionados del dropdown
            character.Skill = (Card.SkillType)skillTypeDropdown.value;

            string cryText = crysDropdown.options[crysDropdown.value].text;
            character.Cry = (Card.Crys)System.Enum.Parse(typeof(Card.Crys), cryText);
        }

        // Si es un hechizo
        if (selectedCard is SpellCard spellCard)
        {
            spellCard.spellType = (Card.SpellType)spellTypeDropdown.value;
            spellCard.attributeTargets.Clear();
            spellCard.attributeChangeAmount.Clear();
            spellCard.attributeTargets.Add((Card.AttributeTarget)attributeTargetDropdown.value);

            if (int.TryParse(attributeAmountInput.text, out int amount))
            {
                spellCard.attributeChangeAmount.Add(amount);
            }
            else
            {
                spellCard.attributeChangeAmount.Add(0); // Default si hay error
            }
        }
// Necesario para cambiar archivos
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(selectedCard);
        UnityEditor.AssetDatabase.SaveAssets();
#endif

        Debug.Log("Cambios guardados en la carta: " + selectedCard.NameCard);

        UpdateAllCardDisplays();
    }

    // Actualiza las cartas al guardar cambios 
    private void UpdateAllCardDisplays()
    {
        // Actualiza la vista previa grande
        if (CardPreviewManager.Instance != null)
        {
            CardPreviewManager.Instance.ShowCard(selectedCard);
        }

        // Actualiza todas las miniaturas en la lista
        DisplayCard[] allCards = Object.FindObjectsByType<DisplayCard>(FindObjectsSortMode.None);

        foreach (var display in allCards)
        {
            if (display.card == selectedCard)
            {
                display.UpdateView();
            }
        }
    }
}
