using TMPro;
using UnityEngine;

/// <summary>
/// Coloque em qualquer texto (TextMeshPro) e preencha as tres versoes no Inspector.
/// O texto troca sozinho quando o idioma muda.
/// </summary>
[RequireComponent(typeof(TMP_Text))]
public class TextoTraduzido : MonoBehaviour
{
    [TextArea(1, 8)] public string portugues;
    [TextArea(1, 8)] public string ingles;
    [TextArea(1, 8)] public string espanhol;

    void OnEnable()
    {
        Idioma.AoMudar += Aplicar;
        Aplicar();
    }

    void OnDisable()
    {
        Idioma.AoMudar -= Aplicar;
    }

    public void Aplicar()
    {
        string texto = Idioma.Escolher(portugues, ingles, espanhol);
        if (!string.IsNullOrEmpty(texto))
            GetComponent<TMP_Text>().text = texto;
    }
}
