using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

/// <summary>
/// Mostra o que mais pesa no ultimo build (sem precisar buildar de novo).
/// Resultado no Console e em Builds/relatorio-assets.txt.
/// Use pelo menu: Ferramentas > Relatorio do Ultimo Build
/// </summary>
public static class RelatorioBuild
{
    [MenuItem("Ferramentas/Relatorio do Ultimo Build")]
    static void Executar()
    {
        BuildReport rel = BuildReport.GetLatestReport();
        if (rel == null)
        {
            Debug.LogWarning("[RelatorioBuild] Nenhum build encontrado.");
            return;
        }

        Dictionary<string, ulong> porAsset = new Dictionary<string, ulong>();
        foreach (PackedAssets pacote in rel.packedAssets)
            foreach (PackedAssetInfo info in pacote.contents)
            {
                string caminho = string.IsNullOrEmpty(info.sourceAssetPath) ? "(interno da Unity)" : info.sourceAssetPath;
                porAsset.TryGetValue(caminho, out ulong atual);
                porAsset[caminho] = atual + info.packedSize;
            }

        ulong total = 0;
        foreach (ulong v in porAsset.Values) total += v;

        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"Total dentro do .data (sem compressao): {Mb(total)} MB");

        sb.AppendLine("\nPor tipo de arquivo:");
        foreach (var grupo in porAsset.GroupBy(p => Tipo(p.Key))
                                      .Select(g => new { tipo = g.Key, soma = g.Aggregate(0UL, (s, p) => s + p.Value), qtd = g.Count() })
                                      .OrderByDescending(g => g.soma))
            sb.AppendLine($"  {Mb(grupo.soma),8} MB  {grupo.tipo} ({grupo.qtd})");

        sb.AppendLine("\n60 maiores assets:");
        foreach (var p in porAsset.OrderByDescending(p => p.Value).Take(60))
            sb.AppendLine($"  {Mb(p.Value),8} MB  {p.Key}");

        Directory.CreateDirectory("Builds");
        File.WriteAllText("Builds/relatorio-assets.txt", sb.ToString());
        Debug.Log("[RelatorioBuild]\n" + sb);
    }

    static string Tipo(string caminho)
    {
        string ext = Path.GetExtension(caminho).ToLowerInvariant();
        switch (ext)
        {
            case ".png": case ".jpg": case ".jpeg": case ".tga": case ".psd": case ".tif": case ".tiff": case ".exr": case ".hdr":
                return "Texturas";
            case ".fbx": case ".obj": case ".blend":
                return "Modelos 3D";
            case ".wav": case ".mp3": case ".ogg":
                return "Audio";
            case ".asset":
                return caminho.Contains("Terrain") ? "Terrenos" : "Assets (.asset)";
            case ".anim": case ".controller":
                return "Animacoes";
            case ".shader": case ".shadergraph": case ".compute":
                return "Shaders";
            case ".unity":
                return "Cenas (lightmaps, objetos)";
            default:
                return ext == "" ? caminho : ext;
        }
    }

    static string Mb(ulong bytes) => (bytes / (1024f * 1024f)).ToString("0.0");
}
