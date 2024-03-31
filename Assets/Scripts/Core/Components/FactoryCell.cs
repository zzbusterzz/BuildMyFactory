using System;
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

        private float timeToSpawn;
        private float curTime;
        private int curGemCount;


        public void Init(int index)
        {
            blockIndex = index;
        }

        public void UpdateSpawnData()
        {
            GemSpawnData gsd = factorySettings.GemSpawnDetails[Level + 1];
            curGemCount = gsd.GemsSpawned;
            timeToSpawn = gsd.Time;
        }

        //We will use grouped time and time via update loop of monobehaviour attached to this
        //This will prevent multiple update calls from mono
        public void UpdateTime()
        {
            curTime += Time.deltaTime;
            if(curTime >= timeToSpawn)
            {
                FactoryGrid.OnGemSpawned.Invoke(curGemCount);
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
            //Allow if max upgrade not reached
            if (visualRep != null)
            {
                //TODO: Pool object
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
    }
}