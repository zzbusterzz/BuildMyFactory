using Test.FactoryRun.Core;
using UnityEngine;

namespace Test.FactoryRun.UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject mainMenuPanel;
        [SerializeField]
        private GameObject gamePanel;

        [SerializeField]
        private GameObject continueButton;

        private void Start()
        {
            mainMenuPanel.SetActive(true);
            gamePanel.SetActive(false);
            continueButton.SetActive(SaveGameManager.IsSaveDataPresent.Invoke());
        }

        public void OnNewClick()
        {
            GameManager.OnUIGameAction(GameState.NEW_GAME);
            mainMenuPanel.SetActive(false);
            gamePanel.SetActive(true);
        }

        public void OnContinueClick()
        {
            GameManager.OnUIGameAction(GameState.CONTINUE);
            mainMenuPanel.SetActive(false);
            gamePanel.SetActive(true);
        }

        public void OnExitClick()
        {
            GameManager.OnUIGameAction(GameState.EXITING);
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }
    }
}


