using System;
using Managers;
using UnityEngine;

namespace Menus
{
    public class StartMenu : MonoBehaviour
    {
        private void Start()
        {
            GameManager.Instance.GameStarted += OnGameStarted;
        }

        private void OnGameStarted(object sender, EventArgs e)
        {
            gameObject.SetActive(false);
        }

        public void StartGame()
        {
            GameManager.Instance.StartGame();
        }

        public void Quit()
        {
            GameManager.Instance.Quit();
        }

        private void OnDestroy()
        {
            GameManager.Instance.GameStarted -= OnGameStarted;
        }
    }
}
