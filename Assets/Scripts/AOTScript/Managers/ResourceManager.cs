using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace GameFramework.Managers
{
    public class ResourceManager : Singleton<ResourceManager>
    {
        private Dictionary<string, AsyncOperationHandle> _loadedAssets = new();

        public async UniTask InitializeAsync()
        {
            await Addressables.InitializeAsync().ToUniTask();
            Debug.Log("Addressables 初始化完成");
        }

        public async UniTask<T> LoadAssetAsync<T>(string key) where T : Object
        {
            if (_loadedAssets.ContainsKey(key))
            {
                var handle = _loadedAssets[key];
                if (handle.IsValid() && handle.IsDone)
                {
                    return handle.Result as T;
                }
            }

            var newHandle = Addressables.LoadAssetAsync<T>(key);
            _loadedAssets[key] = newHandle;

            return await newHandle.ToUniTask();
        }

        public async UniTask<GameObject> InstantiateAsync(string key, Transform parent = null)
        {
            var handle = Addressables.InstantiateAsync(key, parent);
            return await handle.ToUniTask();
        }

        public void ReleaseAsset(string key)
        {
            if (_loadedAssets.TryGetValue(key, out var handle))
            {
                Addressables.Release(handle);
                _loadedAssets.Remove(key);
            }
        }

        public void ReleaseAll()
        {
            foreach (var handle in _loadedAssets.Values)
            {
                Addressables.Release(handle);
            }
            _loadedAssets.Clear();
        }
    }
}
