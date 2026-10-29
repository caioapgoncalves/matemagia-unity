using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

/// <summary>
/// Gera o build WebGL para o itch.io em Builds/itch (Brotli) e confere os limites
/// do itch.io (500 MB no total, 200 MB por arquivo, 1000 arquivos).
/// O resumo sai no Console e em Builds/relatorio-build.txt.
/// Use pelo menu: Ferramentas > Build WebGL (itch.io)
/// </summary>
public static class BuildItch
{
    const string pastaBuild = "Builds/itch";
    const string relatorio = "Builds/relatorio-build.txt";
    const long limiteTotal = 500L * 1024 * 1024;
    const long limiteArquivo = 200L * 1024 * 1024;

    [MenuItem("Ferramentas/Build WebGL (itch.io)")]
    static void Executar()
    {
        Directory.CreateDirectory("Builds");
        if (File.Exists(relatorio)) File.Delete(relatorio);

        PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Brotli;
        PlayerSettings.WebGL.decompressionFallback = false;

        string[] cenas = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();

        BuildPlayerOptions opcoes = new BuildPlayerOptions
        {
            scenes = cenas,
            locationPathName = pastaBuild,
            target = BuildTarget.WebGL,
            options = BuildOptions.None
        };

        BuildReport resultado = BuildPipeline.BuildPlayer(opcoes);

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Resultado: " + resultado.summary.result);
        sb.AppendLine("Cenas: " + string.Join(", ", cenas.Select(Path.GetFileNameWithoutExtension)));

        if (resultado.summary.result == BuildResult.Succeeded && Directory.Exists(pastaBuild))
        {
            AjustarIndexParaItch(Path.Combine(pastaBuild, "index.html"));

            FileInfo[] arquivos = new DirectoryInfo(pastaBuild).GetFiles("*", SearchOption.AllDirectories);
            long total = arquivos.Sum(a => a.Length);

            sb.AppendLine($"Total: {Mb(total)} MB em {arquivos.Length} arquivos (limite itch.io: 500 MB / 1000 arquivos)");
            sb.AppendLine("Maiores arquivos:");
            foreach (FileInfo a in arquivos.OrderByDescending(a => a.Length).Take(8))
                sb.AppendLine($"  {Mb(a.Length),8} MB  {a.FullName.Substring(Path.GetFullPath(pastaBuild).Length + 1)}");

            bool ok = total <= limiteTotal && arquivos.Length <= 1000 && arquivos.All(a => a.Length <= limiteArquivo);
            sb.AppendLine(ok ? "DENTRO dos limites do itch.io." : "ATENCAO: passou de algum limite do itch.io.");
        }
        else
        {
            sb.AppendLine("Erros: " + resultado.summary.totalErrors);
        }

        File.WriteAllText(relatorio, sb.ToString());
        Debug.Log("[BuildItch]\n" + sb);
    }

    // O modelo padrao da Unity fixa a tela em 1920x1080 px; no iframe do itch.io (1280x720)
    // isso corta as bordas. Aqui o jogo passa a ocupar a janela inteira.
    static void AjustarIndexParaItch(string caminho)
    {
        if (!File.Exists(caminho)) return;
        string html = File.ReadAllText(caminho);
        html = System.Text.RegularExpressions.Regex.Replace(html,
            @"canvas\.style\.width = ""\d+px"";\s*canvas\.style\.height = ""\d+px"";",
            "document.querySelector(\"#unity-container\").className = \"unity-mobile\";\n        canvas.className = \"unity-mobile\";");
        html = System.Text.RegularExpressions.Regex.Replace(html, @"<title>.*?</title>", "<title>MateMagia</title>");
        File.WriteAllText(caminho, html);
    }

    static string Mb(long bytes) => (bytes / (1024f * 1024f)).ToString("0.0");
}
