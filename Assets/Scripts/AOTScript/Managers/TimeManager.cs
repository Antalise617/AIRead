using UnityEngine;
using System; // [新增] 需要引用 System 命名空间以使用 DateTimeOffset

namespace GameFramework.Managers
{
    public class TimeManager : Singleton<TimeManager>
    {
        private float _timeScale = 1f;
        private float _gameTime = 0f;
        private bool _isPaused = false;

        public float TimeScale => _timeScale;
        public float GameTime => _gameTime;
        public bool IsPaused => _isPaused;

        // ========================================================================
        // [新增] ServerTime 属性，解决 CS1061 错误
        // ========================================================================
        /// <summary>
        /// 获取当前服务器时间戳（秒）。
        /// 目前使用 UTC 时间模拟，后续对接服务器对时后可在此处加上偏移量。
        /// </summary>
        public long ServerTime
        {
            get
            {
                return DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }
        // ========================================================================

        private void Update()
        {
            if (!_isPaused)
            {
                _gameTime += Time.deltaTime;
            }
        }

        public void SetTimeScale(float scale)
        {
            _timeScale = Mathf.Clamp(scale, 0f, 10f);
            Time.timeScale = _isPaused ? 0f : _timeScale;
        }

        public void Pause()
        {
            _isPaused = true;
            Time.timeScale = 0f;
        }

        public void Resume()
        {
            _isPaused = false;
            Time.timeScale = _timeScale;
        }

        public void TogglePause()
        {
            if (_isPaused)
                Resume();
            else
                Pause();
        }

        public void SlowMotion(float scale, float duration)
        {
            SetTimeScale(scale);
            CancelInvoke(nameof(ResetTimeScale));
            Invoke(nameof(ResetTimeScale), duration);
        }

        private void ResetTimeScale()
        {
            SetTimeScale(1f);
        }

        public string GetFormattedTime()
        {
            int minutes = Mathf.FloorToInt(_gameTime / 60f);
            int seconds = Mathf.FloorToInt(_gameTime % 60f);
            return $"{minutes:00}:{seconds:00}";
        }
    }
}