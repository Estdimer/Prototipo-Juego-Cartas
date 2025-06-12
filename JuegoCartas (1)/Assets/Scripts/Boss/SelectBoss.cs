using UnityEngine;
public class SelectBoss : MonoBehaviour
{
    private LayerMask bossLayer;
    private Boss bossSelected;
    private DisplayBoss displayBoss;
    private GridBossManager gridJefeManager;
    private BotonBatalla botonBatalla;
    void Awake()
    {
        bossLayer = LayerMask.GetMask("Jefes");
        displayBoss = GetComponent<DisplayBoss>();
        gridJefeManager = FindFirstObjectByType<GridBossManager>();
        botonBatalla = FindFirstObjectByType<BotonBatalla>();

    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Clic izquierdo
        {
            Select();
        }
    }
    void Select()
    {
        // Al hacer click en un jefe luego de unas verificaciones guarda una copia en GameManager 
        if (GameManager.Instance.CurrentPhase != GameManager.GamePhase.Figth)
            return;

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, bossLayer);
        if (hit.collider != null && GameManager.Instance.CurrentPhase == GameManager.GamePhase.Figth)
        {
            DisplayBoss mostrarJefe = hit.collider.GetComponent<DisplayBoss>();
            if (mostrarJefe != null)
            {
                if (mostrarJefe.boss.value == Boss.Value.diff1)
                {
                    // Dependiendo de la dificultad, si es nivel 1 se puede seleccionar sin problema 
                    // si es nivel 2 deben estar todos los niveles 1 derrotados antes
                    bossSelected = mostrarJefe.boss;
                    GameManager.Instance.SelectedBoss = bossSelected;
                    botonBatalla.updateNumber();
                    MessageManager.Instance.ShowMessage("Selecionaste a: " + bossSelected.NameText.ToString() + "\n maximo de peleadores 2",4);
                }
                if (mostrarJefe.boss.value == Boss.Value.diff2 && gridJefeManager.gridCells[0, 0].GetComponent<GridCell>().objectInCell == null && gridJefeManager.gridCells[1, 0].GetComponent<GridCell>().objectInCell == null)
                {
                    bossSelected = mostrarJefe.boss;
                    GameManager.Instance.SelectedBoss = bossSelected;
                    botonBatalla.updateNumber();
                    MessageManager.Instance.ShowMessage("Selecionaste a: " + bossSelected.NameText.ToString() + "\n maximo de peleadores 3",4);
                }
                if (mostrarJefe.boss.value == Boss.Value.diff2 && gridJefeManager.gridCells[0, 0].GetComponent<GridCell>().objectInCell != null && gridJefeManager.gridCells[1, 0].GetComponent<GridCell>().objectInCell != null)
                {
                    MessageManager.Instance.ShowMessage("Debes derrotar a los de primer nivel antes!");
                }
            }
        }
    }


    public Boss GetJefeSeleccionado()
    {
        // Retorna el ultimo jefe seleccionado  
        return bossSelected;
    }
}

