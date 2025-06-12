using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public static class Utility
{
    public static void Shuffle<T>(List<T> list)
    {
        // Baraja cartas en una lista
        // Altera el orden de forma aleatoria de una lista de objetos
        System.Random random = new System.Random();
        int n = list.Count;
        for (int i = n - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (list[j], list[i]) = (list[i], list[j]);
        }
    }
    public static int Diceroll()
    {
        // Genera un numero aleatorio entre 1 y 20 simulando el lanzamiento de un dado usado para las tiradas de ataque durante la pelea
        System.Random random = new System.Random();
        int n = 0;
        n = random.Next(1, 20);
        if (PlayerPrefs.GetString("SelectedHero", "Cupid") == "San Patric")
        {
            if (n == PlayerPrefs.GetInt("LuckyNumber", 0))
            {
                GameManager.Instance.PlayerMana += 1;
            }
        }

        return n;
    }
    public static int CounterDice()
    {
        // Genera un numero aleatorio entre 1 y 6 simulando el lanzamiento de un dado usado para las tiradas de ataque de los jefes durante la pelea
        System.Random random = new System.Random();
        int dice = 0;
        dice = random.Next(1, 6);
        return dice;
    }

    public static IEnumerator FadeIn(CanvasGroup group, float alpha, float duration)
    {
        // Funcion de aparicion, hace aparecer un objeto de forma gradual 
        var time = 0.0f;
        var originalAlpha = group.alpha;
        while (time < duration)
        {
            time += Time.deltaTime;
            group.alpha = Mathf.Lerp(originalAlpha, alpha, time / duration);
            yield return new WaitForEndOfFrame();
        }
        group.alpha = alpha;
    }
    public static IEnumerator FadeOut(CanvasGroup group, float alpha, float duration)
    {
        // Funcion de desaparecer, hace desaparecer un objeto de forma gradual 
        var time = 0.0f;
        var originalAlpha = group.alpha;
        while (time < duration)
        {
            time += Time.deltaTime;
            group.alpha = Mathf.Lerp(originalAlpha, alpha, time / duration);
            yield return new WaitForEndOfFrame();
        }
        group.alpha = alpha;
    }
}

