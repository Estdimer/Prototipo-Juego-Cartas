using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class GridBossManager : MonoBehaviour
{
    public int width = 2;
    public int height = 2;
    public GameObject gridJefePrefab;
    public List<GameObject> gridObject = new List<GameObject>();
    public Vector2 cellSize = new Vector2(1.30f, 1.80f);
    public GameObject[,] gridCells;
    public string resourcePath = "Jefes";
    public List<Boss> Bosses = new List<Boss>();
    public List<Boss> BossesList = new List<Boss>();
    public List<Boss> usedJefes = new List<Boss>();
    public Vector2 gridOffset = new Vector2(1f, 1f);
    void Start()
    {
        CreateGrid();
        LoadBoss();
        PopulateGridWithRandomBosses();
    }
    void CreateGrid()
    {
        // Crea la grid para almacenar jefes
        gridCells = new GameObject[width, height];
        float totalHeight = height * cellSize.y;
        float startY = -(totalHeight / 2f) + (cellSize.y / 2f);

        for (int y = 0; y < height; y++)
        {
            int cellsInRow = height - y;
            float rowWidth = cellsInRow * cellSize.x;
            float startX = -(rowWidth / 2f) + (cellSize.x / 2f);

            for (int x = 0; x < cellsInRow; x++)
            {
                Vector2 position = new Vector2(
                    startX + x * cellSize.x + gridOffset.x,
                    startY + y * cellSize.y + gridOffset.y
                );

                GameObject gridCell = Instantiate(gridJefePrefab, new Vector3(position.x, position.y, 1f), Quaternion.identity);
                gridCell.transform.SetParent(this.transform);
                gridCell.GetComponent<GridCell>().gridIndex = new Vector2(x, y);
                gridCells[x, y] = gridCell;

                gridCell.transform.localScale = new Vector3(cellSize.x, cellSize.y, 1f);
            }
        }
    }
    void LoadBoss()
    {
        // Carga los jefes de la lista de resources 
        Boss[] loadedBosses = Resources.LoadAll<Boss>(resourcePath);
        if (loadedBosses.Length > 0)
        {
            for (int i = 0; i < loadedBosses.Length; i++)
            {
                Bosses.Add(loadedBosses[i]);
            }
        }
        else
        {
            Debug.LogWarning("No se encontraron Jefes en " + resourcePath);
        }
    }
    void PopulateGridWithRandomBosses()
    {
        // Asigna jefes a las casillas segun la categoria de estos 
        if (Bosses.Count == 0)
        {
            Debug.LogWarning("No hay Jefes disponibles");
            return;
        }

        for (int y = 0; y < height; y++)
        {
            // Convierte y en enum: Boss.Clase
            Boss.Value ChoosedValue = (Boss.Value)y;

            // Filtra lista por clase deseada
            List<Boss> BossesPerRow = Bosses.FindAll(b => b.value == ChoosedValue && !usedJefes.Contains(b));

            for (int x = 0; x < width; x++)
            {
                if (BossesPerRow.Count == 0)
                {
                    Debug.LogWarning($"No hay más jefes con clase {ChoosedValue} para la fila {y}");
                    continue;
                }

                int randomIndex = UnityEngine.Random.Range(0, BossesPerRow.Count);
                Boss selectedBoss = BossesPerRow[randomIndex];
                BossesPerRow.RemoveAt(randomIndex);
                usedJefes.Add(selectedBoss);

                if (gridCells[x, y] != null)
                {
                    AddJefeToGrid(selectedBoss, new Vector2(x, y));
                }
            }
        }
    }

    public bool AddJefeToGrid(Boss obj, Vector2 gridPosition)
    {
        // Agrega un jefe a la grid 
        if (gridPosition.x >= 0 && gridPosition.x < width && gridPosition.y < height && gridPosition.y >= 0)
        {
            GridCell cell = gridCells[(int)gridPosition.x, (int)gridPosition.y].GetComponent<GridCell>();
            if (cell.cellFull) return false;
            else
            {
                GameObject newObj = Instantiate(obj.BossPrefab, cell.GetComponent<Transform>().position, quaternion.identity);
                newObj.GetComponent<DisplayBoss>().boss = obj;
                newObj.transform.SetParent(transform);
                gridObject.Add(newObj);
                cell.objectInCell = newObj;
                cell.cellFull = true;
                return true;
            }
        }
        else return false;
    }
    public void RemoverJefe(string nombre)
    {
        // Remueve un jefe de una casilla de la grid
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (gridCells[x, y] && gridCells[x, y].GetComponent<GridCell>() != null)
                {
                    GridCell cell = gridCells[x, y].GetComponent<GridCell>();
                    if (cell.objectInCell != null && cell.objectInCell.GetComponent<DisplayBoss>() != null)
                    {
                        if (cell.objectInCell.GetComponent<DisplayBoss>().NameText.text == nombre)
                        {
                            gridObject.Remove(cell.objectInCell);
                            Destroy(cell.objectInCell);
                            cell.objectInCell = null;
                            cell.cellFull = false;
                        }

                    }
                }

            }
        }
    }

}
