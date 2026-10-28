using System.IO;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class CutsceneFinalController : MonoBehaviour
{
    [Header("Video")]
    [SerializeField] VideoPlayer videoPlayer;
    [SerializeField] string nomeArquivoVideo = "cutscenefinal.mp4";

    [Header("UI")]
    [SerializeField] GameObject botaoPular;
    [SerializeField] UnityEngine.UI.Image fundoPreto;

    private void Start()
    {
        fundoPreto.color = new Color(0, 0, 0, 1);

        videoPlayer.url = Path.Combine(Application.streamingAssetsPath, nomeArquivoVideo);
        videoPlayer.prepareCompleted += OnVideoPronto;
        videoPlayer.loopPointReached += OnVideoTerminou;
        videoPlayer.Prepare();
    }

    void OnVideoPronto(VideoPlayer vp)
    {
        fundoPreto.DOFade(0, 1f).SetDelay(0.5f).OnComplete(() =>
        {
            videoPlayer.Play();
            botaoPular.SetActive(true);
        });
    }

    void OnVideoTerminou(VideoPlayer vp)
    {
        IrParaMenu();
    }

    public void BotaoPular()
    {
        videoPlayer.Stop();
        IrParaMenu();
    }

    public void PausarVideo()
    {
        videoPlayer.Pause();
    }

    public void RetomarVideo()
    {
        videoPlayer.Play();
    }

    void IrParaMenu()
    {
        fundoPreto.DOFade(1, 0.5f).OnComplete(() =>
        {
            SceneManager.LoadScene("Menu");
        });
    }

    private void OnDestroy()
    {
        videoPlayer.loopPointReached -= OnVideoTerminou;
        videoPlayer.prepareCompleted -= OnVideoPronto;
    }
}