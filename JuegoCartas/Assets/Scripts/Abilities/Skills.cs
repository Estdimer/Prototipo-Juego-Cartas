using System;
using System.Collections.Generic;
using UnityEngine;

// Este script sirve para activar las habilidades que corresponden 
// Estas habilidades se diferencian al momento de activarse
// BattleCry al ser jugada la carta
// Lastbreath al ser destruidad
// Pasive al cumplirse cierta condicion
// Active al hacerles click a los charactes durante la fase de administracion
public class Skills : MonoBehaviour
{

    // Esta funcion recibe los datos de la carta, usa un switch para diferenciar que tipo de habilidad tiene y envia los datos a las funciones que corresponden a cada habilidad 
    public static void Trigger(DisplayCard character, CharacterStats stats)
    {
        switch (character.Skill)
        {
            case Card.SkillType.battlecry:
                if (stats.Cry == Card.Crys.Search)
                {
                    BattleCry.Search();
                }
                else
                {
                    BattleCry.battlecry(stats);
                }
                break;
            case Card.SkillType.lastBreath:
                if (stats.Health == 0)
                {
                    if (stats.Cry == Card.Crys.Return_LastBreath)
                    {
                        LastBreath.Return(stats);
                    }
                    if (stats.Cry == Card.Crys.DrawOne)
                    {
                        LastBreath.DrawOne();
                    }
                }
                break;
            case Card.SkillType.pasive:
                if (stats.Cry == Card.Crys.Pasive_Crumble)
                {
                    Pasive.Crumble(character, stats);
                }
                if (stats.Cry == Card.Crys.Pasive_Armored)
                {
                    Pasive.Armored();
                }
                break;
            case Card.SkillType.active:
                if (stats.Cry == Card.Crys.Predic)
                {
                    Active.Predic();
                }
                if (stats.Cry == Card.Crys.DrawOne)
                {
                    Active.DrawOne();
                }
                break;
        }
    }
}