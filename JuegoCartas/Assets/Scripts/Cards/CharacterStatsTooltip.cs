using TMPro;
using UnityEngine;

public class CharacterStatsTooltip : MonoBehaviour
{
   public TextMeshProUGUI Name;
   public TextMeshProUGUI Description;
   //public TextMeshProUGUI oro;
   //public TextMeshProUGUI mana;
   public TextMeshProUGUI Attack;
   public TextMeshProUGUI Health;
   public TextMeshProUGUI Defense;
   //public TextMeshProUGUI tipo;
   private RectTransform rectTransform;
   public CanvasGroup canvasGroup;
   [SerializeField] private float lerpFactor = 0.1f;
   [SerializeField] private float xOffset=200f;
   private Canvas canvas;
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        
        if(canvasGroup==null)
        {
            canvasGroup = GetComponentInChildren<CanvasGroup>();
            if(canvasGroup==null)
            {
                Debug.Log("error canvas");
            }
        }
        canvas = GetComponentInParent<Canvas>();
    }
    void Update()
    {
        // Asigna la ubicacion del tooltip cerca del mouse 
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, canvas.worldCamera,out Vector2 pos);
        rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, new Vector2(pos.x + xOffset, pos.y), lerpFactor);
    }
    public void SetStatsText(CharacterStats stats,DisplayCard displayCard)
    {
        // Asigna los valores de CharacterStats a los elementos UI del Tooltip
        Name.text = $"{stats.NameCharacter}";
        Description.text = $"{displayCard.DescriptionText.text.ToString()}";
        Attack.text = $"{stats.Attack} ";
        Health.text = $"{stats.Health}";
        Defense.text = $"{stats.Defense}";
    }
}
