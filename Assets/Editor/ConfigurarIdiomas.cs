using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Prepara o jogo para PT / EN / ES:
/// - Menu: coloca TextoTraduzido nos textos, esconde os acentos soltos fora do portugues
///   e cria os botoes PT / EN / ES no canto superior direito.
/// - Fases: preenche os nomes das fases e o aviso do chefe em ingles e espanhol.
/// - PausePainel (prefab): traduz os textos do painel de pausa.
/// Depois e so ajustar pelo Inspector. Pode rodar de novo sem duplicar nada.
/// Use pelo menu: Ferramentas > Configurar Idiomas
/// </summary>
public static class ConfigurarIdiomas
{
    const string cenaMenu = "Assets/Scenes/Menu.unity";
    const string prefabPause = "Assets/Prefabs/personagems/PREFABPARAINIMIGOS/prefabmenuepause/PausePainel.prefab";
    static readonly string[] cenasFases =
    {
        "Assets/Scenes/Adicao.unity",
        "Assets/Scenes/Subtracao.unity",
        "Assets/Scenes/Multiplicacao.unity",
        "Assets/Scenes/Divisao.unity",
    };

    // texto em portugues -> { ingles, espanhol }
    static readonly Dictionary<string, string[]> textos = new Dictionary<string, string[]>
    {
        { "Modo Historia", new[] { "Story Mode", "Modo Historia" } },
        { "Fases", new[] { "Levels", "Niveles" } },
        { "Sobre", new[] { "About", "Acerca de" } },
        { "Sair", new[] { "Exit", "Salir" } },
        { "Adicao", new[] { "Addition", "Suma" } },
        { "Subtracao", new[] { "Subtraction", "Resta" } },
        { "Multiplicacao", new[] { "Multiplication", "Multiplicacion" } },
        { "Divisao", new[] { "Division", "Division" } },
        { "Escolha seu desafio e complete a fase 3 vezes !", new[] {
            "Choose your challenge and complete the level 3 times!",
            "Elige tu desafio y completa la fase 3 veces!" } },
        { "Jogo Pausado.", new[] { "Game Paused.", "Juego Pausado." } },
        { "Voltar ao jogo", new[] { "Back to game", "Volver al juego" } },
        { "Voltar ao Menu", new[] { "Back to Menu", "Volver al Menu" } },
    };

    static readonly string sobreIngles =
        "Matemagia is an educational game created to make learning math more fun.\n\n\n" +
        "During the adventure, you will travel through different places and face addition, subtraction, " +
        "division and multiplication challenges. To defeat the enemies and move through the levels, " +
        "you will need to correctly solve the operations shown.\n\n\n" +
        "The game was developed to help children practice basic math operations in an interactive way, " +
        "using challenges, characters and worlds full of magic.\n\n\n" +
        "Get ready to learn, explore and show all your math power!\n";

    static readonly string sobreEspanhol =
        "Matemagia es un juego educativo creado para que aprender matematicas sea mas divertido.\n\n\n" +
        "Durante la aventura, pasaras por diferentes escenarios y enfrentaras desafios de suma, resta, " +
        "division y multiplicacion. Para derrotar a los enemigos y avanzar por las fases, sera necesario " +
        "resolver correctamente las operaciones presentadas.\n\n\n" +
        "El juego fue desarrollado para ayudar a los estudiantes a practicar las operaciones matematicas " +
        "basicas de forma interactiva, usando desafios, personajes y mundos llenos de magia.\n\n\n" +
        "Preparate para aprender, explorar y mostrar todo tu poder matematico!\n";

    // nome da fase em portugues -> { ingles, espanhol }
    static readonly Dictionary<string, string[]> nomesFases = new Dictionary<string, string[]>
    {
        { "Floresta Somaris", new[] { "Somaris Forest", "Bosque Somaris" } },
        { "Deserto de Meenos", new[] { "Meenos Desert", "Desierto de Meenos" } },
        { "Galaxia V-zis", new[] { "V-zis Galaxy", "Galaxia V-zis" } },
        { "Antigo Templo Dyvi Zan", new[] { "Ancient Temple of Dyvi Zan", "Antiguo Templo Dyvi Zan" } },
        { "Prepare-se... o Chefe se aproxima!", new[] { "Get ready... the Boss is coming!", "Preparate... el Jefe se acerca!" } },
    };

    static string log;

    [MenuItem("Ferramentas/Configurar Idiomas")]
    static void Executar()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        string cenaOriginal = SceneManager.GetActiveScene().path;
        log = "";

        ConfigurarMenu();
        foreach (string cena in cenasFases) ConfigurarFase(cena);
        ConfigurarPause();

        if (!string.IsNullOrEmpty(cenaOriginal))
            EditorSceneManager.OpenScene(cenaOriginal, OpenSceneMode.Single);

        Debug.Log("[ConfigurarIdiomas] Concluido:" + log);
    }

    // ── Menu ──

    static void ConfigurarMenu()
    {
        Scene cena = EditorSceneManager.OpenScene(cenaMenu, OpenSceneMode.Single);

        // 1) botoes de idioma (antes de traduzir, pra copia do botao nao levar traducao junto)
        CriarBotoesIdioma(cena);

        // 2) textos e acentos
        int traduzidos = 0, acentos = 0;
        foreach (TMP_Text texto in TodosOsTextos(cena))
        {
            string pt = texto.text;
            string chave = pt.Trim();

            if (chave == "s" || chave == "l" || chave == "v")
            {
                if (texto.GetComponent<SomenteEmPortugues>() == null)
                    texto.gameObject.AddComponent<SomenteEmPortugues>();
                acentos++;
                continue;
            }

            if (chave.StartsWith("O Matemagia"))
            {
                Traduzir(texto, pt, sobreIngles, sobreEspanhol);
                traduzidos++;
                continue;
            }

            if (textos.TryGetValue(chave, out string[] t))
            {
                string sufixo = pt.Substring(pt.TrimEnd().Length); // mantem as quebras de linha do final
                Traduzir(texto, pt, t[0] + sufixo, t[1] + sufixo);
                traduzidos++;
            }
        }

        EditorSceneManager.MarkSceneDirty(cena);
        EditorSceneManager.SaveScene(cena);
        log += $"\nMenu: {traduzidos} textos traduzidos, {acentos} acentos so em portugues, botoes PT/EN/ES.";
    }

    static void CriarBotoesIdioma(Scene cena)
    {
        Transform painel = AcharPorNome(cena, "PainelPrincipal");
        Transform modelo = painel != null ? AcharPorNome(painel, "SobreButton") : null;
        if (painel == null || modelo == null)
        {
            log += "\nMenu: nao achei PainelPrincipal/SobreButton, botoes de idioma nao criados.";
            return;
        }

        Transform grupo = painel.Find("Idiomas");
        if (grupo == null)
        {
            GameObject go = new GameObject("Idiomas", typeof(RectTransform));
            grupo = go.transform;
            grupo.SetParent(painel, false);
            RectTransform rtGrupo = (RectTransform)grupo;
            rtGrupo.anchorMin = rtGrupo.anchorMax = rtGrupo.pivot = new Vector2(1, 1);
            rtGrupo.anchoredPosition = new Vector2(-30, -30);
            rtGrupo.sizeDelta = new Vector2(380, 70);
        }

        SeletorIdioma seletor = grupo.GetComponent<SeletorIdioma>();
        if (seletor == null) seletor = grupo.gameObject.AddComponent<SeletorIdioma>();

        string[] rotulos = { "PT", "EN", "ES" };
        UnityAction[] acoes = { seletor.Portugues, seletor.Ingles, seletor.Espanhol };
        string[] campos = { "botaoPortugues", "botaoIngles", "botaoEspanhol" };
        SerializedObject soSeletor = new SerializedObject(seletor);

        for (int i = 0; i < rotulos.Length; i++)
        {
            string nome = "Botao" + rotulos[i];
            Transform botao = grupo.Find(nome);
            if (botao == null)
            {
                botao = Object.Instantiate(modelo.gameObject, grupo, false).transform;
                botao.name = nome;
            }

            RectTransform rt = (RectTransform)botao;
            rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(1, 1);
            rt.localScale = Vector3.one * 2f;
            rt.sizeDelta = new Vector2(60, 35);
            rt.anchoredPosition = new Vector2(-(rotulos.Length - 1 - i) * 130, 0);

            foreach (TextoTraduzido tt in botao.GetComponentsInChildren<TextoTraduzido>(true))
                Object.DestroyImmediate(tt);
            TMP_Text rotulo = botao.GetComponentInChildren<TMP_Text>(true);
            if (rotulo != null) rotulo.text = rotulos[i];

            Button b = botao.GetComponent<Button>();
            if (b != null)
            {
                b.onClick = new Button.ButtonClickedEvent();
                UnityEventTools.AddVoidPersistentListener(b.onClick, acoes[i]);
            }

            CanvasGroup grupoCanvas = botao.GetComponent<CanvasGroup>();
            if (grupoCanvas == null) grupoCanvas = botao.gameObject.AddComponent<CanvasGroup>();
            soSeletor.FindProperty(campos[i]).objectReferenceValue = grupoCanvas;
        }

        soSeletor.ApplyModifiedProperties();
    }

    // ── Fases ──

    static void ConfigurarFase(string caminho)
    {
        Scene cena = EditorSceneManager.OpenScene(caminho, OpenSceneMode.Single);
        string nomeCena = cena.name;
        string feito = "";

        foreach (NomeFase nf in Componentes<NomeFase>(cena))
            if (PreencherCampos(nf, "nomeDaFase", "nomeDaFaseIngles", "nomeDaFaseEspanhol")) feito += " nome da fase;";

        foreach (AvisoBoss ab in Componentes<AvisoBoss>(cena))
            if (PreencherCampos(ab, "textoDoAviso", "textoDoAvisoIngles", "textoDoAvisoEspanhol")) feito += " aviso do chefe;";

        EditorSceneManager.MarkSceneDirty(cena);
        EditorSceneManager.SaveScene(cena);
        log += $"\n{nomeCena}:{(feito == "" ? " nada para traduzir" : feito)}";
    }

    static bool PreencherCampos(Object alvo, string campoPt, string campoEn, string campoEs)
    {
        SerializedObject so = new SerializedObject(alvo);
        string pt = so.FindProperty(campoPt).stringValue;
        if (!nomesFases.TryGetValue(pt.Trim(), out string[] t)) return false;
        so.FindProperty(campoEn).stringValue = t[0];
        so.FindProperty(campoEs).stringValue = t[1];
        so.ApplyModifiedProperties();
        return true;
    }

    // ── Painel de pausa (prefab) ──

    static void ConfigurarPause()
    {
        GameObject raiz = PrefabUtility.LoadPrefabContents(prefabPause);
        int n = 0;
        foreach (TMP_Text texto in raiz.GetComponentsInChildren<TMP_Text>(true))
        {
            string pt = texto.text;
            if (textos.TryGetValue(pt.Trim(), out string[] t))
            {
                Traduzir(texto, pt, t[0], t[1]);
                n++;
            }
        }
        PrefabUtility.SaveAsPrefabAsset(raiz, prefabPause);
        PrefabUtility.UnloadPrefabContents(raiz);
        log += $"\nPausePainel: {n} textos traduzidos.";
    }

    // ── Utilitarios ──

    static void Traduzir(TMP_Text texto, string pt, string en, string es)
    {
        TextoTraduzido tt = texto.GetComponent<TextoTraduzido>();
        if (tt == null) tt = texto.gameObject.AddComponent<TextoTraduzido>();
        tt.portugues = pt;
        tt.ingles = en;
        tt.espanhol = es;
        EditorUtility.SetDirty(tt);
    }

    static List<TMP_Text> TodosOsTextos(Scene cena) => Componentes<TMP_Text>(cena);

    static List<T> Componentes<T>(Scene cena) where T : Component
    {
        List<T> lista = new List<T>();
        foreach (GameObject raiz in cena.GetRootGameObjects())
            lista.AddRange(raiz.GetComponentsInChildren<T>(true));
        return lista;
    }

    static Transform AcharPorNome(Scene cena, string nome)
    {
        foreach (GameObject raiz in cena.GetRootGameObjects())
        {
            Transform t = AcharPorNome(raiz.transform, nome);
            if (t != null) return t;
        }
        return null;
    }

    static Transform AcharPorNome(Transform raiz, string nome)
    {
        foreach (Transform t in raiz.GetComponentsInChildren<Transform>(true))
            if (t.name == nome) return t;
        return null;
    }
}
