using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class CutsceneController : MonoBehaviour
{
    [Header("Video")]
    [SerializeField] VideoPlayer videoPlayer;
    [SerializeField] string nomeArquivoVideo = "cutscene.mp4"; // nome do arquivo em StreamingAssets

    [Header("UI")]
    [SerializeField] GameObject botaoPular;
    [SerializeField] UnityEngine.UI.Image fundoPreto;

    private void Start()
    {
        // Carrega o vídeo via URL para compatibilidade com WebGL
        videoPlayer.source = UnityEngine.Video.VideoSource.Url;
        videoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, nomeArquivoVideo);
        videoPlayer.Prepare();
        videoPlayer.prepareCompleted += OnVideoPronto;
        videoPlayer.loopPointReached += OnVideoTerminou;

        fundoPreto.color = new Color(0, 0, 0, 1);
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
        IrParaAdicao();
    }

    public void BotaoPular()
    {
        videoPlayer.Stop();
        IrParaAdicao();
    }

    public void PausarVideo()
    {
        videoPlayer.Pause();
    }

    public void RetomarVideo()
    {
        videoPlayer.Play();
    }

    void IrParaAdicao()
    {
        fundoPreto.DOFade(1, 0.5f).OnComplete(() =>
        {
            SceneManager.LoadScene("Adicao");
        });
    }

    private void OnDestroy()
    {
        videoPlayer.loopPointReached -= OnVideoTerminou;
        videoPlayer.prepareCompleted -= OnVideoPronto;
    }
}