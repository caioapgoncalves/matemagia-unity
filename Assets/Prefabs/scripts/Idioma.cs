using System;
using UnityEngine;

/// <summary>
/// Idioma atual do jogo (salvo no PlayerPrefs). 0 = Portugues, 1 = Ingles, 2 = Espanhol.
/// </summary>
public static class Idioma
{
    public enum Lingua { Portugues = 0, Ingles = 1, Espanhol = 2 }

    const string Chave = "Idioma";

    /// <summary>Disparado sempre que o idioma muda, pra os textos se atualizarem.</summary>
    public static event Action AoMudar;

    public static Lingua Atual => (Lingua)PlayerPrefs.GetInt(Chave, 0);

    public static void Definir(Lingua lingua)
    {
        PlayerPrefs.SetInt(Chave, (int)lingua);
        PlayerPrefs.Save();
        AoMudar?.Invoke();
    }

    /// <summary>Devolve o texto do idioma atual (se estiver vazio, usa o portugues).</summary>
    public static string Escolher(string portugues, string ingles, string espanhol)
    {
        string texto = Atual == Lingua.Ingles ? ingles
                     : Atual == Lingua.Espanhol ? espanhol
                     : portugues;
        return string.IsNullOrEmpty(texto) ? portugues : texto;
    }
}
