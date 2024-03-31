using System;
using UnityEngine.Purchasing;

namespace Test.FactoryRun.UI
{
    public class UIProduct: BasePurchaseUI
    {
        private Product model;

        public Action<Product, Action> OnPurchase;

        public void Setup(Product product)
        {
            model = product;
            nameText.SetText(product.metadata.localizedTitle);
            descText.SetText(product.metadata.localizedDescription);
            priceText.SetText(product.metadata.localizedPriceString + product.metadata.isoCurrencyCode);
        }

        public override void Purchase()
        {
            purchaseButton.enabled = false;
            OnPurchase?.Invoke(model, OnPurchaseComplete);
        }

        private void OnPurchaseComplete()
        {
            purchaseButton.enabled = true;
        }
    }
}