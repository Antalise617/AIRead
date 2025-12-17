using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using TMPro;

namespace GameFramework.Scenes
{
    public class LoadingScene : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI loadingText;
        [SerializeField] private UnityEngine.UI.Slider progressBar;

        private async void Start()
        {
            await LoadGame();
        }

        private async UniTask LoadGame()
        {
            // 初始化阶段
            UpdateProgress("初始化系统...", 0.1f);
            await UniTask.Delay(500);

            UpdateProgress("加载配置...", 0.2f);
            await UniTask.Delay(300);

            UpdateProgress("初始化资源管理器...", 0.3f);
            await Managers.ResourceManager.Instance.InitializeAsync();

            UpdateProgress("初始化UI系统...", 0.5f);
            await Managers.UIManager.Instance.InitializeAsync();

            UpdateProgress("初始化输入系统...", 0.7f);
            Managers.InputManager.Instance.Initialize();

           /* UpdateProgress("加载数据表...", 0.8f);*/

            UpdateProgress("初始化本地化...", 0.9f);
            Localization.LocalizationManager.Instance.Initialize();

            UpdateProgress("完成!", 1f);
            await UniTask.Delay(500);

            // 加载主菜单场景
            await Managers.SceneManager.Instance.LoadSceneAsync("MainMenu");
        }

        private void UpdateProgress(string message, float progress)
        {
            if (loadingText != null)
            {
                loadingText.text = message;
            }

            if (progressBar != null)
            {
                progressBar.value = progress;
            }

            Debug.Log($"[加载] {message} ({progress * 100}%)");
        }
    }
}
