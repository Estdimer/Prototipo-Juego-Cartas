using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class SelectNumber : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public int SelectedNumber;

    // Para un heroe en especifico seleccionar un dropdown del 1 al 20 
    void Start()
    {
        dropdown.ClearOptions();
        List<string> numeros = new List<string>();
        for (int i = 1; i <= 20; i++)
        {
            numeros.Add(i.ToString());
        }
        dropdown.AddOptions(numeros);
        dropdown.onValueChanged.AddListener(OnSeleccion);
    }
    void OnSeleccion(int index)
    {
        SelectedNumber = index + 1;
        Debug.Log("Elegiste: " + SelectedNumber);
    }
}
