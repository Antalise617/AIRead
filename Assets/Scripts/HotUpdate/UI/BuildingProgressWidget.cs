using UnityEngine.UI;
using GameFramework.Core;
using GameFramework.ECS.Components; // 假设包含生产数据组件

namespace Game.HotUpdate
{
    public class BuildingProgressWidget : UIFollowPanel
    {
        public Image progressFill;

        protected override void LateUpdate()
        {
            base.LateUpdate(); // 处理位置跟随

            
        }
    }
}