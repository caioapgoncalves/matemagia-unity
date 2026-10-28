using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// Otimizador de texturas em lote.
/// Coloque este arquivo em: Assets/Editor/OtimizadorTexturas.cs
/// Use pelo menu: Ferramentas > Otimizar Texturas
/// </summary>
public class OtimizadorTexturas : EditorWindow
{
    private int tamanhoMaximo = 512;
    private TextureImporterCompression compressao = TextureImporterCompression.Compressed;
    private bool usarCrunch = true;
    private int qualidadeCrunch = 50;
    private bool gerarMipmaps = true;
    private bool apenasSimular = true;

    private Vector2 scroll;
    private string resultado = "";

    private readonly int[] opcoesTamanho = { 128, 256, 512, 1024, 2048 };

    [MenuItem("Ferramentas/Otimizar Texturas")]
    public static void AbrirJanela()
    {
        GetWindow<OtimizadorTexturas>("Otimizar Texturas");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Otimizador de Texturas", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "Reduz o tamanho maximo e aplica compressao em todas as texturas do projeto.\n" +
            "Faca um commit no Git antes de aplicar.",
            MessageType.Info);

        EditorGUILayout.Space();

        int indiceAtual = System.Array.IndexOf(opcoesTamanho, tamanhoMaximo);
        if (indiceAtual < 0) indiceAtual = 2;
        string[] rotulos = new string[opcoesTamanho.Length];
        for (int i = 0; i < opcoesTamanho.Length; i++) rotulos[i] = opcoesTamanho[i].ToString();
        indiceAtual = EditorGUILayout.Popup("Tamanho maximo", indiceAtual, rotulos);
        tamanhoMaximo = opcoesTamanho[indiceAtual];

        compressao = (TextureImporterCompression)EditorGUILayout.EnumPopup("Compressao", compressao);
        usarCrunch = EditorGUILayout.Toggle("Usar Crunch", usarCrunch);

        if (usarCrunch)
            qualidadeCrunch = EditorGUILayout.IntSlider("Qualidade Crunch", qualidadeCrunch, 0, 100);

        gerarMipmaps = EditorGUILayout.Toggle("Gerar Mipmaps", gerarMipmaps);

        EditorGUILayout.Space();
        apenasSimular = EditorGUILayout.Toggle("Apenas simular (nao altera)", apenasSimular);

        EditorGUILayout.Space();

        GUI.backgroundColor = apenasSimular ? Color.white : new Color(1f, 0.6f, 0.6f);
        if (GUILayout.Button(apenasSimular ? "Simular" : "APLICAR nas texturas", GUILayout.Height(35)))
        {
            Processar();
        }
        GUI.backgroundColor = Color.white;

        EditorGUILayout.Space();

        if (!string.IsNullOrEmpty(resultado))
        {
            scroll = EditorGUILayout.BeginScrollView(scroll);
            EditorGUILayout.TextArea(resultado, GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();
        }
    }

    private void Processar()
    {
        string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { "Assets" });
        List<string> alteradas = new List<string>();
        int inalteradas = 0;

        try
        {
            AssetDatabase.StartAssetEditing();

            for (int i = 0; i < guids.Length; i++)
            {
                string caminho = AssetDatabase.GUIDToAssetPath(guids[i]);

                EditorUtility.DisplayProgressBar(
                    "Otimizando texturas",
                    caminho,
                    (float)i / guids.Length);

                TextureImporter importer = AssetImporter.GetAtPath(caminho) as TextureImporter;
                if (importer == null) continue;

                bool precisaMudar =
                    importer.maxTextureSize > tamanhoMaximo ||
                    importer.textureCompression != compressao ||
                    importer.crunchedCompression != usarCrunch ||
                    importer.mipmapEnabled != gerarMipmaps;

                if (!precisaMudar)
                {
                    inalteradas++;
                    continue;
                }

                alteradas.Add(caminho + "   (era " + importer.maxTextureSize + ")");

                if (apenasSimular) continue;

                if (importer.maxTextureSize > tamanhoMaximo)
                    importer.maxTextureSize = tamanhoMaximo;

                importer.textureCompression = compressao;
                importer.crunchedCompression = usarCrunch;

                if (usarCrunch)
                    importer.compressionQuality = qualidadeCrunch;

                importer.mipmapEnabled = gerarMipmaps;

                importer.SaveAndReimport();
            }
        }
        finally
        {
            AssetDatabase.StopAssetEditing();
            EditorUtility.ClearProgressBar();
        }

        if (!apenasSimular)
            AssetDatabase.Refresh();

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.AppendLine(apenasSimular ? "=== SIMULACAO (nada foi alterado) ===" : "=== APLICADO ===");
        sb.AppendLine("Total de texturas: " + guids.Length);
        sb.AppendLine("Afetadas: " + alteradas.Count);
        sb.AppendLine("Ja estavam ok: " + inalteradas);
        sb.AppendLine();

        foreach (string linha in alteradas)
            sb.AppendLine(linha);

        resultado = sb.ToString();
        Debug.Log(resultado);
    }
}