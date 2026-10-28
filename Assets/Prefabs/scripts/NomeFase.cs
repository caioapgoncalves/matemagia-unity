using UnityEngine;
using TMPro;
using DG.Tweening;

public class NomeFase : MonoBehaviour
{
    [Header("Texto")]
    [SerializeField] TMP_Text textoNome;
    [SerializeField] string nomeDaFase = "Floresta Tropical";
    [SerializeField] string nomeDaFaseIngles;
    [SerializeField] string nomeDaFaseEspanhol;

    [Header("Animação")]
    [SerializeField] float duracaoReveal = 1.5f;   // tempo para revelar letra a letra
    [SerializeField] float tempoNaTela = 2f;        // quanto tempo fica visível
    [SerializeField] float duracaoFadeOut = 0.8f;  // tempo para sumir

    private void Start()
    {
        string nome = Idioma.Escolher(nomeDaFase, nomeDaFaseIngles, nomeDaFaseEspanhol);

        textoNome.text = nome;
        textoNome.maxVisibleCharacters = 0;
        textoNome.color = new Color(textoNome.color.r, textoNome.color.g, textoNome.color.b, 1f);

        DOTween.To(
            () => textoNome.maxVisibleCharacters,
            x => textoNome.maxVisibleCharacters = x,
            nome.Length,
            duracaoReveal
        )
        .SetEase(Ease.Linear)
        .OnComplete(() =>
        {
            // Após revelar, espera e faz fade out
            textoNome.DOFade(0, duracaoFadeOut)
                .SetDelay(tempoNaTela)
                .OnComplete(() => gameObject.SetActive(false));
        });
    }
}
