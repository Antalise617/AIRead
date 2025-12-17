using UnityEngine;
using GameFramework.Events;
using cfg;
using GameFramework.ECS.Components;
using GameFramework.Managers;
using Cysharp.Threading.Tasks;

namespace HotUpdate.Core
{
    public struct ObjectBuiltEvent : IGameEvent
    {
        public PlacementType Type;
    }
    // 定义任务更新事件
    public struct TaskUpdatedEvent : IGameEvent
    {
        public string Description; // 任务描述
        public bool IsFinished;    // 是否全部完成（可选，用于控制UI显隐）
    }

    public class SimpleQuestManager
    {
        public static readonly SimpleQuestManager Instance = new SimpleQuestManager();

        // 当前任务阶段 (1:岛屿, 2:建筑, 3:桥梁, 4:结束)
        private int _currentStep = 1;

        private int _islandCount = 0;
        private int _buildingCount = 0;
        private int _bridgeCount = 0;

        public void Init()
        {
            UIManager.Instance.ShowPanelAsync<TaskPanel>("TaskPanel").Forget();
            EventManager.Instance.Subscribe<ObjectBuiltEvent>(OnObjectBuilt);
            Debug.Log("【任务系统】启动");
            // 初始化时强制刷新一次UI
            RefreshUI();
        }

        private void OnObjectBuilt(ObjectBuiltEvent evt)
        {
            // 根据当前阶段，只有建造对应类型的物体才计入进度
            switch (_currentStep)
            {
                case 1: // 阶段1：造岛
                    if (evt.Type == PlacementType.Island)
                    {
                        _islandCount++;
                        CheckTask1();
                    }
                    break;
                case 2: // 阶段2：造建筑
                    if (evt.Type == PlacementType.Building)
                    {
                        _buildingCount++;
                        CheckTask2();
                    }
                    break;
                case 3: // 阶段3：造桥
                    if (evt.Type == PlacementType.Bridge)
                    {
                        _bridgeCount++;
                        CheckTask3();
                    }
                    break;
            }
        }

        // 核心方法：发送事件通知 TaskPanel
        private void RefreshUI()
        {
            string taskText = "";
            bool isAllDone = false;

            switch (_currentStep)
            {
                case 1:
                    taskText = $"建造微型岛屿 ({_islandCount}/3)";
                    break;
                case 2:
                    taskText = $"建造任意建筑 ({_buildingCount}/2)";
                    break;
                case 3:
                    taskText = $"连接桥梁 ({_bridgeCount}/2)";
                    break;
                case 4:
                    taskText = "所有任务已完成！";
                    isAllDone = true;
                    break;
            }

            // 发送事件
            EventManager.Instance.Publish(new TaskUpdatedEvent
            {
                Description = taskText,
                IsFinished = isAllDone
            });
        }

        private void CheckTask1()
        {
            RefreshUI(); // 进度变化，刷新UI
            if (_islandCount >= 3)
            {
                _currentStep = 2; // 进入下一阶段
                Debug.Log("<color=yellow>任务1完成！进入第二阶段。</color>");
                RefreshUI(); // 阶段变化，再次刷新UI显示新任务
            }
        }

        private void CheckTask2()
        {
            RefreshUI();
            if (_buildingCount >= 2)
            {
                _currentStep = 3;
                Debug.Log("<color=yellow>任务2完成！进入第三阶段。</color>");
                RefreshUI();
            }
        }

        private void CheckTask3()
        {
            RefreshUI();
            if (_bridgeCount >= 2)
            {
                _currentStep = 4;
                Debug.Log("<color=yellow>任务3完成！全部结束。</color>");
                RefreshUI();
            }
        }
    }
}