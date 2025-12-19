using cfg;
using GameFramework.Core;
using UnityEngine;

namespace GameHotfix.Guide
{
    public class GuideManager
    {
        private static GuideManager _instance;
        public static GuideManager Instance => _instance ??= new GuideManager();

        private GuidePanel _uiView;
        private Guide_Cfg _currentConfig; // Luban生成的配置类

        // 启动某一组引导
        public void StartGuideGroup(int groupId)
        {
            // 从Luban表中查找该组的第一步 (这里假设你遍历表或者有辅助索引)
            // 示例逻辑：
            var config = ConfigManager.Instance.Tables.GuideCfg;
            foreach (var data in config.DataList)
            {
                if (data.GroupId == groupId)
                {
                    EnterStep(data);
                    return;
                }
            }
        }

        private void EnterStep(Guide_Cfg stepCfg)
        {
            _currentConfig = stepCfg;

            // 1. 寻找目标UI
            GameObject targetObj = GameObject.Find(stepCfg.UiPath);
            RectTransform targetRect = targetObj != null ? targetObj.GetComponent<RectTransform>() : null;

            if (targetRect == null && !string.IsNullOrEmpty(stepCfg.UiPath))
            {
                Debug.LogError($"Guide Error: Cannot find UI at path {stepCfg.UiPath}");
            }

            // 2. 加载或获取GuidePanel
            if (_uiView == null)
            {
                // 调用你的 UIManager 打开界面
                // _uiView = UIManager.Instance.OpenWindow("GuidePanel");
            }

            // 3. 刷新界面
            _uiView.Refresh(
                targetRect,
                stepCfg.ContentText,
                stepCfg.ShowHand,
                stepCfg.HandOffset // Luban里配的Vector2需要转换
            );

            // 4. 绑定点击事件用于进入下一步
            // 这里有一个技巧：如果设置了Target，通常我们监听Target本身的Button点击事件
            // 或者 GuideMask 在检测到穿透点击时，通知Manager
            if (targetObj != null)
            {
                var btn = targetObj.GetComponent<UnityEngine.UI.Button>();
                if (btn != null)
                {
                    btn.onClick.AddListener(OnTargetClicked);
                }
            }
        }

        private void OnTargetClicked()
        {
            // 移除监听避免重复
            // 完成当前步，检查NextId
            if (_currentConfig.NextId != 0)
            {
                var config = ConfigManager.Instance.Tables.GuideCfg;
                var nextData = config.Get(_currentConfig.NextId);
                EnterStep(nextData);
            }
            else
            {
                FinishGuide();
            }
        }

        private void FinishGuide()
        {
            // 关闭UI
            // UIManager.Instance.CloseWindow("GuidePanel");
            _uiView = null;
        }
    }
}