using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Test.FactoryRun.Core
{
    [Serializable]
    public class FactoryGrid
    {
        public static Func<FactoryCell, bool> UpgradeCell;
        public static Action<FactoryCell> UpgradeCellForced;
        public static Action<int> OnGemSpawned;

        public FactorySettings Settings { get => settings; }

        [SerializeField]
        private FactorySettings settings;

        [SerializeField]
        private IngameCurPurchaseItems purchaseItems;

        private List<FactoryCell> workingCells = new List<FactoryCell>();
        private Array maxFactoryLevel = Enum.GetValues(typeof(FactoryLevel));
        private CancellationTokenSource source;
        private bool boostIsRunning;

        public void Init()
        {
            UpgradeCell += TryUpgradeCell;
            OnGemSpawned += UpdateGemCount;
            UpgradeCellForced += UpgradeCellNoCost;

            FactorySlotHolder.InitBlocks.Invoke(Settings.SlotSize, Settings.InitFactoryLocationIndex);

            purchaseItems.BoostDetails[BoostType.FACTORY_BOOST].OnBoostApplied += ApplyBoostToSystem;

            ResumeBoostTimer();
        }

        ~FactoryGrid() 
        {
            UpgradeCell -= TryUpgradeCell;
            OnGemSpawned -= UpdateGemCount;
            if(source != null) source.Cancel();
        }

        private bool TryUpgradeCell(FactoryCell cell)
        {
            //Check if factory has reached max level
            //Check if building cost is available in config
            //Check if sufficient number of gems available
            if ((int)cell.Level < maxFactoryLevel.Length - 1 &&
               Settings.BuildingCostPerLevel.ContainsKey(cell.Level + 1) &&
                GameData.UpdateGem(-Settings.BuildingCostPerLevel[cell.Level+1]))
            {
                if(cell.Level == FactoryLevel.LEVEL0)
                {
                    workingCells.Add(cell);

                    if (boostIsRunning)
                    {
                        cell.ToggleFactoryBoost(true, purchaseItems.BoostDetails[BoostType.FACTORY_BOOST].EffectBoostMult);
                    }
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
            boostIsRunning = true;
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

        private void UpgradeCellNoCost(FactoryCell cell)
        {
            workingCells.Add(cell);
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
            source = new CancellationTokenSource();
            await Task.Delay((int)(timer * 1000), source.Token);
            
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
                if (finalTime > UTCTimer.UtcNow)
                {
                    TimeSpan remainingTime = finalTime - UTCTimer.UtcNow;

                    for (int i = 0; i < workingCells.Count; i++)
                    {
                        workingCells[i].ToggleFactoryBoost(true, purchaseItems.BoostDetails[BoostType.FACTORY_BOOST].EffectBoostMult);
                    }
                    boostIsRunning = true;
                    await Task.Delay((int)remainingTime.TotalSeconds * 1000, source.Token);
                    boostIsRunning = false;
                    for (int i = 0; i < workingCells.Count; i++)
                    {
                        workingCells[i].ToggleFactoryBoost(false);
                    }
                }
            }
        }

        public FactoryData[] GetData()
        {
            FactoryData[] buildingData = new FactoryData[workingCells.Count];
            for(int i = 0; i < workingCells.Count; ++i)
            {
                buildingData[i] = workingCells[i].GetCurrentData();
            }
            return buildingData;
        }

        public void LoadData(FactoryData factoryData)
        {
            FactorySlotHolder.LoadDataInBlock(factoryData);
        }
    }
}