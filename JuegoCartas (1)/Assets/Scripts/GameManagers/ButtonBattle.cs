using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using System.Linq;
using TMPro;
using Unity.VisualScripting.AssemblyQualifiedNameParser;
using UnityEngine.SceneManagement;
public class BotonBatalla : MonoBehaviour
{
    private Boss BossDificulty;
    private GridManager gridManager;
    private GridBossManager gridBossManager;
    private CampGridManager gridCampManager;
    private TurnManager turnManager;
    private LayerMask CharacterLayerMask;
    private List<List<DisplayCard>> Fighters = new(); // Lista de characters que seran elegidos para pelear con el jefe
    private int NumbersOfFigthers;
    private int currentFighters = 0;
    private int MaxNumberOfFighters = 5;
    public int AttackSum = 0;
    public bool passTurn { get; set; } = true;
    // Este scrip se encarga de crear una lista de characters que llamaremos peleadores para luego enfrentarlos con el jefe seleccionado
    // El resultado de la batalla se decide con el lanzamiento de un dado de 20 caras, mediante un generador randon de numeros del 1 al 20

    void Awake()
    {
        // LLama variables necesarias
        gridManager = FindAnyObjectByType<GridManager>();
        gridBossManager = FindAnyObjectByType<GridBossManager>();
        gridCampManager = FindAnyObjectByType<CampGridManager>();
        turnManager = FindAnyObjectByType<TurnManager>();
        CharacterLayerMask = LayerMask.GetMask("Characters");
        InitFighters(MaxNumberOfFighters);
    }

    void Update()
    {

        // Al hacer click inicia la funcion choose
        if (Input.GetMouseButtonDown(0))
        {
            Choose();
        }
        // Actualiza el valor de la suma de ataque bajo el boton de batalla
        gridManager.ContAttack.text = AttackSum.ToString();
    }
    public void Battle()
    {
        // Comprueba si existe un jefe seleccionado, si la fase es la de batalla y si los peladores no exeden el numero de peleadores que permite el jefe 
        if (GameManager.Instance.SelectedBoss != null && GameManager.Instance.CurrentPhase == GameManager.GamePhase.Figth && currentFighters <= NumbersOfFigthers)
        {
            // Llama el resultado del dado de 20 caras y el de 6 
            // Luego compara suma el ataque de los peleadores de la lista al d20 y compara con la dificultad del jefe 
            // Si es mayor o igual el jefe es removido del grid de jefes y se añade al grid campamento
            // En caso de ser menor solo envia el mensaje de fallo
            // Luego llama la funcion de CounterAttack para ver si los character de la grid recibieron daño 
            // Activa las habilidades de crumble de los peladores 
            // Por ultimo limpia la lista y al siguiente turno  
            int dice = Utility.Diceroll();
            int counterDice;
            passTurn = false;
            dice += AttackSum;
            BossDificulty = GameManager.Instance.SelectedBoss;
            counterDice = Utility.CounterDice();
            if (dice >= BossDificulty.Dificulty)
            {
                MessageManager.Instance.ShowMessage("Con un " + dice + " Ganaste! La dificultad era :" + BossDificulty.Dificulty + "\n" + "El Enemego contraatacó con un :" + counterDice + "\nEntrando a Fase de Compra", 8);
                gridBossManager.RemoverJefe(BossDificulty.name);
                gridCampManager.AddJefeToGrid(BossDificulty);
                if (GameManager.Instance.SelectedBoss.value == Boss.Value.diff2)
                {
                    StartCoroutine(BackToMenu());
                }
                GameManager.Instance.SelectedBoss = null;
            }
            else
            {
                MessageManager.Instance.ShowMessage("Con un " + dice + " Perdiste! La dificultad era :" + BossDificulty.Dificulty + "\n" + " El Enemego contraatacó con un :" + counterDice + "\n Entrando a Fase de Compra ", 8);
                GameManager.Instance.SelectedBoss = null;
            }

            CounterAttack(counterDice);
            Crumble();
            currentFighters = 0;
            ClearFighters();
            turnManager.NextTurn();
        }
    }
    void InitFighters(int numberOfGroups)
    {
        // Crea la lista donde se guardaron los characters elegidos para la pelea con el jefe
        Fighters.Clear();
        for (int i = 0; i < numberOfGroups; i++)
        {
            Fighters.Add(new List<DisplayCard>());
        }
    }

    bool SearchFighter(DisplayCard card, out int groupIndex)
    {
        // Recorre la lista de grupos buscando si es que ya se encuentra un character 
        // En caso de encontrarlo o no devuleve un boolean
        for (int i = 0; i < Fighters.Count; i++)
        {
            if (Fighters[i].Contains(card))
            {
                groupIndex = i;
                return true;
            }
        }
        groupIndex = -1;
        return false;
    }

    void AddFighter(DisplayCard card)
    {
        // Recorre la lista de peleadores y agrega el recibido a un espacio vacio
        for (int i = 0; i < Fighters.Count; i++)
        {
            if (Fighters[i].Count < MaxNumberOfFighters)
            {
                Fighters[i].Add(card);
                Debug.Log("added");
                currentFighters += 1;
                AttackSum += int.Parse(card.AttackText.text);
                return;
            }
        }
    }

    void RemoveFighter(DisplayCard card)
    {
        // Recorre la lista en busca de un peleador recibido y luego lo quita de la lista en caso de encontrarlo 
        if (SearchFighter(card, out int group))
        {
            Fighters[group].Remove(card);
            //Debug.Log("removed");
            currentFighters -= 1;
            AttackSum -= int.Parse(card.AttackText.text);
        }
    }

    public void Choose()
    {
        // Funcion de llenado de la lista
        // Si al hacer click el raycast choca con un character en el grid de Characters entra al if
        Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mouse, Vector2.zero, Mathf.Infinity, CharacterLayerMask);

        if (hit.collider != null && GameManager.Instance.CurrentPhase == GameManager.GamePhase.Figth)
        {
            if (GameManager.Instance.SelectedBoss != null)
            {
                // Consigue los elementos DisplayCard y CharacterStats del character en el grid de Characters y crea copias
                var display = hit.collider.GetComponent<DisplayCard>();
                var character = hit.collider.GetComponent<CharacterStats>();
                if (display != null && character != null)
                {
                    // Con SearchFighter revisa si es que ya esta dentro de la lista, si no esta lo agrega a la lista llamando AddFighter y activa el highlighter del character
                    // Si ya se encuentra en la lista lo elimina usando RemoveFighter y desactiva el highlighter de character
                    if (!SearchFighter(display, out _))
                    {
                        AddFighter(display);
                        hit.collider.GetComponent<SkillHighlighter>().turnOn();
                    }
                    else
                    {
                        RemoveFighter(display);
                        hit.collider.GetComponent<SkillHighlighter>().turnOff();
                    }
                }
            }
            else
            {
                if (hit.collider.GetComponent<DisplayCard>())
                {
                    MessageManager.Instance.ShowMessage("Aun no eliges un Enemigo!", 3);
                }
            }
        }
    }

    public void updateNumber()
    {
        // Actualiza la cantidad de peleadores maximos por jefe 
        NumbersOfFigthers = (int)GameManager.Instance.SelectedBoss.value + 2;
        //Debug.Log(NumbersOfFigthers);
    }
    public void ClearFighters()
    {
        // Vacia la lista de peleadores y vuelve a 0 la cantidad de peleadores seleccionados y la suma de ataque de los peleadores
        foreach (var group in Fighters)
        {
            group.Clear();
        }
        currentFighters = 0;
        AttackSum = 0;
    }

    private void Crumble()
    {
        // Habilidad pasiva de algunas carta que luego de pelear mueren
        // Revisa la lista de peleadores para ver si alguno posee la pasiva y luego los envia a gridmanager para actualizar la informacion del character dentro del grid
        foreach (var group in Fighters)
        {
            foreach (var card in group)
            {
                if (card.Cry == Card.Crys.Pasive_Crumble)
                {
                    gridManager.Crumble(card);
                }
            }
        }
    }
    private void CounterAttack(int dice)
    {
        // Luego de la batallar con un jefe se lanza un dado de 6 caras que decide si los peleadores que aprticiparon recibieron daño
        // Si el resultado es mayor o igual a la defensa de los peleadores estos reciben 1 de daño
        foreach (var group in Fighters)
        {
            foreach (var card in group)
            {
                //Debug.Log("defensa: " + card.DefenseText.text);
                //Debug.Log("dice: " + dice);
                if (int.Parse(card.DefenseText.text) <= dice)
                {
                    gridManager.defeat(card);
                }
            }
        }
    }
    private IEnumerator BackToMenu()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(0);
    }
}
