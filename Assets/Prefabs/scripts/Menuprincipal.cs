using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;

public class MenuPrincipal : MonoBehaviour
{
    [Header("Paineis")]
    [SerializeField] GameObject painelPrincipal;
    [SerializeField] GameObject painelFases;
    [SerializeField] GameObject painelSobre;

    [Header("Animação dos painéis")]
    [SerializeField] float duracaoTransicao = 0.3f;

    private void Start()
    {
        painelPrincipal.SetActive(false);
        painelFases.SetActive(false);
        painelSobre.SetActive(false);

        string painelRetorno = PlayerPrefs.GetString("PainelRetorno", "Principal");

        if (painelRetorno == "Fases")
        {
            PlayerPrefs.SetString("PainelRetorno", "Principal");
            AbrirPainel(painelFases);
        }
        else
        {
            AbrirPainel(painelPrincipal);
        }
    }

    // ── Botões do painel principal ──

    public void BotaoModoHistoria()
    {
        PlayerPrefs.SetInt("HistoriaAcertos", 0);
        PlayerPrefs.SetInt("HistoriaErros", 0);
        PlayerPrefs.SetString("HistoriaRespostas", "{\"items\":[]}");
        PlayerPrefs.Save();
        PlayerPrefs.SetInt("ModoHistoria", 1);
        PlayerPrefs.SetString("PainelRetorno", "Principal");
        SceneManager.LoadScene("CutsceneInicial");
    }

    public void BotaoFases()
    {
        AbrirPainel(painelFases);
    }

    public void BotaoSobre()
    {
        AbrirPainel(painelSobre);
    }

    public void BotaoSair()
    {
        Application.Quit();
        Debug.Log("Saindo do jogo...");
    }

    // ── Botões do painel de fases ──

    public void CarregarAdicao()
    {
        PlayerPrefs.SetInt("ModoHistoria", 0);
        PlayerPrefs.SetString("PainelRetorno", "Fases");
        SceneManager.LoadScene("Adicao");
    }

    public void CarregarSubtracao()
    {
        PlayerPrefs.SetInt("ModoHistoria", 0);
        PlayerPrefs.SetString("PainelRetorno", "Fases");
        SceneManager.LoadScene("Subtracao");
    }

    public void CarregarDivisao()
    {
        PlayerPrefs.SetInt("ModoHistoria", 0);
        PlayerPrefs.SetString("PainelRetorno", "Fases");
        SceneManager.LoadScene("Divisao");
    }

    public void CarregarMultiplicacao()
    {
        PlayerPrefs.SetInt("ModoHistoria", 0);
        PlayerPrefs.SetString("PainelRetorno", "Fases");
        SceneManager.LoadScene("Multiplicacao");
    }

    // ── Botão Voltar ──

    public void Voltar()
    {
        AbrirPainel(painelPrincipal);
        painelFases.SetActive(false);
        painelSobre.SetActive(false);
    }

    // ── Utilitário ──

    void AbrirPainel(GameObject painel)
    {
        painelPrincipal.SetActive(false);
        painelFases.SetActive(false);
        painelSobre.SetActive(false);

        painel.SetActive(true);
        painel.transform.localScale = Vector3.zero;
        painel.transform.DOScale(Vector3.one, duracaoTransicao).SetEase(Ease.OutBack);
    }
}