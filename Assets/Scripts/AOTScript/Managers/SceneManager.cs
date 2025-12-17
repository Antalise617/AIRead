using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameFramework.Managers
{
    public class SceneManager : Singleton<SceneManager>
    {
        private string _currentScene;

        public async UniTask LoadSceneAsync(string sceneName,
            LoadSceneMode mode = LoadSceneMode.Single)
        {
            Debug.Log($"加载场景: {sceneName}");

            var operation = UnityEngine.SceneManagement.SceneManager
                .LoadSceneAsync(sceneName, mode);

            await operation.ToUniTask(Progress.Create<float>(p =>
            {
                Debug.Log($"场景加载进度: {p * 100}%");
            }));

            _currentScene = sceneName;
            Debug.Log($"场景加载完成: {sceneName}");
        }

        public string GetCurrentScene() => _currentScene;
    }
}
