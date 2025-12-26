using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using HybridCLR.Editor.Commands; // 引用 HybridCLR 编辑器命名空间

public class GameBuildTool
{
    // =========================================================
    // 配置路径
    // =========================================================

    // 最终 EXE 输出目录
    private static string BuildOutputDir => Path.Combine(Directory.GetParent(Application.dataPath).FullName, "BuildOut/Windows");

    // EXE 名字
    private static string ExeName = "GameClient.exe";

    // 热更 DLL 在 Assets 中的存放路径 (根据你之前的文件结构 Assets/Resources_moved/Codes)
    private static string HotfixCodeTargetDir = "Assets/Resources_moved/Codes";

    // 需要复制的热更程序集名称 (根据你的 GameHotfix.asmdef)
    private static string[] HotfixAssemblies = new string[] { "GameHotfix" };

    // =========================================================
    // 菜单入口
    // =========================================================

    [MenuItem("GameBuild/Windows/1. 一键打完整包 (代码+资源+EXE)")]
    public static void BuildFullWindows()
    {
        // 1. 编译热更代码并复制到工程
        if (!CompileAndCopyHotfixDlls(BuildTarget.StandaloneWindows64)) return;

        // 2. 打包 Addressables 资源
        BuildAddressables();

        // 3. 构建 Windows 播放器
        BuildWindowsPlayer();
    }

    [MenuItem("GameBuild/Windows/2. 单独打代码包 (HybridCLR -> Copy)")]
    public static void BuildCodeOnly()
    {
        CompileAndCopyHotfixDlls(BuildTarget.StandaloneWindows64);
        Debug.Log("<color=green>=== 热更代码编译并同步完成 ===</color>");
        Debug.Log("注意：如果你想让代码生效到包里，通常还需要重新打 Addressables (如果 DLL 是作为资源加载的)");
    }

    [MenuItem("GameBuild/Windows/3. 单独打资源包 (Addressables)")]
    public static void BuildAssetsOnly()
    {
        BuildAddressables();
    }

    [MenuItem("GameBuild/Windows/4. 仅构建 EXE (不重新打资源)")]
    public static void BuildExeOnly()
    {
        BuildWindowsPlayer();
    }

    // =========================================================
    // 具体实现逻辑
    // =========================================================

    /// <summary>
    /// 步骤一：编译 HybridCLR 代码并复制到资源目录
    /// </summary>
    private static bool CompileAndCopyHotfixDlls(BuildTarget target)
    {
        Debug.Log("=== 开始编译 HybridCLR 热更代码 ===");

        // 1. 触发 HybridCLR 编译
        CompileDllCommand.CompileDllActiveBuildTarget();

        // 2. 获取编译后的 DLL 路径
        // 注意：HybridCLR 的输出路径可能随版本变化，通常在 {项目根目录}/HybridCLRData/HotUpdateDlls/{平台}
        string projectRoot = Directory.GetParent(Application.dataPath).FullName;
        string hotfixSourceDir = Path.Combine(projectRoot, "HybridCLRData", "HotUpdateDlls", target.ToString());

        if (!Directory.Exists(hotfixSourceDir))
        {
            Debug.LogError($"[Build] 找不到 HybridCLR 输出目录: {hotfixSourceDir}，请先在 HybridCLR 菜单中生成一次。");
            return false;
        }

        // 3. 准备目标文件夹
        if (!Directory.Exists(HotfixCodeTargetDir))
        {
            Directory.CreateDirectory(HotfixCodeTargetDir);
        }

        // 4. 复制并重命名文件 (.dll -> .bytes)
        foreach (var assemblyName in HotfixAssemblies)
        {
            string srcPath = Path.Combine(hotfixSourceDir, $"{assemblyName}.dll");
            string destPath = Path.Combine(HotfixCodeTargetDir, $"{assemblyName}.dll.bytes");

            if (File.Exists(srcPath))
            {
                File.Copy(srcPath, destPath, true);
                Debug.Log($"[Copy] {srcPath} -> {destPath}");
            }
            else
            {
                Debug.LogError($"[Build] 缺失热更 DLL: {assemblyName}.dll");
                return false;
            }
        }

        // 5. 补充元数据 DLL (可选)
        // 如果使用了补充元数据 (AOTGenericReferences)，需要把 AOT 的 DLL 也拷过来
        // 通常建议把 AOTGenericReferences.cs 中的列表对应的 DLL 从 AssembliesPostIl2CppStrip 目录拷过来
        // 这里暂时略过，视你项目是否需要“元数据补充”而定

        // 6. 刷新资源，确保 Unity 识别到新的 bytes 文件
        AssetDatabase.Refresh();

        return true;
    }

    /// <summary>
    /// 步骤二：打包 Addressables
    /// </summary>
    private static void BuildAddressables()
    {
        Debug.Log("=== 开始构建 Addressables 资源 ===");

        // 清理旧构建 (可选，如果为了增量构建可以注释掉 Clean)
        // AddressableAssetSettings.CleanPlayerContent(AddressableAssetSettingsDefaultObject.Settings.ActivePlayerDataBuilder);

        // 执行构建
        AddressableAssetSettings.BuildPlayerContent();

        Debug.Log("<color=green>=== Addressables 构建完成 ===</color>");
    }

    /// <summary>
    /// 步骤三：构建 Windows Player
    /// </summary>
    private static void BuildWindowsPlayer()
    {
        Debug.Log("=== 开始构建 Windows EXE ===");

        // 确保输出目录存在
        if (!Directory.Exists(BuildOutputDir))
        {
            Directory.CreateDirectory(BuildOutputDir);
        }

        // 获取 Build Settings 中启用的场景
        string[] levels = new string[EditorBuildSettings.scenes.Length];
        for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
        {
            if (EditorBuildSettings.scenes[i].enabled)
            {
                levels[i] = EditorBuildSettings.scenes[i].path;
            }
        }

        // 配置 Build 选项
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = levels;
        buildPlayerOptions.locationPathName = Path.Combine(BuildOutputDir, ExeName);
        buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
        buildPlayerOptions.options = BuildOptions.None; // Release 包
        // buildPlayerOptions.options = BuildOptions.Development | BuildOptions.ConnectWithProfiler; // Debug 包

        // 执行构建
        UnityEditor.Build.Reporting.BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);

        // 检查结果
        if (report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
        {
            Debug.Log($"<color=green>=== 构建成功: {buildPlayerOptions.locationPathName} ({report.summary.totalSize / 1024 / 1024} MB) ===</color>");

            // 构建完成后自动打开文件夹
            EditorUtility.RevealInFinder(buildPlayerOptions.locationPathName);
        }
        else
        {
            Debug.LogError($"=== 构建失败: {report.summary.result} ===");
        }
    }
}