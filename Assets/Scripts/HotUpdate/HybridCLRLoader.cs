using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.AddressableAssets;
using HybridCLR;

public class HybridCLRLoader : MonoBehaviour
{
    // 在 Inspector 中填写热更 DLL 的 Addressables 地址
    public List<string> hotUpdateDllAddresses = new List<string>()
    {
        "Assembly_Game.HotFix" // 对应你的 .bytes 文件地址
    };

    // 如果使用了 AOT 泛型补充，在这里填 AOT DLL 的地址 (例如 "Assembly_mscorlib")
    public List<string> aotMetadataDllAddresses = new List<string>();

    void Start()
    {
        StartCoroutine(LoadAndStartGame());
    }

    IEnumerator LoadAndStartGame()
    {
        Debug.Log("[HybridCLR] 开始加载资源系统...");

        // 1. 初始化 Addressables
        var initHandle = Addressables.InitializeAsync();
        yield return initHandle;

        // 2. 加载并补充 AOT 元数据 (Optional, 但强烈建议)
        // 这解决了 "AOT泛型实例化缺失" 的问题
        foreach (var addr in aotMetadataDllAddresses)
        {
            var handle = Addressables.LoadAssetAsync<TextAsset>(addr);
            yield return handle;
            byte[] dllBytes = handle.Result.bytes;

            // 加载元数据
            var err = RuntimeApi.LoadMetadataForAOTAssembly(dllBytes, HomologousImageMode.SuperSet);
            Debug.Log($"[HybridCLR] LoadMetadata: {addr} Result: {err}");
        }

        // 3. 加载热更 DLL
        Dictionary<string, byte[]> loadedHotfixData = new Dictionary<string, byte[]>();
        foreach (var addr in hotUpdateDllAddresses)
        {
            var handle = Addressables.LoadAssetAsync<TextAsset>(addr);
            yield return handle;
            loadedHotfixData[addr] = handle.Result.bytes;
            Debug.Log($"[HybridCLR] Downloaded: {addr}");
        }

        // 4. 将 DLL 载入运行时
        // 注意：必须所有 DLL 下载完后一次性 Load，或者按依赖顺序 Load
        Assembly hotFixAssembly = null;
        foreach (var kvp in loadedHotfixData)
        {
            hotFixAssembly = Assembly.Load(kvp.Value);
            Debug.Log($"[HybridCLR] Assembly Loaded: {hotFixAssembly.FullName}");
        }

        // 5. 进入游戏逻辑
        EnterGame(hotFixAssembly);
    }

    void EnterGame(Assembly hotFixAssembly)
    {
        // 假设原来的入口是 GameBootstrap.Initialize() 或 Start()
        // 由于 GameBootstrap 现在在热更程序集里，我们必须用反射调用它

        // 获取类型 (注意带上命名空间)
        Type entryType = hotFixAssembly.GetType("GameFramework.Core.GameBootstrap");

        if (entryType != null)
        {
            Debug.Log("[HybridCLR] 找到入口，正在启动...");
            GameObject go = new GameObject("GameBootstrap_HotFix");
            go.AddComponent(entryType);
        }
        else
        {
            // 如果还报错，打印一下这个 DLL 里到底有哪些类型，方便排查
            Debug.LogError($"[HybridCLR] 找不到类型 GameFramework.Core.GameBootstrap。该 DLL 包含以下类型：");
            foreach (var type in hotFixAssembly.GetTypes())
            {
                Debug.LogError(type.FullName);
            }
        }
    }
}