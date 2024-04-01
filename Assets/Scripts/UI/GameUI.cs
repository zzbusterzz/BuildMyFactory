using System;
using TMPro;
using UnityEngine;

namespace Test.FactoryRun.UI
{
    public class GameUI : MonoBehaviour
    {
        public static Action<double> updateGemCountUI;

        [SerializeField]
        private GameObject iapPanel;

        [SerializeField]
        private GameObject gameBoostPanel;

        [SerializeField]
        private TextMeshProUGUI gemCounter;

        private void Awake()
        {
            updateGemCountUI += updateGemCounter;
        }

        private void OnDestroy()
        {
            updateGemCountUI -= updateGemCounter;
        }

        private void updateGemCounter(double gemCount)
        {
            gemCounter.SetText(gemCount.ToString());
        }

        public void ToggleIAPPanel(bool val)
        {
            iapPanel.SetActive(val);
            gameBoostPanel.SetActive(false);
        }

        public void ToggleIGameBoosterPanel(bool val)
        {
            gameBoostPanel.SetActive(val);
            iapPanel.SetActive(false);
        }
    }
}


