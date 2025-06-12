using UnityEngine;

public class SpellEffectApplier : MonoBehaviour
{
    public static void ApplySpell(SpellCard spellCard, CharacterStats targetStats)
    {
        // Recibe la estadisticas del hechizo, datos del hechizo y la carta a la cual esta dirigido el hechizo 
        for (int i = 0; i < spellCard.attributeChangeAmount.Count; ++i)
        {
            int changeAmount = spellCard.attributeChangeAmount.Count > i ? spellCard.attributeChangeAmount[i] : 0;
            ApllyEffectToAttribute(spellCard, spellCard.spellType, spellCard.attributeTargets[i], changeAmount, targetStats);
        }
    }
    private static void ApllyEffectToAttribute(SpellCard spellCard, SpellCard.SpellType spellType, Card.AttributeTarget attributeTarget, int changeAmount, CharacterStats targetStats)
    {
        // Cambia los atributos de character seleccionado segun el tipo de hechizo
        int finalChangeAmount = spellType == SpellCard.SpellType.buff ? changeAmount : -changeAmount;
        switch (attributeTarget)
        {
            case Card.AttributeTarget.Health:
                targetStats.Health += finalChangeAmount;
                break;
            case Card.AttributeTarget.Attack:
                targetStats.Attack += finalChangeAmount;
                break;
            default:
                System.Diagnostics.Debug.WriteLine("Error al lanzar hechizo.");
                break;
        }
        ClampCharacterStats(targetStats);
    }
    private static void ClampCharacterStats(CharacterStats stats)
    {
        // No permite que las estadisticas bajen por debajo de 0
        stats.Health = Mathf.Max(stats.Health, 0); // funcion que devuelve el numero mas alto entre un numero y 0
        stats.Attack = Mathf.Max(stats.Attack, 0);
    }

}
