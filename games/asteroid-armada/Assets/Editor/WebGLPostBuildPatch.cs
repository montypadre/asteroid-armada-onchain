using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
using UnityEngine.Timeline;

public class WebGLPostBuildPatch 
{
    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
    {
        if (target != BuildTarget.WebGL) return;

        string indexPath = Path.Combine(pathToBuiltProject, "index.html");
        string html = File.ReadAllText(indexPath);

        string marker = "}).then((unityInstance) => {";
        string injected = marker + "\n        window.unityInstance = unityInstance";

        if (html.Contains(marker) && !html.Contains("window.unityInstance"))
        {
            html = html.Replace(marker, injected);
            File.WriteAllText(indexPath, html);
            UnityEngine.Debug.Log("[WebGLPostBuildPatch] Injected window.unityInstance exposure into index.html");
        }
    }
}
