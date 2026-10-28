using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuizIntermed : MonoBehaviour
{
    [SerializeField] AndarAutomatico player;

    [Header("Textos")]
    [SerializeField] TMP_Text textoPergunta;
    [SerializeField] TMP_Text textoA;
    [SerializeField] TMP_Text textoB;
    [SerializeField] TMP_Text textoC;
    [SerializeField] TMP_Text textoD;
    [SerializeField] Animator animatorPlayer;

    [SerializeField] UnityEngine.UI.Image flashPanel;
    [SerializeField] UnityEngine.UI.Image certoSimbolo;
    [SerializeField] UnityEngine.UI.Image erradoSimbolo;

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip somCerto;
    [SerializeField] AudioClip somErrado;
    [SerializeField] AudioClip somEncontroInimigo;
    [SerializeField] AudioClip somGameOver;

    [Header("Referencia visual (figuras)")]
    [Tooltip("Liga/desliga as figuras de apoio ao lado dos numeros.")]
    [SerializeField] bool usarFigurasVisuais = true;
    [Tooltip("Container das figuras do primeiro numero (com Grid Layout Group).")]
    [SerializeField] Transform containerFigurasA;
    [Tooltip("Container das figuras do segundo numero (com Grid Layout Group).")]
    [SerializeField] Transform containerFigurasB;
    [Tooltip("Prefab de UI (Image) que representa uma unidade.")]
    [SerializeField] GameObject prefabFigura;
    [Tooltip("Imagem das figuras nesta fase (cogumelo, cacto, ametista, coqueiro). Vazio = usa a imagem do prefab.")]
    [SerializeField] Sprite spriteFigura;

    [Header("Numeros")]
    [Tooltip("Maior numero que aparece na conta (os numeros sao sorteados de 1 ate este valor).")]
    [SerializeField] int numeroMaximo = 5;

    public enum TipoOperacao { Soma, Subtracao, Multiplicacao, Divisao }
    [SerializeField] TipoOperacao operacao;

    InimigoChefaoFinalQuiz chefaoAtual;
    int acertosFeitos = 0;
    [SerializeField] int acertosNecessarios = 3;
    List<TipoOperacao> operacoesDisponiveis = new List<TipoOperacao>();
    List<RespostaData> respostasRegistradas = new List<RespostaData>();

    int respostaCorreta;

    void Awake()
    {
        Debug.Log("BUILD V2 - padrao de dado");
    }
    public void AbrirQuiz()
    {
        Debug.Log("AbrirQuiz neste objeto: " + gameObject.name + " | ID: " + GetInstanceID());
        GerarOperacao();
    }

    public void ResetarRespostas()
    {
        respostasRegistradas = new List<RespostaData>();
    }

    public void DefinirChefao(InimigoChefaoFinalQuiz chefao)
    {
        chefaoAtual = chefao;
        acertosFeitos = 0;
        operacoesDisponiveis = new List<TipoOperacao> { TipoOperacao.Soma, TipoOperacao.Subtracao, TipoOperacao.Multiplicacao, TipoOperacao.Divisao };
    }

    public void TocarSomEncontro()
    {
        AudioSource.PlayClipAtPoint(somEncontroInimigo, Camera.main.transform.position);
    }

    public void TocarSomGameOver()
    {
        AudioSource.PlayClipAtPoint(somGameOver, Camera.main.transform.position);
    }

    void GerarOperacao()
    {
        int a = Random.Range(1, numeroMaximo + 1);
        int b = Random.Range(1, numeroMaximo + 1);
        int correta = 0;
        string simbolo = "";

        TipoOperacao op;
        if (chefaoAtual != null)
        {
            if (operacoesDisponiveis.Count == 0)
                operacoesDisponiveis = new List<TipoOperacao> { TipoOperacao.Soma, TipoOperacao.Subtracao, TipoOperacao.Multiplicacao, TipoOperacao.Divisao };

            int idx = Random.Range(0, operacoesDisponiveis.Count);
            op = operacoesDisponiveis[idx];
            operacoesDisponiveis.RemoveAt(idx);
        }
        else
        {
            op = operacao;
        }

        switch (op)
        {
            case TipoOperacao.Soma:
                correta = a + b;
                simbolo = "+";
                break;
            case TipoOperacao.Subtracao:
                if (a < b) (a, b) = (b, a); // garante resultado positivo
                correta = a - b;
                simbolo = "-";
                break;
            case TipoOperacao.Multiplicacao:
                correta = a * b;
                simbolo = "x";
                break;
            case TipoOperacao.Divisao:
                // dividendo vai ate numeroMaximo; divisor sorteado entre os divisores dele (divisao exata)
                List<int> divisores = new List<int>();
                for (int d = 1; d <= a; d++)
                    if (a % d == 0) divisores.Add(d);
                b = divisores[Random.Range(0, divisores.Count)];
                correta = a / b;
                simbolo = "÷";
                break;
        }

        textoPergunta.text = $"{a} {simbolo}    {b} = ?";

        AtualizarFiguras(a, b, op);

        HashSet<int> opcoes = new HashSet<int>();
        opcoes.Add(correta);

        while (opcoes.Count < 4)
        {
            int errada = correta + Random.Range(-5, 6);
            if (errada == correta) continue;
            if (errada < 0) continue;
            opcoes.Add(errada);
        }

        List<int> lista = new List<int>(opcoes);
        for (int i = 0; i < lista.Count; i++)
        {
            int j = Random.Range(i, lista.Count);
            (lista[i], lista[j]) = (lista[j], lista[i]);
        }

        textoA.text = lista[0].ToString();
        textoB.text = lista[1].ToString();
        textoC.text = lista[2].ToString();
        textoD.text = lista[3].ToString();

        respostaCorreta = lista.IndexOf(correta);
    }

    void AtualizarFiguras(int a, int b, TipoOperacao op)
    {
        if (containerFigurasA == null || containerFigurasB == null) return;

        LimparContainer(containerFigurasA);
        LimparContainer(containerFigurasB);

        bool mostrar = usarFigurasVisuais && prefabFigura != null;

        // a grade 3x3 so tem padrao ate 6
        if (mostrar && (a >= padroesDado.Length || b >= padroesDado.Length))
            mostrar = false;

        containerFigurasA.gameObject.SetActive(mostrar);
        containerFigurasB.gameObject.SetActive(mostrar);

        if (!mostrar) return;

        CriarFiguras(containerFigurasA, a);
        CriarFiguras(containerFigurasB, b);
    }

    // Posicoes da grade 3x3:  0 1 2 / 3 4 5 / 6 7 8
    static readonly int[][] padroesDado = new int[][]
    {
    new int[] { },                      // 0
    new int[] { 4 },                    // 1
    new int[] { 0, 8 },                 // 2
    new int[] { 0, 4, 8 },              // 3
    new int[] { 0, 2, 6, 8 },           // 4
    new int[] { 0, 2, 4, 6, 8 },        // 5
    new int[] { 0, 2, 3, 5, 6, 8 },     // 6
    };

    void CriarFiguras(Transform container, int quantidade)
    {
        HashSet<int> ativos = new HashSet<int>(padroesDado[quantidade]);

        for (int i = 0; i < 9; i++)
        {
            GameObject figura = Instantiate(prefabFigura, container);
            figura.transform.localScale = Vector3.one;
            figura.SetActive(true);

            if (spriteFigura != null)
            {
                UnityEngine.UI.Image imagem = figura.GetComponent<UnityEngine.UI.Image>();
                imagem.sprite = spriteFigura;
                imagem.preserveAspect = true;
            }

            CanvasGroup grupo = figura.GetComponent<CanvasGroup>();
            if (grupo == null) grupo = figura.AddComponent<CanvasGroup>();
            grupo.alpha = ativos.Contains(i) ? 1f : 0f;
        }
    }

    void LimparContainer(Transform container)
    {
        for (int i = container.childCount - 1; i >= 0; i--)
        {
            Transform filho = container.GetChild(i);
            filho.SetParent(null); // tira na hora, pro Destroy nao atrasar o layout
            Destroy(filho.gameObject);
        }
    }

    public void EscolherA()
    {
        Debug.Log("CLICOU A neste objeto: " + gameObject.name + " | ID: " + GetInstanceID());
        VerificarResposta(0);
    }

    public void EscolherB()
    {
        Debug.Log("CLICOU B neste objeto: " + gameObject.name + " | ID: " + GetInstanceID());
        VerificarResposta(1);
    }

    public void EscolherC()
    {
        Debug.Log("CLICOU C neste objeto: " + gameObject.name + " | ID: " + GetInstanceID());
        VerificarResposta(2);
    }

    public void EscolherD()
    {
        Debug.Log("CLICOU D neste objeto: " + gameObject.name + " | ID: " + GetInstanceID());
        VerificarResposta(3);
    }

    void VerificarResposta(int indice)
    {
        Debug.Log($"Indice clicado = {indice} | indice correto = {respostaCorreta}");

        if (indice == respostaCorreta)
        {
            acertosFeitos++;
            Debug.Log($"Acertou! ({acertosFeitos}/{acertosNecessarios})");
            audioSource.PlayOneShot(somCerto);
            respostasRegistradas.Add(new RespostaData { pergunta = textoPergunta.text, opcaoSelecionada = GetOpcaoTexto(indice), opcaoCorreta = GetOpcaoTexto(respostaCorreta), acertou = true });

            certoSimbolo.gameObject.SetActive(true);
            certoSimbolo.DOFade(0, 0.8f).OnComplete(() =>
            {
                certoSimbolo.gameObject.SetActive(false);
                certoSimbolo.color = Color.white;
            });

            bool deveFechar = chefaoAtual == null || acertosFeitos >= acertosNecessarios;
            if (deveFechar)
            {
                chefaoAtual = null;
                Flash(new Color(0, 1, 0, 0.8f), true);
                player.Acertou();
            }
            else
            {
                Flash(new Color(0, 1, 0, 0.8f), false);
                GerarOperacao();
            }
        }
        else
        {
            Debug.Log("Errou");
            respostasRegistradas.Add(new RespostaData { pergunta = textoPergunta.text, opcaoSelecionada = GetOpcaoTexto(indice), opcaoCorreta = GetOpcaoTexto(respostaCorreta), acertou = false });
            Flash(new Color(1, 0, 0, 0.8f), false);
            player.ErrarVida();
            GerarOperacao();

            audioSource.PlayOneShot(somErrado);

            erradoSimbolo.gameObject.SetActive(true);
            erradoSimbolo.DOFade(0, 0.8f).OnComplete(() =>
            {
                erradoSimbolo.gameObject.SetActive(false);
                erradoSimbolo.color = Color.white; // reseta alpha pra proxima vez
            });
        }
    }

    string GetOpcaoTexto(int indice)
    {
        switch (indice)
        {
            case 0: return textoA.text;
            case 1: return textoB.text;
            case 2: return textoC.text;
            case 3: return textoD.text;
            default: return "";
        }
    }

    public List<RespostaData> GetRespostas()
    {
        return respostasRegistradas;
    }

    void Flash(Color cor, bool fechar)
    {
        flashPanel.DOKill();
        cor.a = 0.8f;
        flashPanel.color = cor;
        flashPanel.DOFade(0, 0.8f).OnComplete(() => {
            if (fechar) gameObject.SetActive(false);
        });
    }
}