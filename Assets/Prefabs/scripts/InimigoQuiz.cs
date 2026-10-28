using UnityEngine;
using System.Collections;

public class InimigoQuiz : MonoBehaviour
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

    [Header("Patrulha")]
    [SerializeField] private float distanciaPatrulha = 3f;
    [SerializeField] private float velocidadePatrulha = 2f;
    [SerializeField] private bool inverterDirecao = false;
    [SerializeField] private Vector3 direcaoPatrulha = new Vector3(1f, 0f, 0f);
    [SerializeField] private Vector3 eixoFrente = new Vector3(1f, 0f, 0f);

    [Header("Animator")]
    [SerializeField] public Animator animatorInimigo; 

    private Vector3 posicaoOrigem;
    private bool indoParaFrente = true;
    private bool patrulhando = true;
    private float distanciaPercorrida = 0f;

    private Animator animator;

    [SerializeField] private Transform colliderFilho; // arrasta o BoxColliderFilho aqui

    private Vector3 posicaoCollider;

    private void Start()
    {
        posicaoOrigem = transform.position;

        //if (colliderFilho != null)
        //    posicaoCollider = colliderFilho.localPosition; // salva posição local

        animator = GetComponent<Animator>();
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        animator.SetBool("patrulhando", true);

        if (fxImpacto != null)
        {
            var vfx = fxImpacto.GetComponentInChildren<UnityEngine.VFX.VisualEffect>();
            if (vfx != null) vfx.Stop();
        }
    }

    private void Update()
    {
        if (!patrulhando) return;

        // congela o collider filho na posição original
        //if (colliderFilho != null)
        //    colliderFilho.localPosition = posicaoCollider;

        float sentido = indoParaFrente ? 1f : -1f;

        transform.Translate(direcaoPatrulha.normalized * sentido * velocidadePatrulha * Time.deltaTime, Space.World);
        distanciaPercorrida += velocidadePatrulha * Time.deltaTime;

        if (distanciaPercorrida >= distanciaPatrulha)
        {
            distanciaPercorrida = 0f;
            indoParaFrente = !indoParaFrente;
        }

        // Rotaciona pra direção que está andando
        Vector3 direcaoAtual = direcaoPatrulha.normalized * sentido;
        if (direcaoAtual != Vector3.zero)
        {
            Quaternion rotacaoAlvo = Quaternion.LookRotation(direcaoAtual.magnitude * eixoFrente.normalized * Mathf.Sign(Vector3.Dot(direcaoAtual, direcaoPatrulha)));
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacaoAlvo, Time.deltaTime * 2f);
        }
    }

    public void PararPatrulha(Transform player)
    {
        patrulhando = false;
        animator.SetBool("patrulhando", false);
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

    public void VoltarParaOrigem()
    {
        //Debug.Log("Antes: " + transform.position);

        //transform.position = posicaoOrigem;

        //Debug.Log("Depois: " + transform.position);

        //patrulhando = true;
        //indoParaFrente = true;
        //distanciaPercorrida = 0f;
        //animator.SetBool("patrulhando", true);
        transform.position = new Vector3(
     posicaoOrigem.x,
     posicaoOrigem.y,
     posicaoOrigem.z
 );

        patrulhando = true;
        indoParaFrente = true;
        distanciaPercorrida = 0f;
        animator.SetBool("patrulhando", true);
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
}