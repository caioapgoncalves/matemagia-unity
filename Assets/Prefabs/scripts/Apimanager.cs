using UnityEngine;
using System.Runtime.InteropServices;
using System.Collections.Generic;

public class ApiManager : MonoBehaviour
{
    public static ApiManager instancia;

    [DllImport("__Internal")]
    private static extern void EnviarDesempenho(string json);

    [DllImport("__Internal")]
    private static extern void IniciarListenerMensagens();

    private void Awake()
    {
        if (instancia == null)
        {
            instancia = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        IniciarListenerMensagens();
#endif
    }

    public void Enviar(string fase, int acertos, int erros, int tempoSegundos, List<RespostaData> respostas)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        var dados = new DesempenhoData
        {
            fase = fase,
            acertos = acertos,
            erros = erros,
            tempoSegundos = tempoSegundos,
            respostas = respostas
        };
        string json = JsonUtility.ToJson(dados);
        EnviarDesempenho(json);
#else
        Debug.Log($"[ApiManager] Simulando envio: {fase} | Acertos: {acertos} | Erros: {erros} | Tempo: {tempoSegundos}s");
#endif
    }

    // Chamado via postMessage do React
    public void EnviarDadosParciais()
    {
        var andar = FindObjectOfType<AndarAutomatico>();
        if (andar != null)
            andar.EnviarDadosParciais();
    }
}

[System.Serializable]
public class RespostaData
{
    public string pergunta;
    public string opcaoSelecionada;
    public string opcaoCorreta;
    public bool acertou;
}

[System.Serializable]
public class DesempenhoData
{
    public string fase;
    public int acertos;
    public int erros;
    public int tempoSegundos;
    public List<RespostaData> respostas;
}
[System.Serializable]
public class RespostaDataList
{
    public List<RespostaData> items = new List<RespostaData>();
}