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
        }

        private void Update()
        {
            switch (gameState)
            {
                case GameState.MENU:
                    break;

                case GameState.NEW_GAME:
                    GameData gameData = new GameData();
                    GameData.UpdateGem(10);
                    factoryGrid.Init();
                    gameState = GameState.RUNNING;
                    break;

                case GameState.CONTINUE:
                    factoryGrid.Init();
                    gameState = GameState.RUNNING;
                    break;

                case GameState.RUNNING:
                    factoryGrid.Update();
                    break;
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