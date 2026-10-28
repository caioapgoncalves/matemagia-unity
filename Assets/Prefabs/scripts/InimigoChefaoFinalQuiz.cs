using UnityEngine;
using System.Collections;

public class InimigoChefaoFinalQuiz : MonoBehaviour
{
    [SerializeField] public GameObject fxImpacto;
    [SerializeField] private AudioClip somImpacto;
    [SerializeField][Range(0f, 1f)] private float volumeImpacto = 1f;

    [Header("Quiz")]
    public bool gerarSoma = true;
    public int min = 1;
    public int max = 10;
    public string pergunta;
    public string respostaA;
    public string respostaB;
    public string respostaC;
    public string respostaD;
    public int indiceRespostaCorreta;

    [Header("Animator")]
    [SerializeField] public Animator animatorInimigo;

    private Vector3 posicaoOrigem;
    private Animator animator;
    [SerializeField] private Transform colliderFilho;
    private Vector3 posicaoCollider;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        posicaoOrigem = transform.position;

        if (colliderFilho != null)
            posicaoCollider = colliderFilho.position;

        if (fxImpacto != null)
        {
            var vfx = fxImpacto.GetComponentInChildren<UnityEngine.VFX.VisualEffect>();
            if (vfx != null) vfx.Stop();
        }
    }

    public void PararPatrulha(Transform player)
    {
        if (animator != null)
            animator.SetBool("atacando", true);
        StartCoroutine(VirarParaPlayer(player));
    }

    IEnumerator VirarParaPlayer(Transform player)
    {
        Quaternion rotacaoAlvo = Quaternion.LookRotation((player.position - transform.position).normalized);
        rotacaoAlvo = Quaternion.Euler(0f, rotacaoAlvo.eulerAngles.y, 0f);

        while (Quaternion.Angle(transform.rotation, rotacaoAlvo) > 0.5f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacaoAlvo, Time.deltaTime * 2f);
            yield return null;
        }

        transform.rotation = rotacaoAlvo;
    }

    public void VoltarParaOrigem()
    {
        transform.position = posicaoOrigem;
        if (animator != null)
            animator.SetBool("atacando", false);
        ReativarCollider(); 
    }

    public void TocarSomImpacto()
    {
        if (somImpacto != null)
            StartCoroutine(TocarComDelay(0.0f));
    }

    IEnumerator TocarComDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        GameObject obj = new GameObject("SomImpacto");
        AudioSource source = obj.AddComponent<AudioSource>();
        source.clip = somImpacto;
        source.spatialBlend = 0f;
        source.volume = volumeImpacto;
        source.Play();
        Destroy(obj, somImpacto.length);
    }

    public void DesativarCollider()
    {
        if (colliderFilho != null)
            colliderFilho.gameObject.SetActive(false);
    }

    public void ReativarCollider()
    {
        if (colliderFilho != null)
            colliderFilho.gameObject.SetActive(true);
    }

}