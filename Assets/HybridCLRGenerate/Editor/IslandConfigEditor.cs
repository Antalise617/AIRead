using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using OfficeOpenXml; // 必须引入 EPPlus 命名空间

public class IslandConfigEditor : EditorWindow
{
    // 数据结构定义
    private class IslandData
    {
        public int id;
        public string name;
        public int length;
        public int width;
        public HashSet<Vector2Int> buildArea = new HashSet<Vector2Int>();
        public HashSet<Vector2Int> attachmentPoints = new HashSet<Vector2Int>();
        public int rowIndex; // 记录该数据在Excel中的行号，方便回写
    }

    private List<IslandData> loadedIslands = new List<IslandData>();
    private int selectedIslandIndex = 0;
    private string excelPath = "";

    // 编辑器状态
    private enum EditMode { BuildArea, AttachmentPoint }
    private EditMode currentMode = EditMode.BuildArea;
    private Vector2 scrollPos;

    [MenuItem("Tools/岛屿Excel配置编辑器")]
    public static void ShowWindow()
    {
        var window = GetWindow<IslandConfigEditor>("岛屿配置");
        window.minSize = new Vector2(400, 400);
    }

    private void OnGUI()
    {
        GUILayout.Label("岛屿数据编辑器", EditorStyles.boldLabel);

        // 1. 文件加载区域
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("加载 Excel 文件", GUILayout.Width(120)))
        {
            LoadExcel(); // EPPlus读取速度很快，可以直接调用
        }
        GUILayout.Label(string.IsNullOrEmpty(excelPath) ? "未加载文件" : Path.GetFileName(excelPath));
        EditorGUILayout.EndHorizontal();

        DrawHorizontalLine();

        if (loadedIslands.Count == 0)
        {
            GUILayout.Label("请先加载 .xlsx 文件以开始编辑。\n注意：需要 EPPlus.dll 插件支持。", EditorStyles.centeredGreyMiniLabel);
            return;
        }

        // 2. 岛屿选择区域
        if (selectedIslandIndex >= loadedIslands.Count) selectedIslandIndex = 0;

        string[] islandNames = loadedIslands.Select(x => $"{x.id} - {x.name}").ToArray();
        int newIndex = EditorGUILayout.Popup("选择岛屿", selectedIslandIndex, islandNames);
        if (newIndex != selectedIslandIndex)
        {
            selectedIslandIndex = newIndex;
            GUI.FocusControl(null);
        }

        IslandData currentIsland = loadedIslands[selectedIslandIndex];

        // 3. 信息显示与模式切换
        EditorGUILayout.Space();
        GUILayout.Label($"尺寸: {currentIsland.length} x {currentIsland.width} (Excel行号: {currentIsland.rowIndex})", EditorStyles.miniLabel);

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("当前编辑模式:", GUILayout.Width(80));

        string[] modes = new string[] { "可建造区 (绿色)", "附着点 (黄色)" };
        int modeIndex = (currentMode == EditMode.BuildArea) ? 0 : 1;
        int newModeIndex = GUILayout.Toolbar(modeIndex, modes);
        currentMode = (newModeIndex == 0) ? EditMode.BuildArea : EditMode.AttachmentPoint;

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        // 4. 网格绘制区域
        DrawGrid(currentIsland);

        EditorGUILayout.Space();
        DrawHorizontalLine();

        // 5. 数据保存区域
        GUILayout.Label("数据保存", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        // 红色醒目提示
        GUI.backgroundColor = new Color(0.6f, 1f, 0.6f);
        if (GUILayout.Button("保存当前岛屿修改到 Excel", GUILayout.Height(40)))
        {
            if (EditorUtility.DisplayDialog("确认保存", $"确定要将 ID:{currentIsland.id} 的修改写入 Excel 吗？\n请确保 Excel 文件已关闭，否则无法保存。", "确定写入", "取消"))
            {
                SaveExcel(currentIsland);
            }
        }
        GUI.backgroundColor = Color.white;
        EditorGUILayout.EndHorizontal();

        GUILayout.Label("注意：保存时必须关闭 Excel 软件，否则会被占用。", EditorStyles.miniLabel);
    }

    private void DrawGrid(IslandData island)
    {
        if (island.length <= 0 || island.width <= 0) return;

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUIStyle.none, GUI.skin.verticalScrollbar);

        float cellSize = 30f;
        float spacing = 2f;
        float totalGridWidth = island.length * (cellSize + spacing);
        float startX = Mathf.Max(0, (position.width - totalGridWidth) / 2 - 20);

        EditorGUILayout.BeginVertical();

        // Y轴倒序绘制
        for (int y = island.width - 1; y >= 0; y--)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(startX);

            for (int x = 0; x < island.length; x++)
            {
                Vector2Int pos = new Vector2Int(x, y);

                bool isBuild = island.buildArea.Contains(pos);
                bool isAttach = island.attachmentPoints.Contains(pos);

                Color targetColor = Color.gray;
                if (isBuild && isAttach) targetColor = Color.cyan;
                else if (isBuild) targetColor = Color.green;
                else if (isAttach) targetColor = new Color(1f, 0.92f, 0.016f);

                GUI.backgroundColor = targetColor;

                if (GUILayout.Button($"{x},{y}", GUILayout.Width(cellSize), GUILayout.Height(cellSize)))
                {
                    OnGridClick(island, pos);
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();

        GUI.backgroundColor = Color.white;
        EditorGUILayout.EndScrollView();

        EditorGUILayout.BeginHorizontal("box");
        GUILayout.Label("图例:", GUILayout.Width(40));
        DrawColorBox(Color.green, "可建造");
        DrawColorBox(new Color(1f, 0.92f, 0.016f), "附着点");
        DrawColorBox(Color.cyan, "重叠区域");
        EditorGUILayout.EndHorizontal();
    }

    private void DrawColorBox(Color c, string label)
    {
        Color old = GUI.color;
        GUI.color = c;
        GUILayout.Label("■", GUILayout.Width(20));
        GUI.color = old;
        GUILayout.Label(label, GUILayout.Width(60));
    }

    private void OnGridClick(IslandData island, Vector2Int pos)
    {
        if (currentMode == EditMode.BuildArea)
        {
            if (island.buildArea.Contains(pos)) island.buildArea.Remove(pos);
            else island.buildArea.Add(pos);
        }
        else if (currentMode == EditMode.AttachmentPoint)
        {
            if (island.attachmentPoints.Contains(pos)) island.attachmentPoints.Remove(pos);
            else island.attachmentPoints.Add(pos);
        }
        Repaint();
    }

    private void LoadExcel()
    {
        string path = EditorUtility.OpenFilePanel("Open Island Config", "", "xlsx");
        if (string.IsNullOrEmpty(path)) return;

        excelPath = path;
        List<IslandData> newIslands = new List<IslandData>();

        try
        {
            FileInfo fileInfo = new FileInfo(path);

            using (ExcelPackage package = new ExcelPackage(fileInfo))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[1];

                int rowCount = worksheet.Dimension.Rows;
                int colCount = worksheet.Dimension.Columns;

                Debug.Log($"[EPPlus] 打开文件，共 {rowCount} 行");

                for (int row = 1; row <= rowCount; row++)
                {
                    string firstCol = worksheet.Cells[row, 1].Text;
                    if (!string.IsNullOrEmpty(firstCol) && firstCol.StartsWith("##")) continue;

                    if (colCount < 10) continue;

                    string idStr = worksheet.Cells[row, 2].Text;
                    if (!int.TryParse(idStr, out int id)) continue;

                    try
                    {
                        IslandData data = new IslandData();
                        data.id = id;
                        data.rowIndex = row;
                        data.name = worksheet.Cells[row, 3].Text;

                        int.TryParse(worksheet.Cells[row, 5].Text, out data.length);
                        int.TryParse(worksheet.Cells[row, 6].Text, out data.width);

                        data.buildArea = ParseCoordString(worksheet.Cells[row, 9].Text);
                        data.attachmentPoints = ParseCoordString(worksheet.Cells[row, 10].Text);

                        newIslands.Add(data);
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogWarning($"解析第 {row} 行失败: {ex.Message}");
                    }
                }
            }

            loadedIslands = newIslands;
            selectedIslandIndex = 0;

            if (loadedIslands.Count > 0)
                Debug.Log($"<color=green>成功加载 {loadedIslands.Count} 个岛屿 (EPPlus)</color>");
            else
                Debug.LogError("未找到有效数据，请检查 Excel 列位置是否正确。");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"读取失败: {e.Message}");
        }
        Repaint();
    }

    // --- 使用 EPPlus 写入 Excel ---
    private void SaveExcel(IslandData data)
    {
        if (string.IsNullOrEmpty(excelPath)) return;

        try
        {
            FileInfo fileInfo = new FileInfo(excelPath);

            // 写入时需要注意：如果文件被 Excel 打开，这里会报错 IOException
            using (ExcelPackage package = new ExcelPackage(fileInfo))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[1]; // 获取第一个Sheet

                int row = data.rowIndex;

                // 双重校验：确保我们要写的这一行的 ID 还是原来的 ID，防止行号乱了
                string currentIdInFile = worksheet.Cells[row, 2].Text;
                if (currentIdInFile != data.id.ToString())
                {
                    EditorUtility.DisplayDialog("错误", $"文件结构可能已变更！\n第 {row} 行的 ID 是 {currentIdInFile}，但我们想写的是 {data.id}。\n请重新加载文件。", "确定");
                    return;
                }

                // 生成字符串
                string buildAreaStr = GenerateDataString(data.buildArea);
                string attachStr = GenerateDataString(data.attachmentPoints);

                // 写入单元格 (第9列和第10列)
                worksheet.Cells[row, 9].Value = buildAreaStr;
                worksheet.Cells[row, 10].Value = attachStr;

                // 保存文件
                package.Save();
            }

            Debug.Log($"<color=cyan>保存成功！ID: {data.id}</color>");
            EditorUtility.DisplayDialog("成功", "数据已成功写入 Excel 文件！", "确定");

            // 重新加载一次以刷新数据状态
            LoadExcel();
        }
        catch (IOException)
        {
            EditorUtility.DisplayDialog("保存失败", "无法写入文件！\n请检查 Excel 是否正在运行。\n请关闭 Excel 软件后重试。", "确定");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"保存出错: {e.Message}");
            EditorUtility.DisplayDialog("错误", $"保存出错:\n{e.Message}", "确定");
        }
    }

    private HashSet<Vector2Int> ParseCoordString(string str)
    {
        HashSet<Vector2Int> set = new HashSet<Vector2Int>();
        if (string.IsNullOrWhiteSpace(str) || str.Trim() == "0") return set;
        str = str.Replace("\"", "").Trim();
        string[] points = str.Split('|');
        foreach (var p in points)
        {
            string[] xy = p.Split(';');
            if (xy.Length == 2)
            {
                if (int.TryParse(xy[0], out int x) && int.TryParse(xy[1], out int y))
                    set.Add(new Vector2Int(x, y));
            }
        }
        return set;
    }

    private string GenerateDataString(HashSet<Vector2Int> coords)
    {
        if (coords.Count == 0) return "0";
        var sorted = coords.OrderBy(v => v.x).ThenBy(v => v.y).ToList();
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < sorted.Count; i++)
        {
            sb.Append($"{sorted[i].x};{sorted[i].y}");
            if (i < sorted.Count - 1) sb.Append("|");
        }
        return sb.ToString();
    }

    private void DrawHorizontalLine()
    {
        GUILayout.Space(5);
        EditorGUI.DrawRect(GUILayoutUtility.GetRect(100, 1), new Color(0.5f, 0.5f, 0.5f, 1));
        GUILayout.Space(5);
    }
}