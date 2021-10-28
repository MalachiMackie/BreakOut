using System;
using Managers;
using Shared;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Menus
{
    public class EndMenu : MonoBehaviour
    {
        [FormerlySerializedAs("finishText")] [SerializeField] private Text finishTextElement;
        [SerializeField] private string playerWonText;
        [SerializeField] private string playerLostText;
        
        private void Awake()
        {
            Helpers.AssertIsNotNullOrQuit(finishTextElement, "EndMenu.finishText is not assigned");
            Helpers.AssertIsTrueOrQuit(!string.IsNullOrWhiteSpace(playerWonText), "EndMenu.playerWonText is not set");
            Helpers.AssertIsTrueOrQuit(!string.IsNullOrWhiteSpace(playerLostText), "EndMenu.playerLostText is not set");
        }

        private void Start()
        {
            GameManager.Instance.GameFinished += OnGameFinished;
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            GameManager.Instance.GameFinished -= OnGameFinished;
        }

        private void OnGameFinished(object sender, GameState state)
        {
            switch (state)
            {
                case GameState.Lost:
                    finishTextElement.text = playerLostText;
                    break;
                case GameState.Won:
                    finishTextElement.text = playerWonText;
                    break;
                case GameState.None:
                case GameState.StartMenu:
                case GameState.Playing:
                case GameState.Paused:
                default:
                    Helpers.AssertIsTrueOrQuit(false, $"Invalid game state {state}: Probably bug in GameManager");
                    break;
            }
            gameObject.SetActive(true);
        }

        public void Restart()
        {
            GameManager.Instance.Restart();
        }

        public void Quit()
        {
            GameManager.Instance.Quit();
        }
    }
}
