using cfg;
using Cysharp.Threading.Tasks; // 引入 UniTask
using GameFramework.Managers;  // 引入 ResourceManager
using Luban;                   // 引入 Luban
using UnityEngine;

namespace GameFramework.Core
{
    public class ConfigManager : Singleton<ConfigManager>
    {
        private GameConfig _gameConfig;
        public GameConfig GameConfig => _gameConfig;

        // 添加 Luban 表的访问入口
        public cfg.Tables Tables { get; private set; }

        public void Initialize(GameConfig config)
        {
            _gameConfig = config;
        }

        public async UniTask LoadLubanTablesAsync()
        {
            Tables = new cfg.Tables();

            await Tables.LoadAsync(async (file) =>
            {
                // -------------------------------------------------------------
                // 【关键修改】拼接完整的 Unity 资源路径
                // 因为你是按文件夹打包，Address 默认为文件的完整路径
                // -------------------------------------------------------------
                string addressableKey = $"Assets/ConfigAssets/LubanConfig/{file}.bytes";

                // 调试建议：如果再次报错，取消下面这行注释，看看控制台打印的路径是否和 Addressables 窗口里的一样
                // Debug.Log($"尝试加载资源: {addressableKey}");

                var textAsset = await ResourceManager.Instance.LoadAssetAsync<TextAsset>(addressableKey);

                if (textAsset == null)
                {
                    Debug.LogError($"[ConfigManager] 无法加载 Luban 数据表，请检查路径是否匹配: {addressableKey}");
                    return null;
                }

                return new ByteBuf(textAsset.bytes);
            });

            Debug.Log("=== Luban 数据表加载完成 ===");
        }
    }
}