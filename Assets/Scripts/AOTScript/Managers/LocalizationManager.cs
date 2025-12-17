using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.Localization
{
    public enum Language
    {
        Chinese,
        English,
        Japanese,
        Korean
    }

    public class LocalizationManager : Singleton<LocalizationManager>
    {
        private Language _currentLanguage = Language.Chinese;
        private Dictionary<string, Dictionary<Language, string>> _translations = new();

        public Language CurrentLanguage => _currentLanguage;
        public event System.Action<Language> OnLanguageChanged;

        public void Initialize()
        {
            LoadTranslations();
            Debug.Log("本地化系统初始化完成");
        }

        private void LoadTranslations()
        {
            // 从JSON或其他数据源加载翻译
            // 示例数据
            AddTranslation("ui.start_game", new Dictionary<Language, string>
            {
                { Language.Chinese, "开始游戏" },
                { Language.English, "Start Game" },
                { Language.Japanese, "ゲームスタート" },
                { Language.Korean, "게임 시작" }
            });

            AddTranslation("ui.settings", new Dictionary<Language, string>
            {
                { Language.Chinese, "设置" },
                { Language.English, "Settings" },
                { Language.Japanese, "設定" },
                { Language.Korean, "설정" }
            });

            AddTranslation("ui.quit", new Dictionary<Language, string>
            {
                { Language.Chinese, "退出" },
                { Language.English, "Quit" },
                { Language.Japanese, "終了" },
                { Language.Korean, "종료" }
            });

            AddTranslation("game.level_up", new Dictionary<Language, string>
            {
                { Language.Chinese, "升级!" },
                { Language.English, "Level Up!" },
                { Language.Japanese, "レベルアップ！" },
                { Language.Korean, "레벨 업!" }
            });

            AddTranslation("game.game_over", new Dictionary<Language, string>
            {
                { Language.Chinese, "游戏结束" },
                { Language.English, "Game Over" },
                { Language.Japanese, "ゲームオーバー" },
                { Language.Korean, "게임 오버" }
            });
        }

        private void AddTranslation(string key, Dictionary<Language, string> translations)
        {
            _translations[key] = translations;
        }

        public string GetText(string key)
        {
            if (_translations.TryGetValue(key, out var translations))
            {
                if (translations.TryGetValue(_currentLanguage, out var text))
                {
                    return text;
                }
            }

            Debug.LogWarning($"翻译缺失: {key}");
            return key;
        }

        public void SetLanguage(Language language)
        {
            _currentLanguage = language;
            OnLanguageChanged?.Invoke(language);
            Debug.Log($"语言切换至: {language}");
        }

        public string[] GetAvailableLanguages()
        {
            return System.Enum.GetNames(typeof(Language));
        }
    }

    // 本地化文本组件
    public class LocalizedText : MonoBehaviour
    {
        [SerializeField] private string localizationKey;
        private TMPro.TextMeshProUGUI _textComponent;

        private void Awake()
        {
            _textComponent = GetComponent<TMPro.TextMeshProUGUI>();
        }

        private void OnEnable()
        {
            UpdateText();
            LocalizationManager.Instance.OnLanguageChanged += OnLanguageChanged;
        }

        private void OnDisable()
        {
            if (LocalizationManager.Instance != null)
            {
                LocalizationManager.Instance.OnLanguageChanged -= OnLanguageChanged;
            }
        }

        private void OnLanguageChanged(Language language)
        {
            UpdateText();
        }

        private void UpdateText()
        {
            if (_textComponent != null && !string.IsNullOrEmpty(localizationKey))
            {
                _textComponent.text = LocalizationManager.Instance.GetText(localizationKey);
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                UpdateText();
            }
        }
#endif
    }
}
