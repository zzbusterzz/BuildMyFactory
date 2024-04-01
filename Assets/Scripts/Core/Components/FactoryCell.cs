using Test.FactoryRun.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Test.FactoryRun.Core
{
    [RequireComponent(typeof(BoxCollider))]
    public class FactoryCell:MonoBehaviour,IPointerClickHandler
    {   
        public FactoryLevel Level { get => level; }

        [SerializeField]
        private FactoryVisualSettings settings;

        [SerializeField]
        private FactorySettings factorySettings;

        [SerializeField]
        private MeshRenderer meshRenderer;

        //use this block index to save data about this block
        private int blockIndex;
        private FactoryLevel level = FactoryLevel.LEVEL0;
        private GameObject visualRep;
        private ParticleSystem spawnEffect;

        private float timeToSpawn;
        private float curTime;
        private int curGemCount;
        private bool useBoost;
        private int boostMult;

        public void Init(int index)
        {
            blockIndex = index;
            spawnEffect = Instantiate(settings.GemGeneratedEffect, transform).GetComponent<ParticleSystem>();
            spawnEffect.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }

        public void UpdateSpawnData()
        {
            GemSpawnData gsd = factorySettings.GemSpawnDetails[Level + 1];
            curGemCount = gsd.GemsSpawned;
            timeToSpawn = gsd.Time;
        }

        public void ToggleFactoryBoost(bool useBoost, int mult = 1)
        {
            this.useBoost = useBoost;
            boostMult = mult;
        }

        //We will use grouped time and time via update loop of monobehaviour attached to this
        //This will prevent multiple update calls from mono
        public void UpdateTime()
        {
            curTime += Time.deltaTime;
            if(curTime >= timeToSpawn)
            {
                FactoryGrid.OnGemSpawned.Invoke(useBoost? curGemCount * boostMult : curGemCount);
                spawnEffect.Play();
                curTime = 0;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            //Dont allow buildings who are fully upgraded
            if (factorySettings.BuildingCostPerLevel.ContainsKey(level + 1))
            {
                meshRenderer.material.color = settings.TileSelectedColor;
                int cost = factorySettings.BuildingCostPerLevel[Level + 1];
                UpgradeUI.DisplayUpgradeUI.Invoke(cost, this);
            }
        }

        public void OnUpdateCompleted()
        {
            DeselectTile();

            if (visualRep != null)
            {
                //TODO: Pool object
                //Do not use this in actual code
                //I hate it
                GameObject.Destroy(visualRep);
            }
            level++;
            visualRep = GameObject.Instantiate(settings.FactoryInstance[Level].gameObject,
                                                transform);
            visualRep.transform.localPosition = Vector3.zero;
        }

        public void DeselectTile()
        {
            meshRenderer.material.color = settings.TileNormalColor;
        }

        #region SAVE_LOAD
        public FactoryData GetCurrentData()
        {
            FactoryData data = new FactoryData();
            data.Level = level;
            data.CurTime = curTime;
            data.BlockIndex = blockIndex;
            return data;
        }

        public void SetFactoryData(FactoryData data)
        {
            level = data.Level; 
            curTime = data.CurTime;

            GemSpawnData gsd = factorySettings.GemSpawnDetails[Level];
            curGemCount = gsd.GemsSpawned;
            timeToSpawn = gsd.Time;

            DeselectTile();
            if (visualRep != null)
            {
                //TODO: Pool object
                //Do not use this in actual code
                //I hate it
                GameObject.Destroy(visualRep);
            }
            visualRep = GameObject.Instantiate(settings.FactoryInstance[Level].gameObject,
                                                transform);
            visualRep.transform.localPosition = Vector3.zero;
        }
        #endregion
    }
}