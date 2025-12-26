using GameFramework.Managers;
using GameFramework.UI;
using GameFramework.Events;
using TMPro;
using UnityEngine;
using GameFramework.ECS.Components;

// 定义任务更新事件
public struct TaskUpdatedEvent : IGameEvent
{
    public string Description; // 任务描述
    public bool IsFinished;    // 是否全部完成（可选，用于控制UI显隐）
}
// 确保该类继承自你的 UI 基类 (UIPanel)
public class TaskPanel : UIPanel
{
    // [UIBind] 会自动查找名字为 "m_tmp_TaskContent" 的 TextMeshProUGUI 组件
    // 请确保 Unity 编辑器中你的 Text 组件名字和这个变量名一致（或通过参数指定路径）
    [UIBind]
    private TextMeshProUGUI m_tmp_TaskContent;

    // 可选：绑定一个背景或父物体，任务完成时隐藏整个面板
    // [UIBind] private GameObject m_TaskContainer; 

    protected override void OnInit()
    {
        base.OnInit();

        // 订阅任务更新事件
        EventManager.Instance.Subscribe<TaskUpdatedEvent>(OnTaskUpdated);

        Debug.Log("[TaskPanel] 初始化完成，等待任务事件...");
    }

    protected void OnDestroy()
    {
        // 销毁时务必取消订阅，防止报错
        if (EventManager.Instance != null)
        {
            EventManager.Instance.Unsubscribe<TaskUpdatedEvent>(OnTaskUpdated);
        }
    }

    // 事件回调函数
    private void OnTaskUpdated(TaskUpdatedEvent evt)
    {
        if (m_tmp_TaskContent != null)
        {
            m_tmp_TaskContent.text = evt.Description;
            Debug.Log($"[TaskPanel] 更新文本: {evt.Description}");
        }
        else
        {
            Debug.LogError("[TaskPanel] 未找到 m_tmp_TaskContent 组件，请检查命名是否匹配！");
        }
    }
}