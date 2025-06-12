using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public int PlayerGold { get; set; } = 3;
    public int PlayerMana { get; set; } = 3;
    public OptionsMannager OptionsMannager { get; private set; }
    public AudioMannager AudioMannager { get; private set; }
    public DeckManager DeckManager { get; private set; }
    public bool PlayingCard { get; set; } = false;
    public Card CurrentCard { get; set; }
    public Boss SelectedBoss { get; set; }
    public bool BossFighting { get; set; }

    public enum GamePhase
    {
        Shop,
        Administration,
        Figth
    }

    public GamePhase CurrentPhase { get; private set; }


    // Asegurarse que solo exixta uno (singleton) para ser sacar datos desde otras partes
    private void Awake()
    {
        if (Instance == null) // Evitar que se destrulla cuando se cambia de escena 
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeManagers();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Asegurarse que los managers se carguen 
    private void InitializeManagers() 
    {
        OptionsMannager = GetComponentInChildren<OptionsMannager>();
        if (OptionsMannager == null)
        {
            GameObject prefab = Resources.Load<GameObject>("Prefabs/OptionsManager");
            if (prefab != null)
            {
                Instantiate(prefab, transform.position, quaternion.identity, transform);
                OptionsMannager = GetComponentInChildren<OptionsMannager>();
            }
            else
            {
                Debug.Log("OptionsManager prefab not found");
            }
        }

        AudioMannager = GetComponentInChildren<AudioMannager>();
        if (AudioMannager == null)
        {
            GameObject prefab = Resources.Load<GameObject>("Prefabs/AudioManager");
            if (prefab != null)
            {
                Instantiate(prefab, transform.position, quaternion.identity, transform);
                AudioMannager = GetComponentInChildren<AudioMannager>();
            }
            else
            {
                Debug.Log("AudioManager prefab not found");
            }
        }

        HandleDeckManager(SceneManager.GetActiveScene().name);
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        HandleDeckManager(scene.name);
    }

    // Crea o destrulle el DeckManager Dependiendo si se esta en la escana de combate
    private void HandleDeckManager(string sceneName)
    {
        if (sceneName == "Combatscene")
        {
            if (DeckManager == null)
            {
                GameObject prefab = Resources.Load<GameObject>("Prefabs/DeckManager");
                if (prefab != null)
                {
                    Instantiate(prefab, transform.position, quaternion.identity, transform);
                    DeckManager = GetComponentInChildren<DeckManager>();
                }
                else
                {
                    Debug.Log("DeckManager prefab not found");
                }
            }
        }
        else
        {
            if (DeckManager != null)
            {
                Destroy(DeckManager.gameObject);
                DeckManager = null;
            }
        }
    }

    // Manejar las fases del juego 
    public void SetPhase(GamePhase newPhase)
    {
        CurrentPhase = newPhase;
        switch (newPhase)
        {
            case GamePhase.Shop:
                // Habilita tienda, desactiva cartas de pelea
                break;
            case GamePhase.Administration:
                // Habilita upgrades, organización, etc.
                break;
            case GamePhase.Figth:
                // Habilita interacción con cartas y combate
                break;
        }

        Debug.Log("Fase actual: " + CurrentPhase);
    }

}
