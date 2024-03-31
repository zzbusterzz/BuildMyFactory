using System;
using Test.FactoryRun.Core;
using TMPro;
using UnityEngine;

namespace Test.FactoryRun.UI
{
    public class UpgradeUI : MonoBehaviour
    {
        public static Action<int, FactoryCell> DisplayUpgradeUI;

        [SerializeField]
        private TextMeshProUGUI textCost;

        private FactoryCell factoryCellToUpgrade;

        private void Awake()
        {
            DisplayUpgradeUI += OpenUpgradeUI;
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            DisplayUpgradeUI -= OpenUpgradeUI;
        }

        public void AcceptClick()
        {
            if (FactoryGrid.UpgradeCell(factoryCellToUpgrade))
            {
                OnCloseClick();
            }
            else
            {
                Debug.Log("Not enough gems");
            }
        }

        private void OpenUpgradeUI(int upgradeCost, FactoryCell cellToUpgrade)
        {
            factoryCellToUpgrade = cellToUpgrade;
            textCost.SetText("Cost : " + upgradeCost);
            gameObject.SetActive(true);
        }

        public void OnCloseClick()
        {
            factoryCellToUpgrade.DeselectTile();
            gameObject.SetActive(false);
        }
    }
}


