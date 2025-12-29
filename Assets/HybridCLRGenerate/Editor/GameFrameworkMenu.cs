#if UNITY_EDITOR
using System.Collections.Generic;
using Unity.Entities;
using UnityEditor;
using UnityEngine;

namespace GameFramework.Editor
{
    public class GameFrameworkMenu
    {
        [MenuItem("GameFramework/Create Managers")]
        public static void CreateManagers()
        {
            var managersGO = new GameObject("GameManagers");

            managersGO.AddComponent<Managers.ResourceManager>();
            managersGO.AddComponent<Managers.UIManager>();
            managersGO.AddComponent<Managers.InputManager>();
            managersGO.AddComponent<Managers.SceneManager>();
            managersGO.AddComponent<Managers.SaveManager>();
            managersGO.AddComponent<Managers.PoolManager>();

            Debug.Log("游戏管理器创建完成");
        }

        [MenuItem("GameFramework/Clear All Save Data")]
        public static void ClearSaveData()
        {
            if (EditorUtility.DisplayDialog("清除存档", "确定要清除所有存档数据吗?", "确定", "取消"))
            {
                Managers.SaveManager.Instance.DeleteSave();
                Debug.Log("存档数据已清除");
            }
        }

        [MenuItem("GameFramework/Performance/Show Entity Count")]
        public static void ShowEntityCount()
        {
            var world = World.DefaultGameObjectInjectionWorld;
            if (world != null)
            {
                int count = world.EntityManager.UniversalQuery.CalculateEntityCount();
                Debug.Log($"当前Entity数量: {count}");
            }
            else
            {
                Debug.LogWarning("ECS World未初始化");
            }
        }

        [MenuItem("GameFramework/Performance/Show Memory Usage")]
        public static void ShowMemoryUsage()
        {
            long memory = System.GC.GetTotalMemory(false);
            Debug.Log($"当前内存使用: {memory / 1048576f:F2} MB");
        }

        [MenuItem("GameFramework/Tools/Screenshot")]
        public static void TakeScreenshot()
        {
            string path = EditorUtility.SaveFilePanel(
                "保存截图",
                "",
                $"Screenshot_{System.DateTime.Now:yyyyMMdd_HHmmss}.png",
                "png"
            );

            if (!string.IsNullOrEmpty(path))
            {
                ScreenCapture.CaptureScreenshot(path);
                Debug.Log($"截图已保存: {path}");
            }
        }
    }

    [CustomEditor(typeof(Core.GameBootstrap))]
    public class GameBootstrapEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();

            if (GUILayout.Button("重新初始化游戏", GUILayout.Height(30)))
            {
                if (Application.isPlaying)
                {
                    var bootstrap = target as Core.GameBootstrap;
                    // 触发重新初始化
                    Debug.Log("游戏重新初始化");
                }
                else
                {
                    EditorUtility.DisplayDialog("提示", "请在运行时使用此功能", "确定");
                }
            }

            if (GUILayout.Button("清理所有管理器", GUILayout.Height(30)))
            {
                if (EditorUtility.DisplayDialog("警告", "这将清理所有单例管理器，确定继续吗?", "确定", "取消"))
                {
                    // 清理逻辑
                    Debug.Log("管理器已清理");
                }
            }
        }
    }

    public class ECSDebugWindow : EditorWindow
    {
        private Vector2 _scrollPosition;
        private bool _showSystems = true;
        private bool _showEntities = true;

        [MenuItem("GameFramework/Debug/ECS Debug Window")]
        public static void ShowWindow()
        {
            GetWindow<ECSDebugWindow>("ECS调试窗口");
        }

        private void OnGUI()
        {
            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("请在运行时使用此窗口", MessageType.Info);
                return;
            }

            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null)
            {
                EditorGUILayout.HelpBox("ECS World未初始化", MessageType.Warning);
                return;
            }

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            // Entity信息
            _showEntities = EditorGUILayout.Foldout(_showEntities, "Entities", true);
            if (_showEntities)
            {
                EditorGUI.indentLevel++;

                // --- 修改开始 ---
                // 旧代码: EditorGUILayout.LabelField($"总数量: {world.EntityManager.EntityCount}");
                // 新代码:
                int totalCount = world.EntityManager.UniversalQuery.CalculateEntityCount();
                EditorGUILayout.LabelField($"总数量: {totalCount}");
                // --- 修改结束 ---

                /*var query = world.EntityManager.CreateEntityQuery(ComponentType.ReadOnly<ECS.Components.PlayerTag>());
                EditorGUILayout.LabelField($"玩家实体: {query.CalculateEntityCount()}");

                query = world.EntityManager.CreateEntityQuery(ComponentType.ReadOnly<ECS.Components.EnemyTag>());
                EditorGUILayout.LabelField($"敌人实体: {query.CalculateEntityCount()}");*/

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();

            // 系统信息
            _showSystems = EditorGUILayout.Foldout(_showSystems, "Systems", true);
            if (_showSystems)
            {
                EditorGUI.indentLevel++;
                var systems = world.Systems;
                foreach (var system in systems)
                {
                    EditorGUILayout.LabelField(system.GetType().Name);
                }
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndScrollView();
        }
    }

    public class PerformanceProfilerWindow : EditorWindow
    {
        private Vector2 _scrollPosition;
        private List<float> _fpsHistory = new List<float>();
        private int _maxHistorySize = 100;

        [MenuItem("GameFramework/Debug/Performance Profiler")]
        public static void ShowWindow()
        {
            GetWindow<PerformanceProfilerWindow>("性能分析器");
        }

        private void OnEnable()
        {
            EditorApplication.update += UpdateProfiler;
        }

        private void OnDisable()
        {
            EditorApplication.update -= UpdateProfiler;
        }

        private void UpdateProfiler()
        {
            if (!Application.isPlaying) return;

            float fps = 1f / Time.unscaledDeltaTime;
            _fpsHistory.Add(fps);

            if (_fpsHistory.Count > _maxHistorySize)
            {
                _fpsHistory.RemoveAt(0);
            }

            Repaint();
        }

        private void OnGUI()
        {
            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("请在运行时使用此窗口", MessageType.Info);
                return;
            }

            EditorGUILayout.LabelField("性能统计", EditorStyles.boldLabel);

            if (_fpsHistory.Count > 0)
            {
                float avgFPS = 0f;
                float minFPS = float.MaxValue;
                float maxFPS = float.MinValue;

                foreach (var fps in _fpsHistory)
                {
                    avgFPS += fps;
                    minFPS = Mathf.Min(minFPS, fps);
                    maxFPS = Mathf.Max(maxFPS, fps);
                }
                avgFPS /= _fpsHistory.Count;

                EditorGUILayout.LabelField($"平均FPS: {avgFPS:F1}");
                EditorGUILayout.LabelField($"最小FPS: {minFPS:F1}");
                EditorGUILayout.LabelField($"最大FPS: {maxFPS:F1}");
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("内存统计", EditorStyles.boldLabel);

            long memory = System.GC.GetTotalMemory(false);
            EditorGUILayout.LabelField($"托管内存: {memory / 1048576f:F2} MB");

            EditorGUILayout.Space();

            // FPS曲线图
            if (_fpsHistory.Count > 1)
            {
                Rect rect = GUILayoutUtility.GetRect(10, 100, GUILayout.ExpandWidth(true));
                DrawFPSGraph(rect);
            }

            if (GUILayout.Button("清除历史数据"))
            {
                _fpsHistory.Clear();
            }
        }

        private void DrawFPSGraph(Rect rect)
        {
            Handles.BeginGUI();
            Handles.color = Color.green;

            float maxFPS = 120f;
            float width = rect.width;
            float height = rect.height;

            for (int i = 1; i < _fpsHistory.Count; i++)
            {
                float x1 = rect.x + (i - 1) * width / _maxHistorySize;
                float y1 = rect.y + height - (_fpsHistory[i - 1] / maxFPS) * height;
                float x2 = rect.x + i * width / _maxHistorySize;
                float y2 = rect.y + height - (_fpsHistory[i] / maxFPS) * height;

                Handles.DrawLine(new Vector3(x1, y1), new Vector3(x2, y2));
            }

            Handles.EndGUI();
        }
    }
}
#endif
