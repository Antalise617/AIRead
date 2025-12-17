using System.IO;
using HybridCLR.Editor;
using HybridCLR.Editor.Commands;
using UnityEditor;
using UnityEngine;

public class HybridCLRBuildProcessor
{
    [MenuItem("HybridCLR/Build/Copy HotFix DLLs to Assets")]
    public static void CopyHotUpdateDlls()
    {
        // 1. 执行 HybridCLR 生成 (生成 DLL)
        PrebuildCommand.GenerateAll();

        // 2. 定义目标平台
        BuildTarget target = EditorUserBuildSettings.activeBuildTarget;

        // 3. 获取 HotUpdate DLL 的生成路径
        string hotFixDllDir = SettingsUtil.GetHotUpdateDllsOutputDirByTarget(target);

        // 4. 定义复制的目标目录 (必须在 Addressables 管理范围内)
        // 假设你把热更资源放在 Assets/Resources_moved/Codes 下
        string targetDir = Application.dataPath + "/Resources_moved/Codes";
        if (!Directory.Exists(targetDir)) Directory.CreateDirectory(targetDir);

        // 5. 复制并改名 (DLL -> bytes)
        foreach (var dllName in SettingsUtil.HotUpdateAssemblyFilesExcludePreserved)
        {
            string sourcePath = Path.Combine(hotFixDllDir, dllName);
            string targetPath = Path.Combine(targetDir, dllName + ".bytes"); // 改后缀，否则 Unity 不识别

            File.Copy(sourcePath, targetPath, true);
            Debug.Log($"[HybridCLR] Copied: {sourcePath} -> {targetPath}");
        }

        // 6. 补充 AOT 元数据 DLL (用于泛型补充，推荐加上)
        // 需要在 HybridCLR Settings 里填入你需要补充元数据的 AOT 程序集名称 (如 mscorlib, System.Core 等)
        // 这里仅作演示，逻辑类似上面的复制，源路径在 SettingsUtil.GetAssembliesPostIl2CppStripDir(target)

        AssetDatabase.Refresh();
    }
}