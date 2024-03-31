using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace Test.FactoryRun.Core
{
    [CreateAssetMenu(fileName = "FactorySettings", menuName = "RunFactoryRun/FactorySettings")]
    public class FactorySettings : ScriptableObject
    {
        public SerializedDictionary<FactoryLevel, GemSpawnData> GemSpawnDetails;
        public SerializedDictionary<FactoryLevel, int> BuildingCostPerLevel;
        public Vector2Int SlotSize = new Vector2Int(10, 10);
        public int InitFactoryLocationIndex = 0;
    }
}