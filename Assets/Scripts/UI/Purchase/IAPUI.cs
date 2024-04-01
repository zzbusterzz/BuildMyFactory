using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Test.FactoryRun.Core;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using UnityEngine.UI;

namespace Test.FactoryRun.UI
{
    public class IAPUI : MonoBehaviour, IDetailedStoreListener
    {
        private const string IAP_CUR = "Gem";
        private const string IAP_CURWITHAD = "WatchAdGetGem";

        [SerializeField]
        private UIProduct uiProductPrefab;

        [SerializeField]
        private HorizontalLayoutGroup uiAttachPoint;

        [SerializeField]
        private GameObject loadingOverlay;

        private Action onPurchaseCompleted;
        private IStoreController storeController;
        private IExtensionProvider extensionProvider;

        private async void Awake()
        {
            InitializationOptions options = new InitializationOptions()
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                .SetEnvironmentName("test");
#else
                .SetEnvironmentName("prodution");
#endif
            await UnityServices.InitializeAsync(options);
            ResourceRequest op = Resources.LoadAsync<TextAsset>("IAPProductCatalog");
            op.completed += HandleIAPCatalogLoaded;


#if DEVELOPMENT_BUILD
            gameObject.SetActive(false);
#endif
        }

        private void HandleIAPCatalogLoaded(AsyncOperation operation)
        {
            ResourceRequest req = operation as ResourceRequest;
            Debug.Log("Asset loaded" + req.asset);
            ProductCatalog catalog = JsonUtility.FromJson<ProductCatalog>((req.asset as TextAsset).text);
            Debug.Log($"Catalog count {catalog.allProducts.Count}");

            StandardPurchasingModule.Instance().useFakeStoreUIMode = FakeStoreUIMode.StandardUser;
            StandardPurchasingModule.Instance().useFakeStoreAlways = true;

            ConfigurationBuilder builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance(AppStore.GooglePlay));
        
            foreach(ProductCatalogItem item in catalog.allProducts)
            {
                List<PayoutDefinition> pds = new();
                for(int i = 0; i < item.Payouts.Count; i++)
                {
                    ProductCatalogPayout pd = item.Payouts[i];

                    PayoutDefinition pdf = new PayoutDefinition(pd.typeString, pd.subtype, pd.quantity, pd.data);
                    pds.Add(pdf);
                }

                builder.AddProduct(item.id, item.type, null, pds);
            }

            UnityPurchasing.Initialize(this, builder);
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            storeController = controller;
            extensionProvider = extensions;
            StartCoroutine(CreateUI());
        }

        private IEnumerator CreateUI()
        {
            List<Product> products = storeController.products.all
                .OrderBy(item => item.metadata.localizedPrice)
                .ToList();

            foreach(Product product in products)
            {
                UIProduct uIProduct = Instantiate(uiProductPrefab);
                uIProduct.OnPurchase += HandlePurchase;
                uIProduct.Setup(product);
                uIProduct.transform.SetParent(uiAttachPoint.transform, false);
                yield return null;
            }


            gameObject.SetActive(false);
        }

        private void HandlePurchase(Product product, Action onPurchaseComplete)
        {
            loadingOverlay.SetActive(true);
            onPurchaseCompleted = onPurchaseComplete;
            storeController.InitiatePurchase(product);
        }

        public void OnInitializeFailed(InitializationFailureReason error) {}

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            Debug.LogError($"Cannot initialise Error Type : {error} \nError Message: {message}");
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
        {
            Debug.LogError($"Purchase failed Product code : {product.definition.id} \nreason: {failureDescription.message}");
            loadingOverlay.SetActive(false);
            onPurchaseCompleted?.Invoke();
            onPurchaseCompleted = null;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            Debug.LogError($"Purchase failed Product code : {product.definition.id} \nreason: {failureReason}");
            loadingOverlay.SetActive(false);
            onPurchaseCompleted?.Invoke();
            onPurchaseCompleted = null;
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
        {
            Debug.Log($"Purchased successfully : {purchaseEvent.purchasedProduct.definition.id}");
            loadingOverlay.SetActive(false);
            onPurchaseCompleted?.Invoke();
            onPurchaseCompleted = null;

            ProductType type = purchaseEvent.purchasedProduct.definition.type;
            if(type != ProductType.Consumable)
            {
                Debug.Log($"Purhcased {purchaseEvent.purchasedProduct.metadata.localizedTitle}");
            }


            PayoutDefinition payout = purchaseEvent.purchasedProduct.definition.payout;
            if(payout != null)
            {
                switch (payout.type)
                {
                    case PayoutType.Currency:
                        if (payout.subtype == IAP_CUR)
                        {
                            GameData.UpdateGem((int)payout.quantity);
                        }
                        else if (payout.subtype == IAP_CURWITHAD)
                        {
                            //Trigger admob here
                            AdReward.ShowRewardedAd(() =>
                            {
                                GameData.UpdateGem((int)payout.quantity);
                            });
                        }
                        break;

                }
            }
            

            return PurchaseProcessingResult.Complete;
        }
    }
}


