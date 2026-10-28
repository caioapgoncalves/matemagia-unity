using UnityEngine;

/// <summary>
/// Botoes PT / EN / ES do menu. Ligue cada botao (OnClick) ao metodo do idioma.
/// O botao do idioma atual fica aceso e os outros ficam apagados.
/// </summary>
public class SeletorIdioma : MonoBehaviour
{
    [SerializeField] CanvasGroup botaoPortugues;
    [SerializeField] CanvasGroup botaoIngles;
    [SerializeField] CanvasGroup botaoEspanhol;
    [SerializeField, Range(0f, 1f)] float transparenciaApagado = 0.45f;

    void OnEnable()
    {
        Idioma.AoMudar += Atualizar;
        Atualizar();
    }

    void OnDisable()
    {
        Idioma.AoMudar -= Atualizar;
    }

    public void Portugues() => Idioma.Definir(Idioma.Lingua.Portugues);
    public void Ingles() => Idioma.Definir(Idioma.Lingua.Ingles);
    public void Espanhol() => Idioma.Definir(Idioma.Lingua.Espanhol);

    void Atualizar()
    {
        Idioma.Lingua atual = Idioma.Atual;
        if (botaoPortugues != null) botaoPortugues.alpha = atual == Idioma.Lingua.Portugues ? 1f : transparenciaApagado;
        if (botaoIngles != null) botaoIngles.alpha = atual == Idioma.Lingua.Ingles ? 1f : transparenciaApagado;
        if (botaoEspanhol != null) botaoEspanhol.alpha = atual == Idioma.Lingua.Espanhol ? 1f : transparenciaApagado;
    }
}
