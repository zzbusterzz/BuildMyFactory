using System;
using UnityEngine;

namespace Test.FactoryRun.Core
{
    public class SaveGameManager
    {
        public static Func<bool> IsSaveDataPresent;

        private const string gameState = "GameState";
        private SaveDataGroup dataGroup;

        public SaveGameManager()
        {
            IsSaveDataPresent = HasSaveData;
        }

        ~ SaveGameManager()
        {
            IsSaveDataPresent = null;
        }

        public void SaveState(GameData data, FactoryData[] factoryDatas)
        {
            if(dataGroup ==null)
            {
                dataGroup = new SaveDataGroup();
            }

            dataGroup.GameData = data;
            dataGroup.FactoryDatas = factoryDatas;
            dataGroup.saveTime = UTCTimer.UtcNow.ToString();

            string jsonString = JsonUtility.ToJson(dataGroup);
            Debug.Log(jsonString);

            PlayerPrefs.SetString(gameState, jsonString);
            PlayerPrefs.Save();
        }

        public SaveDataGroup LoadState()
        {
            if (HasSaveData())
            {
                string jsonString = PlayerPrefs.GetString(gameState);
                dataGroup = JsonUtility.FromJson<SaveDataGroup>(jsonString);
                return dataGroup;
            }

            return null;
        }

        public bool HasSaveData()
        {
            return PlayerPrefs.HasKey(gameState);
        
        }

        public GameData GetGameData()
        {
            return dataGroup.GameData;
        }

        public FactoryData[] GetFactoryData()
        {
            return dataGroup.FactoryDatas;
        }
    }
}