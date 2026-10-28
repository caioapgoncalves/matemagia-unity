using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Deixa as 4 fases iguais: 3 rodadas (totalVoltas) e figuras de apoio na conta.
/// Copia os containers FigurasA/FigurasB e a posicao do texto da conta da cena Adicao
/// para as outras fases e define a imagem de cada uma. Pode rodar de novo sem duplicar nada.
/// Use pelo menu: Ferramentas > Igualar Fases
/// </summary>
public static class IgualarFases
{
    const int rodadas = 3;

    static readonly string[,] fases =
    {
        { "Assets/Scenes/Adicao.unity",        "Assets/Prefabs/cogumelo.png" },
        { "Assets/Scenes/Subtracao.unity",     "Assets/Prefabs/cactu.png" },
        { "Assets/Scenes/Multiplicacao.unity", "Assets/Prefabs/AMETISTA.png" },
        { "Assets/Scenes/Divisao.unity",       "Assets/Prefabs/coqueiro.png" },
    };

    [MenuItem("Ferramentas/Igualar Fases")]
    static void Executar()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        string cenaOriginal = SceneManager.GetActiveScene().path;

        // Cena modelo (Adicao) fica aberta o tempo todo
        Scene modelo = EditorSceneManager.OpenScene(fases[0, 0], OpenSceneMode.Single);
        QuizIntermed quizModelo = Achar<QuizIntermed>(modelo);
        SerializedObject soModelo = new SerializedObject(quizModelo);
        Transform modeloA = soModelo.FindProperty("containerFigurasA").objectReferenceValue as Transform;
        Transform modeloB = soModelo.FindProperty("containerFigurasB").objectReferenceValue as Transform;
        Object prefabFigura = soModelo.FindProperty("prefabFigura").objectReferenceValue;
        Component perguntaModelo = soModelo.FindProperty("textoPergunta").objectReferenceValue as Component;
        RectTransform rectPerguntaModelo = perguntaModelo != null ? perguntaModelo.transform as RectTransform : null;

        if (modeloA == null || modeloB == null || prefabFigura == null)
        {
            Debug.LogError("[IgualarFases] Na cena Adicao, o QuizIntermed precisa ter FigurasA, FigurasB e o prefab Figura preenchidos.");
            return;
        }

        string log = "";

        for (int i = 0; i < fases.GetLength(0); i++)
        {
            string caminhoCena = fases[i, 0];
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(fases[i, 1]);
            if (sprite == null)
                Debug.LogWarning("[IgualarFases] Imagem nao encontrada como Sprite: " + fases[i, 1]);

            bool ehModelo = i == 0;
            Scene cena = ehModelo ? modelo : EditorSceneManager.OpenScene(caminhoCena, OpenSceneMode.Additive);
            string nomeCena = cena.name;

            QuizIntermed quiz = Achar<QuizIntermed>(cena);
            AndarAutomatico player = Achar<AndarAutomatico>(cena);
            if (quiz == null || player == null)
            {
                log += $"\n{nomeCena}: QuizIntermed ou AndarAutomatico nao encontrado, pulei.";
                if (!ehModelo) EditorSceneManager.CloseScene(cena, true);
                continue;
            }

            SerializedObject so = new SerializedObject(quiz);

            if (!ehModelo)
            {
                Transform pai = AcharPorNome(quiz.transform.root, modeloA.parent.name);
                if (pai == null)
                {
                    log += $"\n{nomeCena}: nao achei o objeto '{modeloA.parent.name}', pulei.";
                    EditorSceneManager.CloseScene(cena, true);
                    continue;
                }

                Transform a = Copiar(modeloA, pai);
                Transform b = Copiar(modeloB, pai);
                so.FindProperty("containerFigurasA").objectReferenceValue = a;
                so.FindProperty("containerFigurasB").objectReferenceValue = b;
                so.FindProperty("prefabFigura").objectReferenceValue = prefabFigura;

                // Texto da conta na mesma posicao/escala da Adicao, pra alinhar com as figuras
                Component pergunta = so.FindProperty("textoPergunta").objectReferenceValue as Component;
                if (pergunta != null && rectPerguntaModelo != null)
                {
                    RectTransform rect = (RectTransform)pergunta.transform;
                    rect.anchoredPosition = rectPerguntaModelo.anchoredPosition;
                    rect.localScale = rectPerguntaModelo.localScale;
                    PrefabUtility.RecordPrefabInstancePropertyModifications(rect);
                }
            }

            so.FindProperty("usarFigurasVisuais").boolValue = true;
            so.FindProperty("spriteFigura").objectReferenceValue = sprite;
            so.FindProperty("numeroMaximo").intValue = 5;
            so.ApplyModifiedProperties();

            SerializedObject soPlayer = new SerializedObject(player);
            soPlayer.FindProperty("totalVoltas").intValue = rodadas;
            soPlayer.ApplyModifiedProperties();

            EditorSceneManager.MarkSceneDirty(cena);
            EditorSceneManager.SaveScene(cena);
            if (!ehModelo) EditorSceneManager.CloseScene(cena, true);

            log += $"\n{nomeCena}: {rodadas} rodadas, numeros ate 5, figura '{(sprite != null ? sprite.name : "padrao")}'.";
        }

        if (!string.IsNullOrEmpty(cenaOriginal) && cenaOriginal != fases[0, 0])
            EditorSceneManager.OpenScene(cenaOriginal, OpenSceneMode.Single);

        Debug.Log("[IgualarFases] Concluido:" + log);
    }

    // Cria (ou reaproveita, se ja existir) uma copia do container no mesmo lugar do modelo
    static Transform Copiar(Transform modelo, Transform pai)
    {
        Transform existente = pai.Find(modelo.name);
        if (existente != null) return existente;

        GameObject copia = Object.Instantiate(modelo.gameObject, pai, false);
        copia.name = modelo.name;
        copia.transform.SetSiblingIndex(Mathf.Min(modelo.GetSiblingIndex(), pai.childCount - 1));
        Undo.RegisterCreatedObjectUndo(copia, "Igualar Fases");
        return copia.transform;
    }

    static T Achar<T>(Scene cena) where T : Component
    {
        foreach (GameObject raiz in cena.GetRootGameObjects())
        {
            T c = raiz.GetComponentInChildren<T>(true);
            if (c != null) return c;
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
