using UnityEngine;
using static Card;

public class CharacterStats : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Character characterStartData;//setear 
    public string NameCharacter;
    public string Description;
    public int GoldValue;
    public int ManaValue;
    public Sprite sprite;
    public int Attack;
    public int Health;
    public int Defense;
    public Factions Faction;
    public SkillType Skill;
    public Crys Cry;

    private bool statsSet = false;

    // Script que guarda las estadisticas de character en un formato mas comdo para usarlas en el tooltip o al usar spells sobre un character
    public void Update()
    {
        if (!statsSet && characterStartData != null)
        {
            SetStarStats();
        }
    }
    private void SetStarStats()
    {
        // Asigna los valores de la carta a los valores de CharacterStats
        NameCharacter = characterStartData.NameCard;
        Description = characterStartData.Description;
        GoldValue = characterStartData.GoldValue;
        ManaValue = characterStartData.ManaValue;
        Attack = characterStartData.Attack;
        Health = characterStartData.Health;
        Defense = characterStartData.Defense;
        Skill = characterStartData.Skill;
        Cry = characterStartData.Cry;
        Faction = characterStartData.Faction;
        statsSet = true;
    }
}
