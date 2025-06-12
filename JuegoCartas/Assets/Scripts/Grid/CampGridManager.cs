using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CampGridManager : MonoBehaviour
{
    public int width = 1;
    public int height = 2;
    public GameObject gridCampPrefab;
    public List<GameObject> gridObject = new List<GameObject>();
    public Vector2 cellSize = new Vector2(1.30f, 1.80f);
    public GameObject[,] gridCells;
    public Vector2 gridOffset = new Vector2(1f, 1f);
    void Start()
    {
        CreateGrid();
    }
    void CreateGrid()
    {
        // Crea una grid solo para jefes que guarda los jefes derrotados 
        gridCells = new GameObject[width, height];
        Vector2 centerOffset = new Vector2((width * cellSize.x) / 2.0f - cellSize.x / 2.0f, (height * cellSize.y) / 2.0f - cellSize.y / 2.0f);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 gridPosition = new Vector2(x * cellSize.x, y * cellSize.y);
                Vector3 spawnPosition = new Vector3(gridPosition.x - centerOffset.x + gridOffset.x, gridPosition.y - centerOffset.y + gridOffset.y, 1f);

                GameObject gridCell = Instantiate(gridCampPrefab, spawnPosition, Quaternion.identity);
                gridCell.transform.SetParent(this.transform);

                // Asigna un nombre único a cada celda en la jerarquía
                gridCell.name = $"Grid Cell ({x}, {y})";

                gridCell.GetComponent<GridCell>().gridIndex = new Vector2(x, y);
                gridCells[x, y] = gridCell;

                gridCell.transform.localScale = new Vector3(cellSize.x, cellSize.y, 1f);
            }
        }
    }
    public void AddJefeToGrid(Boss obj)
    {
        // Agrega el objeto recibido a una celda
        bool placed = false;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GridCell cell = gridCells[x,y].GetComponent<GridCell>();
                if (!cell.cellFull && !placed)
                {
                    GameObject newObj = Instantiate(obj.BossPrefab, cell.GetComponent<Transform>().position, quaternion.identity);
                    newObj.GetComponent<DisplayBoss>().boss = obj;
                    newObj.transform.SetParent(transform);
                    gridObject.Add(newObj);
                    cell.objectInCell = newObj;
                    cell.cellFull = true;
                    placed = true;
                }
            }
        }
    }

}
