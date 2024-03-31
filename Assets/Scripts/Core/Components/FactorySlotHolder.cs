using System;
using System.Collections.Generic;
using UnityEngine;

namespace Test.FactoryRun.Core
{
    public class FactorySlotHolder : MonoBehaviour
    {
        public static Action<Vector2Int, int> InitBlocks;

        #region SERIALISED_FIELDS
        [SerializeField]
        private Transform gridStart;

        [SerializeField]
        private Vector2 factoryBlockSize = new Vector2(50, 50);

        [SerializeField]
        private Vector2 factoryBlockSpacing = new Vector2(0.2f, 0.2f);

        [SerializeField]
        private FactoryCell factoryBlockBasePrefab;
        #endregion

        #region PRIVATE_FIELDS
        private List<FactoryCell> factoryBlock = new ();
        #endregion

        private void Start()
        {
            InitBlocks += Init;
        }

        private void OnDestroy()
        {
            InitBlocks -= Init;
        }

        private void Init(Vector2Int slotSize, int initCellWithBuilding)
        {
            int totalBlocks = slotSize.x * slotSize.y;
            Vector2 blockHalf = new Vector2(factoryBlockSize.x / 2, factoryBlockSize.y / 2);
            for(int i = 0; i < totalBlocks; i++)
            {
                FactoryCell f = Instantiate(factoryBlockBasePrefab);
                f.Init(i);
                float col = i % slotSize.x;
                float row = i / slotSize.y;
                //Remember to always convert xyz to xoz since we wont utilise y in genration
                Vector3 pos = new Vector3(gridStart.position.x + blockHalf.x + (col * factoryBlockSpacing.x) + (col * factoryBlockSize.x),
                                            0.05f,
                                            gridStart.position.z - blockHalf.y - (row * factoryBlockSpacing.y) - (row * factoryBlockSize.y));
                f.transform.position = pos;
                f.transform.localScale = new Vector3(factoryBlockSize.x, factoryBlockSize.y, 1);
                factoryBlock.Add(f);

                if(initCellWithBuilding == i)
                {
                    //Forcefully instantiate bldg on this index
                    FactoryGrid.UpgradeCell(f);
                }
            }

            //for (int j = 0; j < slotSize.y; j++)
            //{
            //    for (int i = 0; i < slotSize.x; i++) 
            //    {
            //        FactoryCell f = Instantiate(factoryBlockBasePrefab);
            //        f.Init(j * slotSize.y + i);
            //        //Remember to always convert xyz to xoz since we wont utilise y in genration
            //        Vector3 pos = new Vector3(gridStart.position.x + blockHalf.x + (i * factoryBlockSpacing.x) + (i * factoryBlockSize.x),
            //                                  0.05f,
            //                                  gridStart.position.z - blockHalf.y - (j * factoryBlockSpacing.y) - (j * factoryBlockSize.y));
            //        f.transform.position = pos;
            //        f.transform.localScale = new Vector3(factoryBlockSize.x, factoryBlockSize.y, 1);
            //        factoryBlock.Add(f);
            //    }
            //}
        }

#if UNITY_EDITOR
        [ContextMenu("ClearGrid")]
        private void ClearGrid()
        {
            for (int i = factoryBlock.Count -1; i >= 0; i--)
            {
                DestroyImmediate(factoryBlock[i].gameObject);
            }
        }
#endif
    }
}