using UnityEngine;


public class Card : ScriptableObject
{
    public string NameCard;
    public string Description;
    public int GoldValue;
    public int ManaValue;
    public Sprite SpriteCard;
    public GameObject prefab;

    public enum TypeCard
    {
        Mercenario,
        Objeto,
        Ritual,
        RitualPermanente,
        Sabotaje
    }

    // Variable pública para seleccionar el tipo de carta.
    public TypeCard typeCard;
    public enum SpellType
    {
        buff,
        debuff,
    }

    public enum AttributeTarget
    {
        Attack,
        Health,
        Defense
    }
    public enum SkillType
    {
        battlecry,
        lastBreath,
        pasive,
        active,
        none
    }
    public enum Factions
    {
        Polar,
        demon,
        soldier
    }
    public enum Crys
    {
        DrawOne,
        Health,
        Defense,
        Return_LastBreath,
        Pasive_Crumble,
        Revive,
        Pasive_Armored,
        Search,
        Predic
    }
}