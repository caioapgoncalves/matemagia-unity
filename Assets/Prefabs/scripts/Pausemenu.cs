using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class PauseMenu : MonoBehaviour
{
    [Header("Painel de Pause")]
    [SerializeField] GameObject painelPause;
    [SerializeField] float duracaoTransicao = 0.3f;

    [Header("Cutscene (opcional)")]
    [SerializeField] CutsceneController cutsceneController;

    [Header("Cutscene Final (opcional)")]
    [SerializeField] CutsceneFinalController cutsceneFinalController;

    [Header("M�sica (opcional)")]
    [SerializeField] MusicaFase musicaFase;

    bool pausado = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            AlternarPause();
    }

    public void BotaoPause()
    {
        AlternarPause();
    }

    void AlternarPause()
    {
        pausado = !pausado;
        if (pausado)
            Pausar();
        else
            Retomar();
    }

    void Pausar()
    {
        Time.timeScale = 0f;
        AudioListener.pause = true;
        if (cutsceneController != null)
            cutsceneController.PausarVideo();
        if (cutsceneFinalController != null)
            cutsceneFinalController.PausarVideo();
        if (musicaFase != null)
            musicaFase.Pausar();

        painelPause.SetActive(true);
        painelPause.transform.localScale = Vector3.zero;
        painelPause.transform.DOScale(Vector3.one, duracaoTransicao)
            .SetEase(Ease.OutBack)
            .SetUpdate(true);
    }

    public void BotaoRetomar()
    {
        pausado = false;
        Retomar();
    }

    void Retomar()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        if (cutsceneController != null)
            cutsceneController.RetomarVideo();
        if (cutsceneFinalController != null)
            cutsceneFinalController.RetomarVideo();
        if (musicaFase != null)
            musicaFase.Retomar();

        painelPause.transform.DOScale(Vector3.zero, duracaoTransicao)
            .SetEase(Ease.InBack)
            .SetUpdate(true)
            .OnComplete(() => painelPause.SetActive(false));
    }

    public void BotaoMenu()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        PlayerPrefs.SetString("PainelRetorno", "Principal");
        SceneManager.LoadScene("Menu");
    }
}