using System;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D.IK;

public class GridManager : MonoBehaviour
{
    public int width = 6;
    public int height = 3;
    public GameObject gridCellPrefab;
    public List<GameObject> gridObjects = new List<GameObject>();
    public GameObject[,] gridCells; //2d array
    public Vector2 cellSize = new Vector2(1.30f, 1.80f);
    public Vector2 gridOffset = new Vector2(0f, 0f);
    public TextMeshProUGUI ContAttack;
    Discard discard;
    public int NumberOfCharacters = 0;

    void Start()
    {
        CreateGrid();
    }
    void Awake()
    {
        discard = FindAnyObjectByType<Discard>();
    }

    void CreateGrid()
    {
        // Crea grid para almacenar character
        gridCells = new GameObject[width, height];
        Vector2 centerOffset = new Vector2((width * cellSize.x) / 2.0f - cellSize.x / 2.0f, (height * cellSize.y) / 2.0f - cellSize.y / 2.0f); // cambio: Ajustado para considerar el tamaño de celda

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 gridPosition = new Vector2(x * cellSize.x, y * cellSize.y); // cambio: Aplicado el tamaño de celda al cálculo de posición
                Vector3 spawnPosition = new Vector3(gridPosition.x - centerOffset.x + gridOffset.x, gridPosition.y - centerOffset.y + gridOffset.y, 1f); // cambio: Se agrega el offset para mover la cuadrícula

                GameObject gridCell = Instantiate(gridCellPrefab, spawnPosition, Quaternion.identity);
                gridCell.transform.SetParent(this.transform);

                // Asigna un nombre único a cada celda en la jerarquía
                gridCell.name = $"Grid Cell ({x}, {y})";

                gridCell.GetComponent<GridCell>().gridIndex = new Vector2(x, y);
                gridCells[x, y] = gridCell;

                gridCell.transform.localScale = new Vector3(cellSize.x, cellSize.y, 1f); // cambio: Ajustar escala de la celda según cellSize
            }
        }
    }

    public bool AddObjectToGrid(GameObject obj, Vector2 gridPosition)
    {
        // Agrega un objeto a la celda 
        if (gridPosition.x >= 0 && gridPosition.x < width && gridPosition.y >= 0 && gridPosition.y < height) // cambio: Corregido el límite superior de gridPosition.x
        {

            GridCell cell = gridCells[(int)gridPosition.x, (int)gridPosition.y].GetComponent<GridCell>();

            if (cell.cellFull) return false;
            else
            {
                GameObject newObj = Instantiate(obj, cell.GetComponent<Transform>().position, quaternion.identity);
                newObj.transform.SetParent(transform);

                // Ajustar la escala solo en el grid
                newObj.transform.localScale = new Vector3(0.5f, 0.5f, 1f);

                // Cartas en grid Layer Characters 
                newObj.layer = LayerMask.NameToLayer("Characters");
                newObj.name = obj.GetComponent<DisplayCard>().card.name;
                CardMovement cardMovement = newObj.GetComponent<CardMovement>();
                if (cardMovement != null)
                {
                    cardMovement.enabled = false;
                }
                gridObjects.Add(newObj);
                cell.objectInCell = newObj;
                newObj.GetComponent<DisplayCard>().UpdateView();
                cell.cellFull = true;
                NumberOfCharacters += 1;
                return true;
            }
        }
        else return false;
    }
    public void RemoveObject(GameObject character)
    {
        // Elimina un objeto de la Grid
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GridCell cell = gridCells[x, y].GetComponent<GridCell>();
                if (cell.objectInCell != null)
                {
                    if (cell.objectInCell.GetComponent<DisplayCard>().card == character.GetComponent<DisplayCard>().card)
                    {
                        gridObjects.Remove(cell.objectInCell);
                        Destroy(cell.objectInCell);
                        cell.objectInCell = null;
                        cell.cellFull = false;
                        NumberOfCharacters -= 1;
                        return;
                    }
                }
            }
        }
    }
    public void removeDeads()
    {
        // Busca y remueve todo los character que tengan vida 0 en el grid de characters
        int convert;
        GameObject card;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GridCell cell = gridCells[x, y].GetComponent<GridCell>();
                if (cell.objectInCell != null && cell.objectInCell.GetComponent<DisplayCard>() != null)
                {
                    convert = cell.objectInCell.GetComponent<CharacterStats>().Health;

                    if (convert == 0)
                    {
                        card = cell.objectInCell;
                        discard.AddToDiscard(cell.objectInCell.GetComponent<DisplayCard>().card);
                        if (card.GetComponent<DisplayCard>().Skill == Card.SkillType.lastBreath)
                        {
                            Skills.Trigger(card.GetComponent<DisplayCard>(), card.GetComponent<CharacterStats>());
                        }
                        gridObjects.Remove(cell.objectInCell);
                        Destroy(cell.objectInCell);
                        cell.objectInCell = null;
                        cell.cellFull = false;
                        NumberOfCharacters -= 1;
                    }
                }
            }
        }
    }
    public void defeat(DisplayCard card)
    {
        // Aplica el daño a los characters que tenian menos defensa que el ataque del jefe
        int convert = 0;
        int damage = 1;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GridCell cell = gridCells[x, y].GetComponent<GridCell>();
                if (cell.objectInCell != null && cell.objectInCell.GetComponent<DisplayCard>() != null && cell.objectInCell.GetComponent<DisplayCard>() == card)
                {
                    convert = int.Parse(cell.objectInCell.GetComponent<DisplayCard>().HealthText.text);
                    convert -= damage;
                    convert = Mathf.Max(convert, 0);
                    cell.objectInCell.GetComponent<DisplayCard>().HealthText.text = convert.ToString();
                    cell.objectInCell.GetComponent<CharacterStats>().Health = int.Parse(cell.objectInCell.GetComponent<DisplayCard>().HealthText.text);
                }
            }
        }
        removeDeads();
    }
    public void SearchActive()
    {
        // Busca characters dentro de la grid que tenga habilidad activa y luego activa el highlighter y la variable used
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GridCell cell = gridCells[x, y].GetComponent<GridCell>();
                if (cell.objectInCell != null && cell.objectInCell.GetComponent<DisplayCard>() != null)
                {
                    if (cell.objectInCell.GetComponent<DisplayCard>().Skill == Card.SkillType.active)
                    {
                        if (GameManager.Instance.CurrentPhase == GameManager.GamePhase.Administration)
                        {
                            //Skills.Trigger(cell.objectInCell.GetComponent<DisplayCard>(), cell.objectInCell.GetComponent<CharacterStats>());
                            cell.objectInCell.GetComponent<SkillHighlighter>().turnOn();
                        }
                        else
                        {
                            //Skills.Trigger(cell.objectInCell.GetComponent<DisplayCard>(), cell.objectInCell.GetComponent<CharacterStats>());
                            cell.objectInCell.GetComponent<SkillHighlighter>().turnOff();
                        }
                    }

                }
            }
        }
    }
    public void Crumble(DisplayCard card)
    {
        // Busca al character recibido y activa la habilidad crumble, luego remueve los hcaracters con 0 de vida 
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GridCell cell = gridCells[x, y].GetComponent<GridCell>();
                if (cell.objectInCell != null && cell.objectInCell.GetComponent<DisplayCard>() != null)
                {
                    if (cell.objectInCell.GetComponent<DisplayCard>() == card )
                    {
                        Skills.Trigger(cell.objectInCell.GetComponent<DisplayCard>(), cell.objectInCell.GetComponent<CharacterStats>());
                        removeDeads();
                    }
                }
            }
        }
    }
    public void EverythingOff()
    {
        // Busca y desactiva todas los Highlighters y deja en false las variables used de SkillHighlighter 
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GridCell cell = gridCells[x, y].GetComponent<GridCell>();
                if (cell.objectInCell != null && cell.objectInCell.GetComponent<DisplayCard>() != null)
                {
                    cell.objectInCell.GetComponent<SkillHighlighter>().turnOff();
                }
            }
        }
    }
    public List<GameObject> GetCurrentGridObjects()
    {
        return new List<GameObject>(gridObjects);
    }


}