using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq; // 引入 Linq 方便处理列表

public class FBXPrefabPacker : EditorWindow
{
    // 默认源路径 (拆解工具输出的根目录)
    private string sourceRootPath = "Assets/ExtractedAssets";
    // 默认输出路径 (生成的 Prefab 存放位置)
    private string outputPrefabPath = "Assets/GamePrefabs";

    [MenuItem("Tools/FBX Prefab Packer (Auto Match)")]
    public static void ShowWindow()
    {
        GetWindow<FBXPrefabPacker>("Prefab Packer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Prefab 自动组装工具 (适配新材质规则)", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // 1. 源文件夹设置
        GUILayout.Label("资源源目录 (包含 building, vehicle 等子文件夹)", EditorStyles.label);
        EditorGUILayout.BeginHorizontal();
        sourceRootPath = EditorGUILayout.TextField("", sourceRootPath);
        if (GUILayout.Button("选择", GUILayout.Width(60)))
        {
            string s = EditorUtility.OpenFolderPanel("选择资源根目录", "Assets", "");
            if (!string.IsNullOrEmpty(s)) sourceRootPath = FileUtil.GetProjectRelativePath(s);
        }
        EditorGUILayout.EndHorizontal();

        // 2. 输出文件夹设置
        GUILayout.Label("Prefab 输出目录", EditorStyles.label);
        EditorGUILayout.BeginHorizontal();
        outputPrefabPath = EditorGUILayout.TextField("", outputPrefabPath);
        if (GUILayout.Button("选择", GUILayout.Width(60)))
        {
            string s = EditorUtility.OpenFolderPanel("选择Prefab输出目录", "Assets", "");
            if (!string.IsNullOrEmpty(s)) outputPrefabPath = FileUtil.GetProjectRelativePath(s);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox(
            "逻辑说明:\n" +
            "1. 扫描 Assets/ExtractedAssets/{Category}/Meshes\n" +
            "2. 提取 ID: Model_Type_Name\n" +
            "3. 识别层级: 文件名含 '_main' 为父节点，'_sub' 为子节点\n" +
            "4. 材质匹配: 自动在 {Category}/Materials 中寻找名称最匹配的材质。",
            MessageType.Info);

        if (GUILayout.Button("开始组装 Prefabs", GUILayout.Height(40)))
        {
            StartPacking();
        }
    }

    private void StartPacking()
    {
        if (string.IsNullOrEmpty(sourceRootPath) || string.IsNullOrEmpty(outputPrefabPath)) return;

        if (!Directory.Exists(outputPrefabPath)) Directory.CreateDirectory(outputPrefabPath);

        // 获取源目录下的分类文件夹 (如 building)
        string[] subDirectories = Directory.GetDirectories(sourceRootPath);

        foreach (string folderPath in subDirectories)
        {
            string categoryName = Path.GetFileName(folderPath);
            if (categoryName.StartsWith(".")) continue;

            PackCategoryFolder(FixPath(folderPath), categoryName);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("完成", $"Prefab 已打包至: {outputPrefabPath}", "确定");
    }

    // 定义一个结构来存储解析后的文件信息
    private class MeshFileInfo
    {
        public string fullPath;
        public string assetName; // 文件名 (不含后缀)
        public bool isMain;
        public bool isSub;
    }

    private void PackCategoryFolder(string categorySourcePath, string categoryName)
    {
        string meshPath = Path.Combine(categorySourcePath, "Meshes");
        string matPath = Path.Combine(categorySourcePath, "Materials");

        if (!Directory.Exists(meshPath)) return;

        // 1. 预加载该分类下的所有材质，供后续匹配使用
        List<Material> categoryMaterials = LoadAllMaterials(matPath);

        // 2. 扫描所有 Mesh
        string[] meshGuids = AssetDatabase.FindAssets("t:Mesh", new[] { meshPath });

        // 字典: Key = 模型唯一标识ID (Model_Type_Name), Value = 该组所有的Mesh文件信息
        Dictionary<string, List<MeshFileInfo>> modelGroups = new Dictionary<string, List<MeshFileInfo>>();

        foreach (string guid in meshGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string fileName = Path.GetFileNameWithoutExtension(path);

            // 解析 ID 和 Main/Sub 标记
            string modelID = ExtractModelID(fileName, out bool isMain, out bool isSub);

            if (!modelGroups.ContainsKey(modelID))
            {
                modelGroups[modelID] = new List<MeshFileInfo>();
            }

            modelGroups[modelID].Add(new MeshFileInfo
            {
                fullPath = path,
                assetName = fileName,
                isMain = isMain,
                isSub = isSub
            });
        }

        // 3. 分组处理生成 Prefab
        int current = 0;
        foreach (var kvp in modelGroups)
        {
            string modelID = kvp.Key;
            List<MeshFileInfo> files = kvp.Value;

            EditorUtility.DisplayProgressBar($"处理分类: {categoryName}", $"组装: {modelID}", (float)current / modelGroups.Count);

            // 输出路径: Assets/GamePrefabs/building/
            string finalOutFolder = Path.Combine(outputPrefabPath, categoryName);
            if (!Directory.Exists(finalOutFolder)) Directory.CreateDirectory(finalOutFolder);

            CreatePrefabWithHierarchy(modelID, files, categoryMaterials, finalOutFolder);
            current++;
        }
        EditorUtility.ClearProgressBar();
    }

    // --- ID 解析逻辑 (适配 Model_Type_Name_MeshName) ---
    private string ExtractModelID(string fileName, out bool isMain, out bool isSub)
    {
        string lowerName = fileName.ToLower();
        isMain = lowerName.Contains("_main");
        isSub = lowerName.Contains("_sub");

        string[] parts = fileName.Split('_');

        // 假设标准格式: Model_Type_Name_...
        // 无论后面接了多少个 MeshName片段，前三段一定是 ID
        if (parts.Length >= 3 && parts[0].Equals("Model", System.StringComparison.OrdinalIgnoreCase))
        {
            // 返回 "Model_Type_Name"
            return string.Join("_", parts, 0, 3);
        }

        // 如果格式不对，回退到去掉最后一段
        int lastIdx = fileName.LastIndexOf('_');
        return lastIdx > 0 ? fileName.Substring(0, lastIdx) : fileName;
    }

    // --- 核心组装逻辑 ---
    private void CreatePrefabWithHierarchy(string modelID, List<MeshFileInfo> files, List<Material> availableMats, string outputFolder)
    {
        // 1. 创建最外层 Root
        GameObject rootGO = new GameObject(modelID);

        // 2. 寻找 Main 节点
        MeshFileInfo mainInfo = files.Find(x => x.isMain);
        Transform anchorTransform = rootGO.transform;

        if (mainInfo != null)
        {
            GameObject mainGO = CreateObjectFromMesh(mainInfo, availableMats);
            // 名字清理: 去掉 ModelID 前缀，去掉 _main
            mainGO.name = CleanName(mainInfo.assetName, modelID).Replace("main", "").Trim();
            if (string.IsNullOrEmpty(mainGO.name)) mainGO.name = "MainNode";

            mainGO.transform.SetParent(rootGO.transform);
            mainGO.transform.localPosition = Vector3.zero;

            anchorTransform = mainGO.transform;
            files.Remove(mainInfo);
        }

        // 3. 处理剩余 Mesh
        foreach (var fileInfo in files)
        {
            GameObject go = CreateObjectFromMesh(fileInfo, availableMats);

            // 名字清理
            go.name = CleanName(fileInfo.assetName, modelID).Replace("sub", "").Trim();

            // 设置层级
            if (fileInfo.isSub)
                go.transform.SetParent(anchorTransform); // 挂在 Main 下
            else
                go.transform.SetParent(rootGO.transform); // 挂在 Root 下

            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;
        }

        // 4. 保存 Prefab
        string finalPath = Path.Combine(outputFolder, modelID + ".prefab");
        finalPath = FixPath(finalPath);

        PrefabUtility.SaveAsPrefabAsset(rootGO, finalPath);
        DestroyImmediate(rootGO);
    }

    private GameObject CreateObjectFromMesh(MeshFileInfo info, List<Material> availableMats)
    {
        Mesh mesh = AssetDatabase.LoadAssetAtPath<Mesh>(info.fullPath);
        if (mesh == null) return new GameObject(info.assetName + "_Error");

        GameObject go = new GameObject(mesh.name); // 初始名，后续会改
        MeshFilter mf = go.AddComponent<MeshFilter>();
        mf.sharedMesh = mesh;
        MeshRenderer mr = go.AddComponent<MeshRenderer>();

        // --- 核心修改：智能材质匹配 ---
        Material bestMatch = FindBestMatchMaterial(mesh.name, info.assetName, availableMats);
        if (bestMatch != null)
        {
            mr.sharedMaterial = bestMatch;
        }

        return go;
    }

    // --- 智能材质匹配逻辑 ---
    private Material FindBestMatchMaterial(string meshName, string assetFileName, List<Material> mats)
    {
        if (mats == null || mats.Count == 0) return null;

        // 策略 0: 如果该文件夹下只有一个材质，直接用它 (大概率是对的)
        if (mats.Count == 1) return mats[0];

        string meshLower = meshName.ToLower();
        string fileLower = assetFileName.ToLower();

        // 策略 1: 完全包含匹配 (Mesh名字里包含材质关键名，或者材质名包含Mesh名)
        // 材质名格式通常是: Material_RenCaiZhongXin
        // 我们去掉 "Material_" 前缀来比对
        foreach (var mat in mats)
        {
            string matKey = mat.name.Replace("Material_", "").ToLower();

            // 检查: 材质关键字 是否出现在 Mesh名 或 文件名 中
            // 例如 Mesh: "Wall_01", Material Key: "Wall" -> 匹配
            if (meshLower.Contains(matKey) || fileLower.Contains(matKey))
            {
                return mat;
            }

            // 反向检查: Mesh名 是否出现在 材质名 中
            if (matKey.Contains(meshLower))
            {
                return mat;
            }
        }

        // 策略 2: 如果实在匹配不到，默认返回第一个，或者返回 null 让其显示粉色(以便发现错误)
        // 这里为了方便，如果没匹配到，暂不赋值
        return null;
    }

    private List<Material> LoadAllMaterials(string path)
    {
        List<Material> list = new List<Material>();
        if (!Directory.Exists(path)) return list;

        string[] guids = AssetDatabase.FindAssets("t:Material", new[] { path });
        foreach (var g in guids)
        {
            list.Add(AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(g)));
        }
        return list;
    }

    private string CleanName(string fullName, string modelID)
    {
        // 移除 ID 前缀，移除下划线
        string s = fullName.Replace(modelID, "");
        // 移除开头可能残留的下划线
        if (s.StartsWith("_")) s = s.Substring(1);
        return s;
    }

    private string FixPath(string path)
    {
        if (string.IsNullOrEmpty(path)) return "";
        return path.Replace("\\", "/");
    }
}