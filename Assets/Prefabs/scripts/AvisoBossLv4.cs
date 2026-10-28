using UnityEngine;
using TMPro;
using DG.Tweening;

public class AvisoBoss : MonoBehaviour
{
    [Header("Texto")]
    [SerializeField] TMP_Text textoAviso;
    [SerializeField] string textoDoAviso = "Prepare-se... o Chefe se aproxima!";
    [SerializeField] string textoDoAvisoIngles;
    [SerializeField] string textoDoAvisoEspanhol;

    [Header("Animação")]
    [SerializeField] float duracaoReveal = 1.5f;   // tempo para revelar letra a letra
    [SerializeField] float tempoNaTela = 2f;        // quanto tempo fica visível
    [SerializeField] float duracaoFadeOut = 0.8f;  // tempo para sumir

    [Header("Configuração")]
    [SerializeField] string tagJogador = "Player";

    private bool jaAtivou = false; // garante que só dispare uma vez

    private void OnTriggerEnter(Collider other)
    {
        if (jaAtivou) return;
        if (!other.CompareTag(tagJogador)) return;

        jaAtivou = true;
        MostrarAviso();
    }

    private void MostrarAviso()
    {
        string aviso = Idioma.Escolher(textoDoAviso, textoDoAvisoIngles, textoDoAvisoEspanhol);

        textoAviso.gameObject.SetActive(true);
        textoAviso.text = aviso;
        textoAviso.maxVisibleCharacters = 0;
        textoAviso.color = new Color(textoAviso.color.r, textoAviso.color.g, textoAviso.color.b, 1f);

        DOTween.To(
            () => textoAviso.maxVisibleCharacters,
            x => textoAviso.maxVisibleCharacters = x,
            aviso.Length,
            duracaoReveal
        )
        .SetEase(Ease.Linear)
        .OnComplete(() =>
        {
            // Após revelar, espera e faz fade out
            textoAviso.DOFade(0, duracaoFadeOut)
                .SetDelay(tempoNaTela)
                .OnComplete(() => textoAviso.gameObject.SetActive(false));
        });
    }
}
