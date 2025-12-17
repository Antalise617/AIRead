// 确保项目引入了 Cinemachine，否则注释掉相关行
using Cinemachine;
using GameFramework.CameraSystem;
using GameFramework.ECS;
using GameFramework.ECS.Components;
using Unity.Entities;
using Unity.Mathematics;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using GameFramework.Gameplay;
using GameFramework.Managers;

namespace GameFramework.Examples
{
    /// <summary>
    /// 测试场景构建器 (ECS + Cinemachine 环境)
    /// </summary>
    public class TestSceneSetup : MonoBehaviour
    {
        // 公开方法，由 GameBootstrap 调用
        public void SetupTestScene()
        {
            Debug.Log("=== ECS High Performance Setup ===");

            CreateEnvironment();
            CreateCameraSystem(); // 包含初始化 CameraSync

            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var factory = new EntityFactory(entityManager);
            _ = UIManager.Instance.ShowPanelAsync<MainPanel>("MainPanel");
        }

        private void CreateEnvironment()
        {
            // 光照
            var lightGO = new GameObject("Directional Light");
            var light = lightGO.AddComponent<Light>();
            light.type = LightType.Directional;
            lightGO.transform.rotation = Quaternion.Euler(50, -30, 0);
        }

        private void CreateCameraSystem()
        {
            // 确保主摄像机存在
            var mainCamera = UnityEngine.Camera.main;
            if (mainCamera == null)
            {
                var cameraGO = new GameObject("Main Camera");
                cameraGO.tag = "MainCamera";
                mainCamera = cameraGO.AddComponent<UnityEngine.Camera>();
                cameraGO.AddComponent<AudioListener>();
            }
            // 添加虚拟摄像机模块
            if (mainCamera.GetComponent<CinemachineBrain>() == null)
            {
                mainCamera.gameObject.AddComponent<CinemachineBrain>();
            }
            // 创建虚拟摄像机
            var vcamGO = new GameObject("VirtualCamera");
            var vcam = vcamGO.AddComponent<CinemachineVirtualCamera>();
            vcam.m_Lens.FieldOfView = 60f;
            vcam.transform.rotation = Quaternion.Euler(45, 0, 0); // 45度俯视

            var transposer = vcam.AddCinemachineComponent<CinemachineTransposer>();
            transposer.m_FollowOffset = new Vector3(0, 20, -20); // 调整高一点，视野更广
            transposer.m_BindingMode = CinemachineTransposer.BindingMode.LockToTarget;
            vcam.AddCinemachineComponent<CinemachineHardLookAt>();
            // 默认是1，设为0表示由代码完全控制位置
            transposer.m_XDamping = 0f;
            transposer.m_YDamping = 0f;
            transposer.m_ZDamping = 0f;

            // 3. 确保 CameraController 存在 (保持原样...)
            if (CameraController.Instance == null)
            {
                var controllerGO = new GameObject("CameraController");
                controllerGO.AddComponent<CameraController>();
            }

            // 4. 创建自由摄像机焦点 (Strategy Camera Focus)
            var focusGO = new GameObject("CameraFocus_Strategy");

            // 设置初始位置 (比如地图中心)
            focusGO.transform.position = new Vector3(0, 0, 0);

            // 添加控制脚本
            var strategyController = focusGO.AddComponent<StrategyCameraController>();

            // 配置参数 (可选，也可以在 StrategyCameraController 脚本里设默认值)
            strategyController.moveSpeed = 30f;
            strategyController.minBounds = new Vector2(0, 0);
            strategyController.maxBounds = new Vector2(100, 100);

            // 5. 设置跟随
            // 让 CameraController 通知 Cinemachine 盯着这个焦点看
            CameraController.Instance.SetFollow(focusGO.transform);
        }

        // 辅助：获取内置资源（仅作兜底，实际应使用 AssetBundle 或 Addressables 加载的资源）
        private Mesh GetBuiltinMesh(PrimitiveType type)
        {
            GameObject temp = GameObject.CreatePrimitive(type);
            Mesh mesh = temp.GetComponent<MeshFilter>().sharedMesh;
            DestroyImmediate(temp);
            return mesh;
        }

        private Material GetBuiltinMaterial(Color color)
        {
            var mat = new Material(Shader.Find("Universal Render Pipeline/Lit")); // 假设是 URP
            mat.color = color;
            mat.enableInstancing = true; // 开启 GPU Instancing 关键！
            return mat;
        }
    }
}