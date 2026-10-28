using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Mostra este objeto (e os filhos) so quando o jogo esta em portugues.
/// Usado nos acentos desenhados a parte (´ ~ ç) do menu.
/// </summary>
public class SomenteEmPortugues : MonoBehaviour
{
    void OnEnable()
    {
        Idioma.AoMudar += Aplicar;
        Aplicar();
    }

    void OnDisable()
    {
        Idioma.AoMudar -= Aplicar;
    }

    void Aplicar()
    {
        bool mostrar = Idioma.Atual == Idioma.Lingua.Portugues;
        foreach (Graphic grafico in GetComponentsInChildren<Graphic>(true))
            grafico.enabled = mostrar;
    }
}
