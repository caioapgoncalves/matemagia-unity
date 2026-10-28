using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class MusicaFase : MonoBehaviour
{
    [Header("Música")]
    [SerializeField] AudioClip musica;
    [SerializeField][Range(0f, 0.3f)] float volume = 0.3f;

    [Header("Fade")]
    [SerializeField] float duracaoFadeIn = 2f;
    [SerializeField] float duracaoFadeOut = 1f;

    private AudioSource audioSource;

    public static MusicaFase instancia;

    private void Awake()
    {
        instancia = this;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = musica;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = 0f;
    }

    private void Start()
    {
        audioSource.Play();
        DOTween.To(() => audioSource.volume, x => audioSource.volume = x, volume, duracaoFadeIn);
    }

    public void Pausar()
    {
        audioSource.Pause();
    }

    public void Retomar()
    {
        audioSource.UnPause();
    }

    // Só faz o fadeout, sem trocar de scene (usado na Comemoracao)
    public void FadeOut()
    {
        DOTween.To(() => audioSource.volume, x => audioSource.volume = x, 0f, duracaoFadeOut);
    }

    // Fadeout e carrega scene pelo nome (usado ao voltar pro menu)
    public void FadeOutECarregar(string nomeScene)
    {
        DOTween.To(() => audioSource.volume, x => audioSource.volume = x, 0f, duracaoFadeOut)
            .OnComplete(() => SceneManager.LoadScene(nomeScene));
    }
}