using UnityEngine;
using System.Collections.Generic;

namespace GameFramework.Managers
{
    public class PoolManager : Singleton<PoolManager>
    {
        private Dictionary<string, Queue<GameObject>> _pools = new();
        private Dictionary<string, GameObject> _prefabs = new();

        public void Initialize()
        {
            Debug.Log("对象池初始化完成");
        }

        public void CreatePool(string key, GameObject prefab, int initialSize = 10)
        {
            if (_pools.ContainsKey(key))
                return;

            _prefabs[key] = prefab;
            _pools[key] = new Queue<GameObject>();

            for (int i = 0; i < initialSize; i++)
            {
                var obj = Instantiate(prefab);
                obj.SetActive(false);
                obj.transform.SetParent(transform);
                _pools[key].Enqueue(obj);
            }
        }

        public GameObject Spawn(string key, Vector3 position, Quaternion rotation)
        {
            if (!_pools.ContainsKey(key))
            {
                Debug.LogWarning($"对象池不存在: {key}");
                return null;
            }

            GameObject obj;

            if (_pools[key].Count > 0)
            {
                obj = _pools[key].Dequeue();
            }
            else
            {
                obj = Instantiate(_prefabs[key]);
            }

            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.SetActive(true);

            return obj;
        }

        public void Despawn(string key, GameObject obj)
        {
            if (!_pools.ContainsKey(key))
                return;

            obj.SetActive(false);
            obj.transform.SetParent(transform);
            _pools[key].Enqueue(obj);
        }
    }
}
