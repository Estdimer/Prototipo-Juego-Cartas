using TMPro;
//using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;
[CreateAssetMenu(fileName = "Nueva Boss", menuName = "Boss", order = 0)]
public class Boss : ScriptableObject
{
    public int Dificulty;
    public int Damage;
    public string NameText;
    public string DescriptionText;
    public string DificultyText;
    public string DamageText;
    public Sprite Art;
    public GameObject BossPrefab;
    public enum Value
    {
        diff1,
        diff2,
        diff3,
        diff4
    }
    public Value value;
}
