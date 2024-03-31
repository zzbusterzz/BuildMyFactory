using System;
using Test.FactoryRun.Core;
using UnityEngine;

namespace Test.FactoryRun.UI
{
    public class IngameGemProduct : BasePurchaseUI
    {
        private int gemCost;

        public void Setup(string title, string desc, int gemCost)
        {
            nameText.SetText(title);
            descText.SetText(desc);
            priceText.SetText(gemCost.ToString());
            this.gemCost = gemCost;
        }

        public override void Purchase()
        {
            purchaseButton.enabled = false;
            if (GameData.UpdateGem(-gemCost))
            {
                Debug.Log("Purchase successful");
                PlayerPrefs.SetString("BoosterPack", DateTime.UtcNow.ToString());
            }
        }

    }
}


