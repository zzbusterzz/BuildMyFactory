using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace Test.FactoryRun.Core
{
    [CreateAssetMenu(fileName = "IngameCurPurchaseItems", menuName = "RunFactoryRun/IngameCurPurchaseItems")]
    public class IngameCurPurchaseItems :ScriptableObject
    {
        public SerializedDictionary<BoostType, ItemSpecification> BoostDetails = new();
    }
}