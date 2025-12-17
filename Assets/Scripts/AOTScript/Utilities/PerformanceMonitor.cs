using UnityEngine;
using System.Diagnostics;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;

namespace GameFramework.Utilities
{
    public class PerformanceMonitor : Singleton<PerformanceMonitor>
    {
        private float _deltaTime = 0f;
        private float _fps = 0f;
        private int _frameCount = 0;
        private float _updateInterval = 0.5f;
        private float _timeSinceUpdate = 0f;

        private Dictionary<string, Stopwatch> _timers = new();
        private Dictionary<string, long> _averageTimes = new();
        private Dictionary<string, int> _timerCounts = new();

        [SerializeField] private bool showDebugUI = true;
        [SerializeField] private KeyCode toggleKey = KeyCode.F1;

        private GUIStyle _style;

        private void Update()
        {
            _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;
            _frameCount++;
            _timeSinceUpdate += Time.unscaledDeltaTime;

            if (_timeSinceUpdate >= _updateInterval)
            {
                _fps = _frameCount / _timeSinceUpdate;
                _frameCount = 0;
                _timeSinceUpdate = 0f;
            }

            if (Input.GetKeyDown(toggleKey))
            {
                showDebugUI = !showDebugUI;
            }
        }

        public void StartTimer(string name)
        {
            if (!_timers.ContainsKey(name))
            {
                _timers[name] = new Stopwatch();
                _averageTimes[name] = 0;
                _timerCounts[name] = 0;
            }

            _timers[name].Restart();
        }

        public void StopTimer(string name)
        {
            if (_timers.TryGetValue(name, out var timer))
            {
                timer.Stop();

                // 计算平均值
                _timerCounts[name]++;
                _averageTimes[name] = (_averageTimes[name] + timer.ElapsedMilliseconds) / 2;
            }
        }

        public long GetAverageTime(string name)
        {
            return _averageTimes.TryGetValue(name, out var time) ? time : 0;
        }

        private void OnGUI()
        {
            if (!showDebugUI) return;

            if (_style == null)
            {
                _style = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 20,
                    normal = { textColor = Color.white },
                    alignment = TextAnchor.UpperLeft
                };
            }

            int w = Screen.width, h = Screen.height;
            Rect rect = new Rect(10, 10, w, h * 2 / 100);

            float msec = _deltaTime * 1000.0f;
            string text = $"FPS: {_fps:F1} ({msec:F1}ms)\n";
            text += $"Memory: {(System.GC.GetTotalMemory(false) / 1048576f):F2} MB\n";
            text += $"Entity Count: {GetEntityCount()}\n\n";

            foreach (var kvp in _averageTimes)
            {
                text += $"{kvp.Key}: {kvp.Value}ms\n";
            }

            GUI.Label(rect, text, _style);
        }

        private int GetEntityCount()
        {
            var world = Unity.Entities.World.DefaultGameObjectInjectionWorld;
            // 检查 World 是否存在
            if (world == null) return 0;

            // 修正：通过 UniversalQuery 来计算实体数量
            return world.EntityManager.UniversalQuery.CalculateEntityCount();
        }

        public void LogPerformance(string message)
        {
            Debug.Log($"[Performance] {message} - FPS: {_fps:F1}");
        }
    }
}
