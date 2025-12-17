using UnityEngine;
using System.IO;
using Newtonsoft.Json;

namespace GameFramework.Managers
{
    [System.Serializable]
    public class SaveData
    {
        public int playerLevel;
        public int playerXP;
        public float[] playerPosition;
        public string lastScene;
        public long saveTimestamp;
    }

    public class SaveManager : Singleton<SaveManager>
    {
        private string _savePath;
        private SaveData _currentSave;

        public void Initialize()
        {
            _savePath = Path.Combine(Application.persistentDataPath, "savegame.json");
            Debug.Log($"存档路径: {_savePath}");
        }

        public void SaveGame(SaveData data)
        {
            data.saveTimestamp = System.DateTimeOffset.Now.ToUnixTimeSeconds();
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(_savePath, json);
            _currentSave = data;
            Debug.Log("游戏已保存");
        }

        public SaveData LoadGame()
        {
            if (!File.Exists(_savePath))
            {
                Debug.Log("没有找到存档文件");
                return CreateNewSave();
            }

            try
            {
                string json = File.ReadAllText(_savePath);
                _currentSave = JsonConvert.DeserializeObject<SaveData>(json);
                Debug.Log("游戏已加载");
                return _currentSave;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"加载存档失败: {e.Message}");
                return CreateNewSave();
            }
        }

        private SaveData CreateNewSave()
        {
            return new SaveData
            {
                playerLevel = 1,
                playerXP = 0,
                playerPosition = new float[] { 0, 0, 0 },
                lastScene = "MainMenu"
            };
        }

        public void DeleteSave()
        {
            if (File.Exists(_savePath))
            {
                File.Delete(_savePath);
                Debug.Log("存档已删除");
            }
        }
    }
}
