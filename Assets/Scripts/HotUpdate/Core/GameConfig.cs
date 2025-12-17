using UnityEngine;

namespace GameFramework.Core
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "GameFramework/Game Config")]
    public class GameConfig : ScriptableObject
    {
        [Header("”Œœ∑…Ë÷√")]
        public string gameVersion = "1.0.0";
    }

    [System.Serializable]
    public class PlatformConfig
    {
        public int targetWidth = 1920;
        public int targetHeight = 1080;
        public bool fullscreen = true;
        public int qualityLevel = 2;
    }
}
