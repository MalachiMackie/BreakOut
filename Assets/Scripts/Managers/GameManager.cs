using System;
using System.Collections.Generic;
using System.Linq;
using GamePlay;
using Shared;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Managers
{
    public class GameManager : Singleton<GameManager>
    {
        [SerializeField] private GameObject powerUpPrefab;
        private Player _player;

        public event EventHandler GamePaused;
        public event EventHandler<GameState> GameFinished;
        public event EventHandler GameStarted;

        private GameState _gameState;

        public readonly Guid _id = Guid.NewGuid();

        public bool IsPlaying => _gameState == GameState.Playing;

        private void Awake()
        {
            var otherGameManagers = FindObjectsOfType<GameManager>();
            if (otherGameManagers.Any(x => x._id != _id))
            {
                Destroy(gameObject);
                return;
            }
            
            Debug.Log(_id);

            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            _gameState = GameState.StartMenu;
            Helpers.AssertIsNotNullOrQuit(powerUpPrefab, "GameManager.powerUpPrefab was not assigned");
            SetupLevel();
            Time.timeScale = 0f;
        }

        // Update is called once per frame
        private void Update()
        {
            if (_gameState == GameState.Playing && Input.GetKeyDown(KeyCode.Escape))
            {
                Pause();
            }
        }

        private void Pause()
        {
            _gameState = GameState.Paused;
            Time.timeScale = 0f;
            GamePaused?.Invoke(this, EventArgs.Empty);
        }

        public void Unpause()
        {
            _gameState = GameState.Playing;
            Time.timeScale = 1f;
        }

        public void Quit()
        {
            Helpers.Quit();
        }

        private void SetupLevel()
        {
            _player = FindObjectOfType<Player>();
            Helpers.AssertIsNotNullOrQuit(_player, "Could not find player in level");
            var levelDetails = FindObjectOfType<LevelDetails>();
            var bricks = FindObjectsOfType<Brick>();
            var poweredUpBricks = new HashSet<int>();

            foreach (var powerUp in levelDetails.powerUps)
            {
                var bricksLeft = powerUp.count;
                while (bricksLeft > 0)
                {
                    if (poweredUpBricks.Count >= bricks.Length)
                    {
                        throw new InvalidOperationException(
                            "The number of power ups is greater than the number of bricks in the level");
                    }

                    int index;
                    do
                    {
                        index = Random.Range(0, bricks.Length);
                    } while (poweredUpBricks.Contains(index));

                    poweredUpBricks.Add(index);
                    bricksLeft--;
                    bricks[index].SetPowerUp(powerUp.type, powerUp.applies);
                }
            }

            _player.NewBalls(1);
        }

        public void Restart()
        {
            var currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadSceneAsync(currentScene.buildIndex).completed += _ =>
            {
                StartCoroutine(Helpers.DoNextFrame(() =>
                {
                    SetupLevel();
                    StartGame();
                }));
            };
        }

        public void StartGame()
        {
            _gameState = GameState.Playing;
            GameStarted?.Invoke(this, EventArgs.Empty);
            Time.timeScale = 1f;
        }

        public void BrickDestroyed(Brick destroyedBrick)
        {
            var bricks = FindObjectsOfType<Brick>();
            if (!bricks.Except(new[] {destroyedBrick}).Any())
            {
                PlayerWon();
            }
        }

        public void BallCrashed(Ball crashedBall)
        {
            AudioManager.Instance.PlayBallCrash();
            var ballsRemaining = FindObjectsOfType<Ball>().Except(new[] {crashedBall});
            if (!ballsRemaining.Any())
            {
                PlayerDied();
            }
        }

        public void BallBounced()
        {
            AudioManager.Instance.PlayBallBounce();
        }

        private void PlayerDied()
        {
            _gameState = GameState.Lost;
            GameEnded();
        }

        private void PlayerWon()
        {
            _gameState = GameState.Won;
            GameEnded();
        }

        private void GameEnded()
        {
            Time.timeScale = 0f;
            GameFinished?.Invoke(this, _gameState);
        }
    }

    public enum GameState
    {
        None,
        StartMenu,
        Playing,
        Paused,
        Won,
        Lost
    }
}