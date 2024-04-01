using UnityEngine;

namespace Test.FactoryRun.Core
{
    [SerializeField]
    public class SaveDataGroup
    {
        public GameData GameData;
        public FactoryData[] FactoryDatas;
        public string saveTime;
    }
}