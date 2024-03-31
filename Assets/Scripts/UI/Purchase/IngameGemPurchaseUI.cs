using System;
using System.Collections;
using Test.FactoryRun.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Test.FactoryRun.UI
{
    public class IngameGemPurchaseUI : MonoBehaviour
    {
        [SerializeField]
        private IngameCurPurchaseItems availItems;

        [SerializeField]
        private IngameGemProduct uiProductPrefab;

        [SerializeField]
        private HorizontalLayoutGroup uiAttachPoint;

        [SerializeField]
        private GameObject loadingOverlay;

        private void Awake()
        {
            StartCoroutine(CreateUI());
        }

        private IEnumerator CreateUI()
        {
            foreach (BoostType item in Enum.GetValues(typeof(BoostType)))
            {
                IngameGemProduct uIProduct = Instantiate(uiProductPrefab);
                uIProduct.OnPurchase += HandlePurchase;
                ItemSpecification itemSpecification = availItems.BoostDetails[item];
                uIProduct.Setup(item, itemSpecification.Title, itemSpecification.Description, itemSpecification.Cost);
                uIProduct.transform.SetParent(uiAttachPoint.transform, false);
                yield return null;
            }
            gameObject.SetActive(false);
        }

        private void HandlePurchase(BoostType item, Action onPurchaseComplete)
        {
            loadingOverlay.SetActive(true);
            string val;
            ItemSpecification itemSpecification = availItems.BoostDetails[item];
            if ((val = PlayerPrefs.GetString(item.ToString(), String.Empty)) != string.Empty)
            {
                DateTime finalTime = DateTime.Parse(val).Add(TimeSpan.FromSeconds(itemSpecification.DurationOfEffect));
                
                //Compare previous purchase timestamp and see if the timer is complete
                //if not throw error message
                if (finalTime < DateTime.UtcNow)
                {
                    if (GameData.UpdateGem(-itemSpecification.Cost))
                    {
                        Debug.Log("Purchase successful");
                        PlayerPrefs.SetString(item.ToString(), DateTime.UtcNow.ToString());
                        itemSpecification.OnBoostApplied?.Invoke();
                    }
                }
                else
                {
                    Debug.LogError("cannot buy the boost yet its in cooldown");
                }
            }
            else
            {
                //Directly purchase if new boost is purchased
                if (GameData.UpdateGem(-itemSpecification.Cost))
                {
                    Debug.Log("Purchase successful");
                    PlayerPrefs.SetString(item.ToString(), DateTime.UtcNow.ToString());
                    itemSpecification.OnBoostApplied?.Invoke();
                }
            }
            loadingOverlay.SetActive(false);
            onPurchaseComplete?.Invoke();
        }
    }
}


