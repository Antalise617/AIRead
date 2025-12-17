using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.Events
{
    public class EventManager : Singleton<EventManager>
    {
        private Dictionary<Type, Delegate> _eventDictionary = new();

        public void Subscribe<T>(Action<T> listener) where T : IGameEvent
        {
            var eventType = typeof(T);

            if (_eventDictionary.TryGetValue(eventType, out var existingDelegate))
            {
                _eventDictionary[eventType] = Delegate.Combine(existingDelegate, listener);
            }
            else
            {
                _eventDictionary[eventType] = listener;
            }
        }

        public void Unsubscribe<T>(Action<T> listener) where T : IGameEvent
        {
            var eventType = typeof(T);

            if (_eventDictionary.TryGetValue(eventType, out var existingDelegate))
            {
                var newDelegate = Delegate.Remove(existingDelegate, listener);

                if (newDelegate == null)
                {
                    _eventDictionary.Remove(eventType);
                }
                else
                {
                    _eventDictionary[eventType] = newDelegate;
                }
            }
        }

        public void Publish<T>(T gameEvent) where T : IGameEvent
        {
            var eventType = typeof(T);

            if (_eventDictionary.TryGetValue(eventType, out var existingDelegate))
            {
                var callback = existingDelegate as Action<T>;
                callback?.Invoke(gameEvent);
            }
        }

        public void Clear()
        {
            _eventDictionary.Clear();
        }
    }

    // 事件接口
    public interface IGameEvent { }

    // 事件示例
    public struct GamePausedEvent : IGameEvent
    {
        public bool IsPaused;
    }

    public struct ScoreChangedEvent : IGameEvent
    {
        public int OldScore;
        public int NewScore;
    }
}
