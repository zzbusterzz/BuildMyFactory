using GoogleMobileAds.Api;
using System;
using UnityEngine;

namespace Test.FactoryRun.Core
{
    public class GameManager : MonoBehaviour
    {
        public static Action<GameState> OnUIGameAction;

        [SerializeField]
        private FactoryGrid factoryGrid;
        [SerializeField]
        private int defaultGemCount = 10;

        private GameState gameState;
        private SaveGameManager saveGameManager;
        private GameData gameData;

        private void Awake()
        {
            saveGameManager = new();
            gameData = new();
            UTCTimer.InitializeTime();
        }

        // Start is called before the first frame update
        private void Start()
        {
            OnUIGameAction += OnGameActionClick;
            MobileAds.Initialize(initStatus => 
            {
                AdReward.LoadRewardedAd();
            });
        }

        private void OnDestroy()
        {
            OnUIGameAction -= OnGameActionClick;
        }

        private void OnGameActionClick(GameState state)
        {
            gameState = state;

            if(state == GameState.EXITING)
            {
                saveGameManager.SaveState(gameData, factoryGrid.GetData());
            }
        }

        private void Update()
        {
            switch (gameState)
            {
                case GameState.MENU:
                    break;

                case GameState.NEW_GAME:                    
                    GameData.UpdateGem(10);
                    factoryGrid.Init();
                    gameState = GameState.RUNNING;
                    break;

                case GameState.CONTINUE:
                    factoryGrid.Init();
                    UpdateData();
                    gameState = GameState.RUNNING;
                    break;

                case GameState.RUNNING:
                    factoryGrid.Update();
                    break;
            }
        }


        void UpdateData()
        {
            SaveDataGroup saveDataGroup = saveGameManager.LoadState();
            gameData = saveDataGroup.GameData;
            DateTime savedTime = DateTime.Parse(saveDataGroup.saveTime);
            TimeSpan elapsedTime = UTCTimer.UtcNow - savedTime;

            FactorySettings fs = factoryGrid.Settings;
            //To calc gem if we want too accurately it will lead to more complex cases where we have to 
            //take each counter in factory and get elapsed time to get more accurate reading
            //for games which we dont need so
            //to simplify im taking approximation that is time elapsed will be applicable per building 
            //regardless of internal time passed

            foreach (FactoryData fd in saveDataGroup.FactoryDatas)
            {
                double spawnInstances = Math.Truncate(elapsedTime.TotalSeconds / (double) fs.GemSpawnDetails[fd.Level].Time);
                
                //Im ignoring double data but it will break if the spawn instances are way too large
                gameData.Gems += fs.GemSpawnDetails[fd.Level].GemsSpawned * spawnInstances;
                factoryGrid.LoadData(fd);
                //Load individual blocks of data
            }
        }

#if UNITY_EDITOR
        [ContextMenu("GenerateTestGrid")]
        void GenerateTestGrid()
        {
            factoryGrid.Init();
        }
#endif

    }
}