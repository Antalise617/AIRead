using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class FBXExtractorTool : EditorWindow
{
    // --- 设置 ---
    private string sourcePath = "Assets/";
    private string targetBasePath = "Assets/ExtractedAssets";

    // --- 枚举定义 ---
    public enum ShaderType
    {
        Standard, // 内置管线
        URP       // 通用渲染管线
    }

    // --- 开关与选项 ---
    private bool extractMeshes = true;
    private bool extractAnimations = true; // 包含 AnimationClip 和 Avatar
    private bool generateMaterials = true; // 基于贴图生成材质
    private ShaderType targetShaderType = ShaderType.Standard; // 默认使用 Standard

    // --- 命名常量 ---
    private const string MATERIAL_PREFIX = "Material_";        // 输出前缀

    [MenuItem("Tools/FBX Asset Extractor (Auto Material)")]
    public static void ShowWindow()
    {
        GetWindow<FBXExtractorTool>("FBX Tool");
    }

    private void OnGUI()
    {
        GUILayout.Label("FBX 定制提取工具 (Diff Texture -> Material)", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // 1. 路径设置
        EditorGUILayout.BeginHorizontal();
        sourcePath = EditorGUILayout.TextField("源文件夹", sourcePath);
        if (GUILayout.Button("选择", GUILayout.Width(60)))
        {
            string s = EditorUtility.OpenFolderPanel("选择源文件夹", "Assets", "");
            if (!string.IsNullOrEmpty(s)) sourcePath = FileUtil.GetProjectRelativePath(s);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        targetBasePath = EditorGUILayout.TextField("输出根目录", targetBasePath);
        if (GUILayout.Button("选择", GUILayout.Width(60)))
        {
            string s = EditorUtility.OpenFolderPanel("选择输出文件夹", "Assets", "");
            if (!string.IsNullOrEmpty(s)) targetBasePath = FileUtil.GetProjectRelativePath(s);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        // 2. 选项
        GUILayout.Label("提取规则", EditorStyles.boldLabel);
        extractMeshes = EditorGUILayout.Toggle("提取 Meshes", extractMeshes);
        extractAnimations = EditorGUILayout.Toggle("提取 Animations & Avatar", extractAnimations);

        EditorGUILayout.Space();
        GUILayout.Label("材质生成规则", EditorStyles.boldLabel);
        generateMaterials = EditorGUILayout.Toggle("根据 Diffuse 生成材质", generateMaterials);

        if (generateMaterials)
        {
            // --- Shader 类型选择 ---
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("目标材质管线:");
            targetShaderType = (ShaderType)EditorGUILayout.EnumPopup(targetShaderType);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.HelpBox(
                $"逻辑说明:\n1. 识别前缀: 'textures_diffuse_' 或 'texture_diffuse_'\n" +
                $"2. 材质名: 保留前缀后所有字段 (如 _3x3daoyu_sub)\n" +
                $"3. 输出: '{MATERIAL_PREFIX}3x3daoyu_sub'\n" +
                $"4. Shader: {(targetShaderType == ShaderType.URP ? "URP/Lit" : "Standard")}",
                MessageType.Info);
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("开始处理", GUILayout.Height(40)))
        {
            StartExtraction();
        }
    }

    private void StartExtraction()
    {
        if (string.IsNullOrEmpty(sourcePath) || string.IsNullOrEmpty(targetBasePath)) return;

        string[] guids = AssetDatabase.FindAssets("t:Model", new[] { sourcePath });
        int total = guids.Length;

        for (int i = 0; i < total; i++)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
            string fileName = Path.GetFileNameWithoutExtension(assetPath);

            // 1. 检查前缀 Model
            if (!fileName.StartsWith("Model", System.StringComparison.OrdinalIgnoreCase))
                continue;

            string[] nameParts = fileName.Split('_');

            // 2. 检查命名格式 Model_Type_Name
            if (nameParts.Length < 2)
            {
                Debug.LogWarning($"跳过文件 {fileName}: 命名格式不符合 Model_Type_Name");
                continue;
            }

            string categoryFolder = nameParts[1];

            EditorUtility.DisplayProgressBar("处理中...", fileName, (float)i / total);

            ProcessFBX(assetPath, fileName, categoryFolder);
        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("完成", "资源提取与材质生成完毕！", "确定");
    }

    private void ProcessFBX(string fbxPath, string fileName, string categoryFolder)
    {
        // 基础路径: Assets/ExtractedAssets/building
        string baseOutPath = Path.Combine(targetBasePath, categoryFolder);
        baseOutPath = FixPath(baseOutPath);

        // --- 1. Mesh 提取 ---
        if (extractMeshes)
        {
            ExtractAssets<Mesh>(fbxPath, fileName, baseOutPath, "Meshes", ".asset", (src) => {
                Mesh m = Instantiate(src);
                m.name = src.name;
                return m;
            });
        }

        // --- 2. Animations & Avatar (骨骼) 提取 ---
        if (extractAnimations)
        {
            string animPath = FixPath(Path.Combine(baseOutPath, "Animations"));

            // A. 提取 AnimationClip
            ExtractAssets<AnimationClip>(fbxPath, fileName, baseOutPath, "Animations", ".anim", (src) => {
                if (src.name.StartsWith("__preview__")) return null; // 过滤预览动画
                AnimationClip newClip = Instantiate(src);
                newClip.name = src.name;
                return newClip;
            });

            // B. 提取 Avatar (骨骼配置)
            Object[] assets = AssetDatabase.LoadAllAssetsAtPath(fbxPath);
            foreach (var asset in assets)
            {
                if (asset is Avatar avatar)
                {
                    Directory.CreateDirectory(animPath);
                    Avatar newAvatar = Instantiate(avatar);
                    newAvatar.name = avatar.name;
                    string finalPath = FixPath(Path.Combine(animPath, $"{fileName}_{avatar.name}.asset"));
                    AssetDatabase.DeleteAsset(finalPath);
                    AssetDatabase.CreateAsset(newAvatar, finalPath);
                }
            }
        }

        // --- 3. 材质生成 ---
        if (generateMaterials)
        {
            GenerateMaterialsFromDependencies(fbxPath, baseOutPath);
        }
    }

    private void GenerateMaterialsFromDependencies(string fbxPath, string baseOutPath)
    {
        string texOutPath = FixPath(Path.Combine(baseOutPath, "Textures"));
        string matOutPath = FixPath(Path.Combine(baseOutPath, "Materials"));

        Directory.CreateDirectory(texOutPath);
        Directory.CreateDirectory(matOutPath);

        // --- 核心修改开始 ---

        // 1. 获取 FBX 所在的目录 (例如 Assets/Model_island_kongdao)
        string fbxDirectory = Path.GetDirectoryName(fbxPath);

        // 2. 暴力搜索该目录(包含子目录)下的所有 Texture 类型资源
        // 这样即使贴图放在 Textures 子文件夹里也能被找到
        string[] texGuids = AssetDatabase.FindAssets("t:Texture", new[] { fbxDirectory });

        // 用于去重，防止同一个文件夹下多个 FBX 重复处理相同的贴图
        HashSet<string> processedPaths = new HashSet<string>();

        foreach (string guid in texGuids)
        {
            string texPath = AssetDatabase.GUIDToAssetPath(guid);

            // 跳过已经处理过的路径
            if (processedPaths.Contains(texPath)) continue;
            processedPaths.Add(texPath);

            // 跳过不是贴图的文件 (虽然 t:Texture 过滤了一次，但双重保险)
            if (texPath.EndsWith(".cs") || texPath.EndsWith(".mat")) continue;

            string texFileName = Path.GetFileNameWithoutExtension(texPath);
            string contentName = null;

            // --- 这里的逻辑保持不变，依然通过前缀识别 ---

            // 情况 1: 标准复数前缀 textures_diffuse_
            if (texFileName.StartsWith("textures_diffuse_", System.StringComparison.OrdinalIgnoreCase))
            {
                contentName = texFileName.Substring("textures_diffuse_".Length);
            }
            // 情况 2: 单数前缀 texture_diffuse_
            else if (texFileName.StartsWith("texture_diffuse_", System.StringComparison.OrdinalIgnoreCase))
            {
                contentName = texFileName.Substring("texture_diffuse_".Length);
            }

            // 如果匹配到了符合命名规范的贴图
            if (!string.IsNullOrEmpty(contentName))
            {
                // 3. 复制贴图
                string targetTexPath = FixPath(Path.Combine(texOutPath, Path.GetFileName(texPath)));

                // 检查目标是否存在，避免重复报错
                if (!File.Exists(targetTexPath))
                {
                    AssetDatabase.CopyAsset(texPath, targetTexPath);
                }

                // 4. 创建材质球
                string matName = $"{MATERIAL_PREFIX}{contentName}.mat";
                string targetMatPath = FixPath(Path.Combine(matOutPath, matName));

                // --- Shader 选择逻辑 (保持不变) ---
                string shaderName = "Standard";
                if (targetShaderType == ShaderType.URP)
                {
                    shaderName = "Universal Render Pipeline/Lit";
                }

                Shader targetShader = Shader.Find(shaderName);
                if (targetShader == null)
                {
                    Debug.LogWarning($"未找到 Shader: {shaderName}，回退到 Standard。");
                    targetShader = Shader.Find("Standard");
                }

                // 如果材质不存在，或者我们要覆盖更新
                Material newMat = AssetDatabase.LoadAssetAtPath<Material>(targetMatPath);
                if (newMat == null)
                {
                    newMat = new Material(targetShader);
                    AssetDatabase.CreateAsset(newMat, targetMatPath);
                }
                else
                {
                    newMat.shader = targetShader;
                }

                // 重新加载复制后的贴图引用
                Texture2D extractedTex = AssetDatabase.LoadAssetAtPath<Texture2D>(targetTexPath);

                if (extractedTex != null)
                {
                    newMat.mainTexture = extractedTex; // Standard & URP(Lit) 兼容 _MainTex

                    // URP 有时需要额外设置 _BaseMap
                    if (targetShaderType == ShaderType.URP)
                    {
                        if (newMat.HasProperty("_BaseMap"))
                            newMat.SetTexture("_BaseMap", extractedTex);
                    }

                    EditorUtility.SetDirty(newMat);
                }
                else
                {
                    Debug.LogError($"无法加载贴图: {targetTexPath}");
                }
            }
        }
        // --- 核心修改结束 ---
    }

    // --- 通用 Helper ---
    private void ExtractAssets<T>(string fbxPath, string fileName, string basePath, string subFolder, string ext, System.Func<T, Object> processor) where T : Object
    {
        string outPath = FixPath(Path.Combine(basePath, subFolder));
        Directory.CreateDirectory(outPath);

        Object[] assets = AssetDatabase.LoadAllAssetsAtPath(fbxPath);
        foreach (Object asset in assets)
        {
            if (asset is T typedAsset)
            {
                Object newAsset = processor(typedAsset);
                if (newAsset == null) continue;

                string finalPath = FixPath(Path.Combine(outPath, $"{fileName}_{typedAsset.name}{ext}"));

                AssetDatabase.DeleteAsset(finalPath);
                AssetDatabase.CreateAsset(newAsset, finalPath);
            }
        }
    }

    private string FixPath(string path)
    {
        if (string.IsNullOrEmpty(path)) return "";
        return path.Replace("\\", "/");
    }
}