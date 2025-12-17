using GameFramework.Events;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.Core
{
    public enum GameState
    {
        Initializing,
        MainMenu,
        Loading,
        Playing,
        Paused,
        GameOver,
        Victory
    }

    public class GameStateManager : Singleton<GameStateManager>
    {
        private GameState _currentState = GameState.Initializing;
        private Dictionary<GameState, IGameState> _states = new();

        public GameState CurrentState => _currentState;
        public event Action<GameState, GameState> OnStateChanged;

        protected override void Awake()
        {
            base.Awake();
            InitializeStates();
        }

        private void InitializeStates()
        {
            _states[GameState.Initializing] = new InitializingState();
            _states[GameState.MainMenu] = new MainMenuState();
            _states[GameState.Loading] = new LoadingState();
            _states[GameState.Playing] = new PlayingState();
            _states[GameState.Paused] = new PausedState();
            _states[GameState.GameOver] = new GameOverState();
            _states[GameState.Victory] = new VictoryState();
        }

        public void ChangeState(GameState newState)
        {
            if (_currentState == newState)
                return;

            var previousState = _currentState;

            // 退出当前状态
            if (_states.TryGetValue(_currentState, out var currentStateObj))
            {
                currentStateObj.OnExit();
            }

            _currentState = newState;

            // 进入新状态
            if (_states.TryGetValue(_currentState, out var newStateObj))
            {
                newStateObj.OnEnter();
            }

            OnStateChanged?.Invoke(previousState, newState);
            Debug.Log($"状态切换: {previousState} -> {newState}");
        }

        private void Update()
        {
            if (_states.TryGetValue(_currentState, out var state))
            {
                state.OnUpdate(Time.deltaTime);
            }
        }
    }

    // 状态接口
    public interface IGameState
    {
        void OnEnter();
        void OnUpdate(float deltaTime);
        void OnExit();
    }

    // 状态实现
    public class InitializingState : IGameState
    {
        public void OnEnter() { Debug.Log("进入初始化状态"); }
        public void OnUpdate(float deltaTime) { }
        public void OnExit() { Debug.Log("退出初始化状态"); }
    }

    public class MainMenuState : IGameState
    {
        public void OnEnter()
        {
            Debug.Log("进入主菜单状态");
            Time.timeScale = 1f;
        }
        public void OnUpdate(float deltaTime) { }
        public void OnExit() { Debug.Log("退出主菜单状态"); }
    }

    public class LoadingState : IGameState
    {
        public void OnEnter() { Debug.Log("进入加载状态"); }
        public void OnUpdate(float deltaTime) { }
        public void OnExit() { Debug.Log("退出加载状态"); }
    }

    public class PlayingState : IGameState
    {
        public void OnEnter()
        {
            Debug.Log("进入游戏状态");
            Time.timeScale = 1f;
        }
        public void OnUpdate(float deltaTime)
        {
            // 检测暂停输入
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GameStateManager.Instance.ChangeState(GameState.Paused);
            }
        }
        public void OnExit() { Debug.Log("退出游戏状态"); }
    }

    public class PausedState : IGameState
    {
        public void OnEnter()
        {
            Debug.Log("进入暂停状态");
            Time.timeScale = 0f;
            EventManager.Instance.Publish(new Events.GamePausedEvent { IsPaused = true });
        }

        public void OnUpdate(float deltaTime)
        {
            // 检测恢复输入
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GameStateManager.Instance.ChangeState(GameState.Playing);
            }
        }

        public void OnExit()
        {
            Debug.Log("退出暂停状态");
            Time.timeScale = 1f;
            EventManager.Instance.Publish(new Events.GamePausedEvent { IsPaused = false });
        }
    }

    public class GameOverState : IGameState
    {
        public void OnEnter()
        {
            Debug.Log("进入游戏结束状态");
            Time.timeScale = 0f;
        }
        public void OnUpdate(float deltaTime) { }
        public void OnExit() { Debug.Log("退出游戏结束状态"); }
    }

    public class VictoryState : IGameState
    {
        public void OnEnter()
        {
            Debug.Log("进入胜利状态");
            Time.timeScale = 0f;
        }
        public void OnUpdate(float deltaTime) { }
        public void OnExit() { Debug.Log("退出胜利状态"); }
    }
}
