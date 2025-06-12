using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class HeroSelector : MonoBehaviour
{
    [System.Serializable]
    public class Hero
    {
        public string Name;
        public Sprite image;
        public string description;
    }

    public Hero[] heroes;
    public GameObject LuckyNumber;
    private Hero HeroTransfer = null;
    public GameObject heroButtonPrefab;
    public Transform buttonContainer;

    public Image heroImageDisplay;
    public TextMeshProUGUI heroNameDisplay;
    public TextMeshProUGUI heroDescriptionDisplay;

    public Button chooseButton; // Referencia al botón "Elegir"

    private GameObject currentSelectedButton = null;
    private Hero selectedHero = null;


    void Start()
    {
        // Evitar errores
        PlayerPrefs.SetString("", "");
        if (heroes == null || heroes.Length == 0 || heroButtonPrefab == null || buttonContainer == null)
        {
            Debug.LogError("Faltan referencias en el Inspector.");
            return;
        }

        // Instancia botón para cada héroe
        for (int i = 0; i < heroes.Length; i++)
        {
            Hero hero = heroes[i];
            GameObject btn = Instantiate(heroButtonPrefab, buttonContainer);

            var nameObj = btn.transform.Find("HeroName");
            var portraitObj = btn.transform.Find("Portrait");
            var borderObj = btn.transform.Find("SelectionBorder");

            if (nameObj == null || portraitObj == null || borderObj == null)
            {
                Debug.LogError("HeroButton prefab está incompleto.");
                continue;
            }

            var nameText = nameObj.GetComponent<TextMeshProUGUI>();
            var imageComp = portraitObj.GetComponent<Image>();
            var borderImage = borderObj.gameObject;

            nameText.text = hero.Name;
            imageComp.sprite = hero.image;
            borderImage.SetActive(false);

            // Captura variables locales para el closure del lambda
            GameObject buttonCopy = btn;
            GameObject borderCopy = borderImage;
            Hero heroCopy = hero;

            // Asignar eventos de click a cada botón 
            btn.GetComponent<Button>().onClick.AddListener(() =>
            {
                OnHeroSelected(heroCopy, buttonCopy, borderCopy);
            });

            // Seleccionar el primer héroe por defecto
            if (i == 0)
                OnHeroSelected(hero, btn, borderImage);
        }
        LuckyNumber.SetActive(false);

        if (chooseButton != null)
        {
            chooseButton.onClick.AddListener(OnChooseClicked);
        }
    }

    // Solo se activa con un Heróe en especial 
    void Update()
    {
        if (selectedHero != null && selectedHero.Name == "San Patric")
        {
            LuckyNumber.SetActive(true);
        }
        else
        {
            LuckyNumber.SetActive(false);
        }
    }

    // Al seleccionar heroe muestra despliega los datos necesarios 
    void OnHeroSelected(Hero hero, GameObject clickedButton, GameObject borderImage)
    {
        heroImageDisplay.sprite = hero.image;
        heroNameDisplay.text = hero.Name;
        heroDescriptionDisplay.text = hero.description;

        if (currentSelectedButton != null)
        {
            var oldBorder = currentSelectedButton.transform.Find("SelectionBorder");
            if (oldBorder != null) oldBorder.gameObject.SetActive(false);
        }

        borderImage.SetActive(true);
        currentSelectedButton = clickedButton;
        selectedHero = hero;
    }

    // Cargar datos al hacer click en el botón
    void OnChooseClicked()
    {
        if (selectedHero != null)
        {
            HeroTransfer = selectedHero;
            Debug.Log("Héroe elegido: " + selectedHero.Name);
            PlayerPrefs.SetString("SelectedHero", HeroTransfer.Name);
            if (HeroTransfer.Name == "San Patric")
            {
                PlayerPrefs.SetInt("LuckyNumber", GetComponent<SelectNumber>().SelectedNumber);

            }
            SceneManager.LoadSceneAsync(2);
        }
        else
        {
            Debug.LogWarning("No hay héroe seleccionado.");
        }
    }

}
