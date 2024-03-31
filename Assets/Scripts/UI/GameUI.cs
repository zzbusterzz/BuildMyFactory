using System;
using TMPro;
using UnityEngine;

namespace Test.FactoryRun.UI
{

    public class GameUI : MonoBehaviour
    {
        public static Action<int> updateGemCountUI;

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

        private void updateGemCounter(int obj)
        {
            gemCounter.SetText(obj.ToString());
        }
    }
}


