using UnityEngine;

[CreateAssetMenu(fileName = "Nueva Hero", menuName = "Hero", order = 0)]
public class Hero : ScriptableObject
{
    public string Name;
    public Sprite image;
    public string description;
}
