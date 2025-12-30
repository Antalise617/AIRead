using UnityEngine;
using Unity.Entities;
using GameFramework.ECS.Components;
using GameFramework.Managers;
using Game.HotUpdate.UI;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.Collections;
using System.Linq; // 用于查找 List 中的元素
using Game.HotUpdate;

namespace GameFramework.ECS.Systems
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial class IslandStatusSystem : SystemBase
    {
        private Dictionary<Entity, ProgressBarWidget> _activeWidgets;

        protected override void OnCreate()
        {
            base.OnCreate();
            _activeWidgets = new Dictionary<Entity, ProgressBarWidget>();
        }

        protected override void OnUpdate()
        {
            // 获取当前服务器时间(秒)
            long currentServerTime = TimeManager.Instance.ServerTime;

            // 使用 EntityCommandBuffer 来在遍历中安全地修改组件
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            Entities.ForEach((Entity entity, ref IslandStatusComponent status) =>
            {
                // -----------------------------------------------------------
                // 1. 自动触发完成逻辑
                // -----------------------------------------------------------
                // 状态 0(建造中) 或 3(销毁中) 需要倒计时
                if (status.State == 0 || status.State == 3)
                {
                    // 检查是否倒计时结束
                    if (currentServerTime >= status.EndTime)
                    {
                        // 检查是否尚未发送请求 (且有有效的 ServerId)
                        if (!status.IsRequestSent && !status.ServerId.IsEmpty)
                        {
                            status.IsRequestSent = true; // 标记为已发送，防止下一帧重复执行
                            
                            // 发送请求给服务器
                            SendTimerEndRequest(entity, status.ServerId.ToString()).Forget();
                        }
                    }
                }

                // -----------------------------------------------------------
                // 2. UI 管理逻辑
                // -----------------------------------------------------------
                bool needProgressBar = (status.State == 0 || status.State == 3);

                if (needProgressBar)
                {
                    // 如果需要显示且没创建，则创建
                    if (!_activeWidgets.ContainsKey(entity))
                    {
                        CreateWidgetForEntity(entity, status).Forget();
                    }
                    else
                    {
                        // 如果已创建，刷新数据
                        var widget = _activeWidgets[entity];
                        if (widget != null)
                        {
                            widget.RefreshData(entity, status.State, status.StartTime, status.EndTime);
                        }
                    }
                }
                else
                {
                    // 如果不需要显示（例如状态已变为 1），则移除
                    if (_activeWidgets.ContainsKey(entity))
                    {
                        var widget = _activeWidgets[entity];
                        if (widget != null) widget.CloseSelf();
                        _activeWidgets.Remove(entity);
                    }
                }

            }).WithoutBurst().Run();

            // 应用组件修改 (IsRequestSent = true)
            ecb.Playback(EntityManager);
            ecb.Dispose();
        }

        /// <summary>
        /// 发送倒计时结束请求
        /// </summary>
        private async UniTaskVoid SendTimerEndRequest(Entity entity, string tileId)
        {
            Debug.Log($"[IslandStatusSystem] 倒计时结束，发送状态同步请求... ID: {tileId}");

            var dto = new TileUpLevelDTO { _id = tileId };

            // 【注意】根据 Swagger，地块完成使用 /tile/upLevelSpeedEnd 接口
            var result = await NetworkManager.Instance.SendAsync<GamesDTO>("/tile/upLevelSpeedEnd", dto);

            if (result != null && result.Tile != null)
            {
                // 找到返回数据中对应的那个岛屿
                var updatedTile = result.Tile.FirstOrDefault(t => t._id == tileId);
                
                if (updatedTile != null)
                {
                    Debug.Log($"[IslandStatusSystem] 状态同步成功！新状态: {updatedTile.state}");

                    // 再次检查实体是否还存在
                    if (EntityManager.Exists(entity))
                    {
                        var status = EntityManager.GetComponentData<IslandStatusComponent>(entity);
                        
                        // 更新组件数据
                        status.State = updatedTile.state; // 应该变为 1 (正常) 或 4 (已销毁)
                        status.StartTime = updatedTile.start_time / 1000;
                        status.EndTime = updatedTile.end_time / 1000;
                        status.IsRequestSent = false; // 重置发送标记
                        
                        EntityManager.SetComponentData(entity, status);

                        // 如果状态变为 4 (已销毁)，可能需要在这里处理销毁实体的逻辑
                        if (updatedTile.state == 4) 
                        {
                             // EntityManager.DestroyEntity(entity); // 根据需求决定是否立即销毁
                        }
                    }
                }
            }
            else
            {
                Debug.LogError($"[IslandStatusSystem] 请求失败或未返回有效数据。将在下一帧重试（或需要手动重试）。");
                // 如果需要重试，可以将 IsRequestSent 设回 false，但建议加个冷却时间防止刷接口
                if (EntityManager.Exists(entity))
                {
                    var status = EntityManager.GetComponentData<IslandStatusComponent>(entity);
                    status.IsRequestSent = false; 
                    EntityManager.SetComponentData(entity, status);
                }
            }
        }

        private async UniTaskVoid CreateWidgetForEntity(Entity entity, IslandStatusComponent status)
        {
            if (_activeWidgets.ContainsKey(entity)) return;
            _activeWidgets[entity] = null; // 占位

            var panel = await UIManager.Instance.ShowPanelAsync<ProgressBarWidget>("ProgressBarWidget");
            
            if (EntityManager.Exists(entity))
            {
                if (panel != null)
                {
                    _activeWidgets[entity] = panel;
                    panel.RefreshData(entity, status.State, status.StartTime, status.EndTime);
                }
                else
                {
                    _activeWidgets.Remove(entity);
                }
            }
            else
            {
                if (panel != null) panel.CloseSelf();
                _activeWidgets.Remove(entity);
            }
        }
    }
}