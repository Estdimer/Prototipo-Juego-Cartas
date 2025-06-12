using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

// hechizos
[CreateAssetMenu(fileName = "Nueva Spell Carta", menuName = "Carta/Spell", order = 0)]
public class SpellCard : Card
{
    public SpellType spellType;
    public List<AttributeTarget>attributeTargets;
    public List<int> attributeChangeAmount;

    public static implicit operator int(SpellCard v)
    {
        throw new NotImplementedException();
    }
}
