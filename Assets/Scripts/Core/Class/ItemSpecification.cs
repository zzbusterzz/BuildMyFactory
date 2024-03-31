using System;

namespace Test.FactoryRun.Core
{
    [Serializable]
    public class ItemSpecification
    {
        public float DurationOfEffect;
        public int EffectBoostMult;
        public int EffectBoostIncrementPerSec;
        public string Title;
        public string Description;
        public int Cost;
        public Action OnBoostApplied;
    }
}