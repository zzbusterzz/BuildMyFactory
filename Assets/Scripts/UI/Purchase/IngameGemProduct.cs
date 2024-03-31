using System;
using Test.FactoryRun.Core;
using UnityEngine;

namespace Test.FactoryRun.UI
{
    public class IngameGemProduct : BasePurchaseUI
    {
        private BoostType id;
        public Action<BoostType, Action> OnPurchase;

        public void Setup(BoostType id, string title, string desc, int gemCost)
        {
            nameText.SetText(title);
            descText.SetText(desc);
            priceText.SetText(gemCost.ToString());
            this.id = id;
        }

        public override void Purchase()
        {
            purchaseButton.enabled = false;
            OnPurchase?.Invoke(id, OnPurchaseComplete);
        }

        void OnPurchaseComplete()
        {
            purchaseButton.enabled = true;
        }
    }
}


