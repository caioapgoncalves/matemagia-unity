using UnityEngine;
using UnityEngine.SceneManagement;

using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class AndarAutomatico : MonoBehaviour
{
    [SerializeField] float velocidade = 6;
    [SerializeField] GameObject quizPainel;
    [SerializeField] QuizIntermed quiz;
    [SerializeField] GameObject[] coracoes;
    [SerializeField] GameObject[] inimigos;
    Quaternion rotacaoOriginal;
    [SerializeField] Transform visualPlayer;

    [SerializeField] private GameObject fireballEsquerda;
    [SerializeField] private GameObject fireballDireita;
    [SerializeField] private Transform pontoDisparoEsquerda;
    [SerializeField] private Transform pontoDisparoDireita;
    [SerializeField] AudioClip somFireball;
    [SerializeField] AudioClip somPassos;
    [SerializeField] AudioClip somVitoria;
    private AudioSource audioSource;

    int vidas = 3;
    Animator animatorInimigo;
    bool atacarAlternado = false;
    InimigoQuiz inimigoAtual;
    InimigoChefaoFinalQuiz chefaoAtual;
    bool encontrouChefao = false;

    Vector3 posicaoInicial;
    bool podeAndar = true;
    int acertos = 0;
    int erros = 0;
    float tempoInicio;
    bool modoHistoria = true;
    int voltasCompletas = 0;
    [SerializeField] int totalVoltas = 3;

    Animator animator;

#if UNITY_WEBGL && !UNITY_EDITOR
    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void SalvarRespostasHistoria(string json);

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern string CarregarRespostasHistoria();

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void LimparRespostasHistoria();
#endif

    // Wrappers: no build WebGL usam localStorage (via jslib), no Editor caem para PlayerPrefs
    // pra dar pra testar sem precisar rodar build toda hora.
    void SalvarRespostas(RespostaDataList lista)
    {
        string json = JsonUtility.ToJson(lista);
        Debug.Log("[HistoriaDebug] Salvando no localStorage: " + json);
#if UNITY_WEBGL && !UNITY_EDITOR
        SalvarRespostasHistoria(json);
#else
        PlayerPrefs.SetString("HistoriaRespostas", json);
        PlayerPrefs.Save();
#endif
    }

    RespostaDataList CarregarRespostas()
    {
        string json;
#if UNITY_WEBGL && !UNITY_EDITOR
        json = CarregarRespostasHistoria();
        if (string.IsNullOrEmpty(json)) json = "{\"items\":[]}";
#else
        json = PlayerPrefs.GetString("HistoriaRespostas", "{\"items\":[]}");
#endif
        Debug.Log("[HistoriaDebug] Carregado do localStorage (raw): " + json);
        return JsonUtility.FromJson<RespostaDataList>(json);
    }

    void LimparRespostas()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        LimparRespostasHistoria();
#else
        PlayerPrefs.SetString("HistoriaRespostas", "{\"items\":[]}");
        PlayerPrefs.Save();
#endif
    }

    private void Start()
    {
        posicaoInicial = transform.position;
        animator = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = somPassos;
        audioSource.loop = true;
        modoHistoria = PlayerPrefs.GetInt("ModoHistoria", 1) == 1;
        tempoInicio = Time.time;

        // Reseta na primeira fase ou no modo fases ANTES de carregar acumulado
        if (!modoHistoria || UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Adicao")
        {
            PlayerPrefs.SetInt("HistoriaAcertos", 0);
            PlayerPrefs.SetInt("HistoriaErros", 0);
            LimparRespostas();
            quiz.ResetarRespostas();
        }

        // Carrega acumulado DEPOIS do reset
        if (modoHistoria)
        {
            acertos = PlayerPrefs.GetInt("HistoriaAcertos", 0);
            erros = PlayerPrefs.GetInt("HistoriaErros", 0);
        }
    }
    void Update()
    {
        if (podeAndar)
        {
            Vector3 direcao = new Vector3(0f, 0f, 1f);
            transform.Translate(direcao * velocidade * Time.deltaTime);

            animator.SetBool("correndo", true);

            if (!audioSource.isPlaying)
                audioSource.Play();
        }
        else
        {
            animator.SetBool("correndo", false);

            if (audioSource.isPlaying)
                audioSource.Stop();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter chamado #" + Time.frameCount + " tag: " + other.tag + " objeto: " + other.name);

        if (other.CompareTag("Inimigo") && podeAndar)
        {
            podeAndar = false;
            inimigoAtual = other.GetComponent<InimigoQuiz>();
            if (inimigoAtual == null)
                inimigoAtual = other.GetComponentInParent<InimigoQuiz>();
            if (inimigoAtual == null)
                inimigoAtual = other.transform.parent?.GetComponentInChildren<InimigoQuiz>();

            Debug.Log("inimigoAtual: " + inimigoAtual);
            Debug.Log("other: " + other.name + " | pai: " + other.transform.parent?.name);

            animatorInimigo = inimigoAtual.animatorInimigo;

            inimigoAtual?.PararPatrulha(transform);
            inimigoAtual?.DesativarCollider();
            quiz.AbrirQuiz();
            quiz.TocarSomEncontro();
            Debug.Log("ATIVANDO quizPainel INIMIGO - tag: " + other.tag);
            quizPainel.SetActive(true);
            Debug.Log("Animator inimigo: " + animatorInimigo);
        }
        if (other.CompareTag("InimigoFinal") && podeAndar && !encontrouChefao)
        {
            encontrouChefao = true;
            podeAndar = false;
            chefaoAtual = other.GetComponent<InimigoChefaoFinalQuiz>();
            if (chefaoAtual == null)
                chefaoAtual = other.GetComponentInParent<InimigoChefaoFinalQuiz>();
            if (chefaoAtual == null)
                chefaoAtual = other.transform.parent?.GetComponentInChildren<InimigoChefaoFinalQuiz>();

            if (chefaoAtual == null) return;

            Debug.Log("chefaoAtual: " + chefaoAtual);
            Debug.Log("other: " + other.name + " | pai: " + other.transform.parent?.name);

            animatorInimigo = chefaoAtual.animatorInimigo;

            chefaoAtual?.PararPatrulha(transform);
            quiz.DefinirChefao(chefaoAtual);
            quiz.AbrirQuiz();
            quiz.TocarSomEncontro();
            Debug.Log("ATIVANDO quizPainel CHEFAO - tag: " + other.tag);
            quizPainel.SetActive(true);
            Debug.Log("Animator chefao: " + animatorInimigo);
        }
        if (other.CompareTag("Comemoracao"))
        {
            podeAndar = false;
            if (MusicaFase.instancia != null)
                MusicaFase.instancia.FadeOut();
            StartCoroutine(EsperarVitoria());
        }
        if (other.CompareTag("Finish"))
        {
            int proximaIndex = SceneManager.GetActiveScene().buildIndex + 1;
            SceneManager.LoadScene(proximaIndex);
        }
    }

    public void Acertou()
    {
        acertos++;
        StartCoroutine(EsperarAtaque());
    }

    IEnumerator EsperarVitoria()
    {
        animator.SetTrigger("vencerLevel");
        AudioSource.PlayClipAtPoint(somVitoria, transform.position);
        yield return null;
        yield return null;
        float duracao = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(duracao);

        if (modoHistoria)
        {
            PlayerPrefs.SetInt("HistoriaAcertos", PlayerPrefs.GetInt("HistoriaAcertos", 0) + acertos);
            PlayerPrefs.SetInt("HistoriaErros", PlayerPrefs.GetInt("HistoriaErros", 0) + erros);
            var respostasAtuais = quiz.GetRespostas();
            var listaAntiga = CarregarRespostas();
            Debug.Log("Lista antiga count: " + (listaAntiga?.items?.Count ?? -1));
            listaAntiga.items.AddRange(respostasAtuais);
            SalvarRespostas(listaAntiga);
            quiz.ResetarRespostas(); // ← adiciona essa linha aqui!
            acertos = 0;
            erros = 0;
            podeAndar = true;
        }
        else
        {
            voltasCompletas++;
            if (voltasCompletas >= totalVoltas)
            {
                // Envia desempenho apenas na ultima volta com total acumulado
                int tempoTotal = (int)(Time.time - tempoInicio);
                int tempoSegundos = tempoTotal / totalVoltas; // tempo medio por volta
                string fase = modoHistoria ? "Modo Historia" : UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
                var respostas = quiz.GetRespostas();
                Debug.Log("ApiManager instancia: " + ApiManager.instancia);
                if (ApiManager.instancia != null)
                    ApiManager.instancia.Enviar(fase, acertos, erros, tempoSegundos, respostas);
                else
                    Debug.LogWarning("ApiManager.instancia e null!");

                if (MusicaFase.instancia != null)
                    MusicaFase.instancia.FadeOutECarregar("Menu");
                else
                    SceneManager.LoadScene("Menu");
            }
            else
            {
                tempoInicio = Time.time;
                Reiniciar();
            }
        }
    }

    public void EnviarDadosParciais()
    {
        int totalAcertos = modoHistoria ? PlayerPrefs.GetInt("HistoriaAcertos", 0) + acertos : acertos;
        int totalErros = modoHistoria ? PlayerPrefs.GetInt("HistoriaErros", 0) + erros : erros;
        if (totalAcertos == 0 && totalErros == 0) return;
        int tempoSegundos = (int)(Time.time - tempoInicio) / Mathf.Max(voltasCompletas, 1);
        string fase = modoHistoria ? "Modo Historia" : UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        List<RespostaData> todasRespostas = new List<RespostaData>();
        if (modoHistoria)
        {
            var listaAntiga = CarregarRespostas();
            if (listaAntiga != null) todasRespostas.AddRange(listaAntiga.items);
        }
        todasRespostas.AddRange(quiz.GetRespostas());
        if (ApiManager.instancia != null)
            ApiManager.instancia.Enviar(fase, totalAcertos, totalErros, tempoSegundos, todasRespostas);
        PlayerPrefs.SetInt("HistoriaAcertos", 0);
        PlayerPrefs.SetInt("HistoriaErros", 0);
        LimparRespostas();
        PlayerPrefs.Save();
    }

    void OnApplicationQuit()
    {
        EnviarDadosParciais();
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
            EnviarDadosParciais();
    }

    IEnumerator EsperarAtaque()
    {
        rotacaoOriginal = transform.rotation;
        Vector3 direcaoInimigo = animatorInimigo.transform.position - transform.position;
        direcaoInimigo.y = 0f;
        transform.DORotateQuaternion(Quaternion.LookRotation(direcaoInimigo), 0.3f);
        yield return new WaitForSeconds(0.3f);

        if (atacarAlternado)
        {
            animator.SetTrigger("atacarEsquerda");
            StartCoroutine(VoarFireball(true));
        }
        else
        {
            animator.SetTrigger("atacarDireita");
            StartCoroutine(VoarFireball(false));
        }

        atacarAlternado = !atacarAlternado;

        yield return null;
        yield return null;
        float duracao = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(duracao);

        animatorInimigo.SetTrigger("morrer");

        yield return null;
        yield return null;
        while (!animatorInimigo.GetCurrentAnimatorStateInfo(0).IsName("Morrendo"))
        {
            Debug.Log("Estado atual: " + animatorInimigo.GetCurrentAnimatorStateInfo(0).IsName("Morrendo") + " | hash: " + animatorInimigo.GetCurrentAnimatorStateInfo(0).shortNameHash);
            yield return null;
        }

        float duracaoMorte = animatorInimigo.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(duracaoMorte);

        transform.DORotateQuaternion(rotacaoOriginal, 0.3f);
        yield return new WaitForSeconds(0.3f);

        podeAndar = true;
    }

    IEnumerator VoarFireball(bool esquerda)
    {
        GameObject fireball = esquerda ? fireballEsquerda : fireballDireita;
        Transform ponto = esquerda ? pontoDisparoDireita : pontoDisparoEsquerda;

        yield return null;
        yield return null;
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime == 0 ||
               (!animator.GetCurrentAnimatorStateInfo(0).IsName("atacarEsquerda") &&
                !animator.GetCurrentAnimatorStateInfo(0).IsName("atacarDireita")))
        {
            yield return null;
        }

        float duracao = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(duracao * 0.5f);

        fireball.transform.SetParent(ponto);
        fireball.transform.localPosition = Vector3.zero;
        fireball.transform.localRotation = Quaternion.identity;

        Vector3 posicaoInicial = ponto.position;
        Vector3 alvo = animatorInimigo.transform.position + Vector3.up * 25f;

        fireball.SetActive(true);
        AudioSource.PlayClipAtPoint(somFireball, fireball.transform.position);
        fireball.transform.SetParent(null);
        fireball.transform.position = posicaoInicial;

        Vector3 direcao = (alvo - posicaoInicial).normalized;
        fireball.transform.rotation = Quaternion.LookRotation(direcao);

        fireball.transform.DOMove(alvo, 0.4f).OnComplete(() =>
        {
            fireball.transform.SetParent(ponto);
            fireball.transform.localPosition = Vector3.zero;
            fireball.transform.localRotation = Quaternion.identity;
            fireball.SetActive(false);

            GameObject fx = inimigoAtual != null ? inimigoAtual.fxImpacto : chefaoAtual?.fxImpacto;
            if (fx != null)
            {
                fx.transform.position = alvo;
                fx.SetActive(true);
                if (inimigoAtual != null) inimigoAtual.TocarSomImpacto();
                else chefaoAtual?.TocarSomImpacto();
                DOVirtual.DelayedCall(1.5f, () => fx.SetActive(false));
            }
        });
    }

    public void ErrarVida()
    {
        erros++;
        vidas--;
        coracoes[vidas].SetActive(false);

        if (vidas <= 0)
        {
            quizPainel.SetActive(false);
            StartCoroutine(EsperarGameOver());
        }
        else
        {
            animator.SetTrigger("levarDano");
        }
    }

    IEnumerator EsperarGameOver()
    {
        animator.SetTrigger("morrer");
        quiz.TocarSomGameOver();
        yield return null;
        yield return null;
        float duracao = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(duracao);
        voltasCompletas = 0;
        Reiniciar();
    }

    void Reiniciar()
    {
        vidas = 3;
        foreach (GameObject c in coracoes)
            c.SetActive(true);

        foreach (GameObject inimigo in inimigos)
        {
            InimigoQuiz iq = inimigo.GetComponent<InimigoQuiz>();
            iq?.VoltarParaOrigem();
            iq?.ReativarCollider();
            Animator anim = inimigo.GetComponentInChildren<Animator>();
            if (anim != null)
            {
                anim.Rebind();
                anim.Update(0f);
                anim.SetBool("patrulhando", true);
            }
        }
        chefaoAtual?.VoltarParaOrigem();
        chefaoAtual = null;
        encontrouChefao = false;

        transform.position = posicaoInicial;
        visualPlayer.rotation = rotacaoOriginal;
        quizPainel.SetActive(false);
        podeAndar = true;
    }
}