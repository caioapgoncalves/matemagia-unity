using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Liga a Mesh Compression (aba Model do Inspector) nos modelos 3D usados nas cenas do build,
/// para o build WebGL caber no limite do itch.io. Modelos acima de 1 MB recebem "High",
/// os menores "Medium". Tambem liga "Optimize Mesh Data" no Player Settings.
/// Use pelo menu: Ferramentas > Comprimir Modelos 3D
/// </summary>
public static class ComprimirModelos
{
    [MenuItem("Ferramentas/Comprimir Modelos 3D")]
    static void Executar()
    {
        string[] cenas = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();
        string[] modelos = AssetDatabase.GetDependencies(cenas, true)
            .Where(p => p.EndsWith(".fbx", System.StringComparison.OrdinalIgnoreCase)
                     || p.EndsWith(".obj", System.StringComparison.OrdinalIgnoreCase))
            .ToArray();

        int alterados = 0;
        AssetDatabase.StartAssetEditing();
        try
        {
            foreach (string caminho in modelos)
            {
                ModelImporter imp = AssetImporter.GetAtPath(caminho) as ModelImporter;
                if (imp == null) continue;

                long tamanho = new FileInfo(caminho).Length;
                ModelImporterMeshCompression alvo = tamanho > 1024 * 1024
                    ? ModelImporterMeshCompression.High
                    : ModelImporterMeshCompression.Medium;

                if (imp.meshCompression == alvo) continue;
                imp.meshCompression = alvo;
                imp.SaveAndReimport();
                alterados++;
            }
        }
        finally
        {
            AssetDatabase.StopAssetEditing();
        }

        PlayerSettings.stripUnusedMeshComponents = true;
        AssetDatabase.SaveAssets();

        Debug.Log($"[ComprimirModelos] {alterados} de {modelos.Length} modelos com Mesh Compression ligada.");
    }
}
