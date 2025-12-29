#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace GameFramework.Editor
{
    public class ProjectSetupChecker : EditorWindow
    {
        private Vector2 _scrollPosition;
        private List<SetupItem> _setupItems = new List<SetupItem>();

        [MenuItem("GameFramework/Project Setup Checker")]
        public static void ShowWindow()
        {
            GetWindow<ProjectSetupChecker>("项目设置检查");
        }

        private void OnEnable()
        {
            CheckSetup();
        }

        private void CheckSetup()
        {
            _setupItems.Clear();

            // 检查必需的包
            CheckPackage("com.unity.entities", "Unity Entities");
            CheckPackage("com.unity.entities.graphics", "Unity Entities Graphics");
            CheckPackage("com.unity.burst", "Burst Compiler");
            CheckPackage("com.unity.addressables", "Addressables");
            CheckPackage("com.unity.inputsystem", "Input System");

            // 检查场景
            CheckScene("BootScene");
            CheckScene("MainMenu");
            CheckScene("GameScene");

            // 检查资源
            CheckAsset("Assets/Resources/GameConfig.asset", "GameConfig");
            CheckAsset("Assets/AddressableAssets", "Addressables目录");

            // 检查脚本
            CheckScript("GameBootstrap");
            CheckScript("GameStateManager");
            CheckScript("ResourceManager");
        }

        private void CheckPackage(string packageName, string displayName)
        {
            var request = UnityEditor.PackageManager.Client.List();
            while (!request.IsCompleted) { }

            bool found = false;
            if (request.Status == UnityEditor.PackageManager.StatusCode.Success)
            {
                foreach (var package in request.Result)
                {
                    if (package.name == packageName)
                    {
                        found = true;
                        break;
                    }
                }
            }

            _setupItems.Add(new SetupItem
            {
                name = displayName,
                status = found ? SetupStatus.OK : SetupStatus.Missing,
                message = found ? "已安装" : "未安装"
            });
        }

        private void CheckScene(string sceneName)
        {
            bool found = false;
            foreach (var scene in EditorBuildSettings.scenes)
            {
                if (scene.path.Contains(sceneName))
                {
                    found = true;
                    break;
                }
            }

            _setupItems.Add(new SetupItem
            {
                name = $"场景: {sceneName}",
                status = found ? SetupStatus.OK : SetupStatus.Warning,
                message = found ? "已添加到构建设置" : "未添加到构建设置"
            });
        }

        private void CheckAsset(string path, string displayName)
        {
            bool exists = System.IO.File.Exists(path) || System.IO.Directory.Exists(path);

            _setupItems.Add(new SetupItem
            {
                name = displayName,
                status = exists ? SetupStatus.OK : SetupStatus.Warning,
                message = exists ? "存在" : "不存在"
            });
        }

        private void CheckScript(string scriptName)
        {
            var guids = AssetDatabase.FindAssets($"t:Script {scriptName}");
            bool found = guids.Length > 0;

            _setupItems.Add(new SetupItem
            {
                name = $"脚本: {scriptName}",
                status = found ? SetupStatus.OK : SetupStatus.Missing,
                message = found ? "找到" : "未找到"
            });
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("项目设置检查", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            if (GUILayout.Button("重新检查", GUILayout.Height(30)))
            {
                CheckSetup();
            }

            EditorGUILayout.Space();

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            foreach (var item in _setupItems)
            {
                EditorGUILayout.BeginHorizontal();

                // 状态图标
                Color iconColor = item.status == SetupStatus.OK ? Color.green :
                                 item.status == SetupStatus.Warning ? Color.yellow : Color.red;

                GUI.color = iconColor;
                GUILayout.Label("●", GUILayout.Width(20));
                GUI.color = Color.white;

                // 名称和消息
                EditorGUILayout.LabelField(item.name, GUILayout.Width(200));
                EditorGUILayout.LabelField(item.message);

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();

            // 统计
            EditorGUILayout.Space();
            int okCount = _setupItems.FindAll(i => i.status == SetupStatus.OK).Count;
            int warningCount = _setupItems.FindAll(i => i.status == SetupStatus.Warning).Count;
            int errorCount = _setupItems.FindAll(i => i.status == SetupStatus.Missing).Count;

            EditorGUILayout.LabelField($"✓ 正常: {okCount}  ⚠ 警告: {warningCount}  ✗ 错误: {errorCount}");
        }

        private enum SetupStatus
        {
            OK,
            Warning,
            Missing
        }

        private class SetupItem
        {
            public string name;
            public SetupStatus status;
            public string message;
        }
    }
}
#endif
