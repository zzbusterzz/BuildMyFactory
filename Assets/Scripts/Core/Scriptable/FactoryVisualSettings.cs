using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace Test.FactoryRun.Core
{
    [CreateAssetMenu(fileName = "FactoryVisualSettings", menuName = "RunFactoryRun/FactoryVisualSettings")]
    public class FactoryVisualSettings : ScriptableObject
    {
        public SerializedDictionary<FactoryLevel, GameObject> FactoryInstance;
        public Color TileNormalColor = Color.white;
        public Color TileSelectedColor = Color.green;
    }
}