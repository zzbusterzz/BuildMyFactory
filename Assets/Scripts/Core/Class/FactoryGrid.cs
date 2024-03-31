using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Test.FactoryRun.Core
{

    [Serializable]
    public class FactoryGrid
    {
        public static Func<FactoryCell, bool> UpgradeCell;
        public static Action<int> OnGemSpawned;

        [SerializeField]
        private FactorySettings settings;

        [SerializeField]
        private IngameCurPurchaseItems purchaseItems;

        private List<FactoryCell> workingCells = new List<FactoryCell>();
        private Array maxFactoryLevel = Enum.GetValues(typeof(FactoryLevel));

        public void Init()
        {
            UpgradeCell += TryUpgradeCell;
            OnGemSpawned += UpdateGemCount;
            FactorySlotHolder.InitBlocks.Invoke(settings.SlotSize, settings.InitFactoryLocationIndex);

            purchaseItems.BoostDetails[BoostType.FACTORY_BOOST].OnBoostApplied += ApplyBoostToSystem;
        }

        ~FactoryGrid() 
        {
            UpgradeCell -= TryUpgradeCell;
            OnGemSpawned -= UpdateGemCount;
        }

        private bool TryUpgradeCell(FactoryCell cell)
        {
            //Check if factory has reached max level
            //Check if building cost is available in config
            //Check if sufficient number of gems available
            if ((int)cell.Level < maxFactoryLevel.Length - 1 &&
               settings.BuildingCostPerLevel.ContainsKey(cell.Level + 1) &&
                GameData.UpdateGem(-settings.BuildingCostPerLevel[cell.Level+1]))
            {
                if(cell.Level == FactoryLevel.LEVEL0)
                {
                    workingCells.Add(cell);
                }

                ////Im taking the new update data if the timer increment is happening
                ////since its not mentioned weather the timer needs to reset or continue after upgrading
                ////taking the old timer value and not restarting from 0
                //GemSpawnData gsd = settings.GemSpawnDetails[cell.Level + 1];
                cell.UpdateSpawnData();
                cell.OnUpdateCompleted();
                return true;
            }

            return false;
        }

        private void ApplyBoostToSystem()
        {
            for (int i = 0; i < workingCells.Count; i++)
            {
                workingCells[i].ToggleFactoryBoost(true, purchaseItems.BoostDetails[BoostType.FACTORY_BOOST].EffectBoostMult);
            }
            WaitForFactoryBoostTimerToExpire(purchaseItems.BoostDetails[BoostType.FACTORY_BOOST].DurationOfEffect);
        }

        private void UpdateGemCount(int gemCount)
        {
            GameData.UpdateGem(gemCount);
        }

        public void Update()
        {
            for (int i = 0; i < workingCells.Count; i++)
            {
                workingCells[i].UpdateTime();
            }
        }

        async void WaitForFactoryBoostTimerToExpire(float timer)
        {
            await Task.Delay((int)(timer * 1000));
            
            for (int i = 0; i < workingCells.Count; i++)
            {
                workingCells[i].ToggleFactoryBoost(false);
            }
        }

        async void ResumeBoostTimer()
        {
            string utcTime = PlayerPrefs.GetString(BoostType.FACTORY_BOOST.ToString(), String.Empty);

            if(!String.IsNullOrEmpty(utcTime))
            {
                DateTime finalTime = DateTime.Parse(utcTime).Add(TimeSpan.FromSeconds(purchaseItems.BoostDetails[BoostType.FACTORY_BOOST].DurationOfEffect));
                if (finalTime > DateTime.UtcNow)
                {
                    TimeSpan remainingTime = finalTime - DateTime.UtcNow;
                    await Task.Delay((int)remainingTime.TotalSeconds * 1000);

                    for (int i = 0; i < workingCells.Count; i++)
                    {
                        workingCells[i].ToggleFactoryBoost(false);
                    }
                }
            }
        }
    }
}