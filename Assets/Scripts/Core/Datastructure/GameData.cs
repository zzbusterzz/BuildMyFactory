using System;
using Test.FactoryRun.UI;

namespace Test.FactoryRun.Core
{
    [Serializable]
    public class GameData
    {
        public static Func<int, bool> UpdateGem;
        
        public double Gems;

        public GameData() 
        {
            UpdateGem = UpdateGems;
        }

        private bool UpdateGems(int gemCount)
        {
            Gems += gemCount;
            if(Gems < 0)
            {
                Gems -= gemCount;
                GameUI.updateGemCountUI.Invoke(Gems);
                return false;
            }
            GameUI.updateGemCountUI.Invoke(Gems);
            return true;
        }
    }
}