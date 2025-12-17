using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace GameFramework.VFX
{
    public class VFXManager : Singleton<VFXManager>
    {
        private Dictionary<string, Queue<ParticleSystem>> _particlePools = new();
        private Dictionary<string, GameObject> _particlePrefabs = new();

        public void Initialize()
        {
            Debug.Log("VFX管理器初始化完成");
        }

        public async UniTask<ParticleSystem> PlayEffect(string effectKey, Vector3 position,
            Quaternion rotation, Transform parent = null)
        {
            ParticleSystem particleSystem = await GetParticleSystem(effectKey);

            if (particleSystem == null)
            {
                Debug.LogWarning($"粒子效果不存在: {effectKey}");
                return null;
            }

            particleSystem.transform.position = position;
            particleSystem.transform.rotation = rotation;

            if (parent != null)
            {
                particleSystem.transform.SetParent(parent);
            }

            particleSystem.Play();

            // 自动回收
            float duration = particleSystem.main.duration + particleSystem.main.startLifetime.constantMax;
            RecycleAfterDelay(effectKey, particleSystem, duration).Forget();

            return particleSystem;
        }

        private async UniTask<ParticleSystem> GetParticleSystem(string key)
        {
            // 检查对象池
            if (_particlePools.TryGetValue(key, out var pool) && pool.Count > 0)
            {
                var ps = pool.Dequeue();
                ps.gameObject.SetActive(true);
                return ps;
            }

            // 加载新的
            if (!_particlePrefabs.ContainsKey(key))
            {
                var prefab = await Managers.ResourceManager.Instance.LoadAssetAsync<GameObject>(key);
                _particlePrefabs[key] = prefab;
            }

            var instance = Instantiate(_particlePrefabs[key]);
            return instance.GetComponent<ParticleSystem>();
        }

        private async UniTaskVoid RecycleAfterDelay(string key, ParticleSystem ps, float delay)
        {
            await UniTask.Delay((int)(delay * 1000));
            RecycleParticleSystem(key, ps);
        }

        private void RecycleParticleSystem(string key, ParticleSystem ps)
        {
            ps.Stop();
            ps.Clear();
            ps.gameObject.SetActive(false);
            ps.transform.SetParent(transform);

            if (!_particlePools.ContainsKey(key))
            {
                _particlePools[key] = new Queue<ParticleSystem>();
            }

            _particlePools[key].Enqueue(ps);
        }

        public void ClearPool(string key)
        {
            if (_particlePools.TryGetValue(key, out var pool))
            {
                while (pool.Count > 0)
                {
                    var ps = pool.Dequeue();
                    Destroy(ps.gameObject);
                }
            }
        }

        public void ClearAllPools()
        {
            foreach (var pool in _particlePools.Values)
            {
                while (pool.Count > 0)
                {
                    var ps = pool.Dequeue();
                    Destroy(ps.gameObject);
                }
            }
            _particlePools.Clear();
        }
    }
}
