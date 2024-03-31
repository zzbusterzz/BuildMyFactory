using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Test.FactoryRun.UI
{
    public abstract class BasePurchaseUI : MonoBehaviour
    {
        [SerializeField]
        protected TextMeshProUGUI nameText;

        [SerializeField]
        protected TextMeshProUGUI descText;

        [SerializeField]
        protected Image image;

        [SerializeField]
        protected TextMeshProUGUI priceText;

        [SerializeField]
        protected Button purchaseButton;

        public abstract void Purchase();
    }
}


