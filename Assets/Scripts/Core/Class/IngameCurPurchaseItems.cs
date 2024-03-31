using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using UnityEngine;

namespace Test.FactoryRun.Core
{
    public enum BoostType
    {
        FACTORY_BOOST
    }

    [CreateAssetMenu(fileName = "IngameCurPurchaseItems", menuName = "RunFactoryRun/IngameCurPurchaseItems")]
    public class IngameCurPurchaseItems :ScriptableObject
    {
        //public List<string> ItemType;

        public SerializedDictionary<BoostType, ItemSpecification> BoostDetails = new();

        //[ContextMenu("UpdateDictData")]
        //public void UpdateDictData()
        //{
        //    for(int i = 0; i < ItemType.Count; i++)
        //    {
        //        if (!string.IsNullOrEmpty( ItemType[i]) &&
        //            !BoostDetails.ContainsKey(ItemType[i]))
        //        {
        //            BoostDetails.Add(ItemType[i], new ItemSpecification());
        //        }
        //    }
        //}
    }
}