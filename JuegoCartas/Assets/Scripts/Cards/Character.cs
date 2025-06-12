using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//minions o mercenarios(?)
[CreateAssetMenu(fileName = "Nueva Character Carta", menuName = "Carta/Character", order = 0)]
public class Character : Card
{
    public int Attack;
    public int Health;
    public int Defense; 
    public Factions Faction;
    public SkillType Skill;
    public Crys Cry;

}