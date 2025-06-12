using UnityEngine;
using UnityEngine.UI;

public class CardButtonHandler : MonoBehaviour
{
    private Card card;
    private CardEditorManager editor;
    
    // Asocia una carta y el CardEditorManager con el botón y agrega el hacer click
    public void Setup(Card assignedCard, CardEditorManager manager)
    {
        card = assignedCard;
        editor = manager;
        Debug.Log("Setup llamado con carta: " + assignedCard.NameCard);
        GetComponent<Button>().onClick.AddListener(OnClick);
        Debug.Log("Setup llamado con carta: " + assignedCard.NameCard);
    }

    // Cargar carta asociada al botón cuando se le hace click 
    void OnClick()
    {
        editor.SelectCardFromOutside(card);
    }
}
