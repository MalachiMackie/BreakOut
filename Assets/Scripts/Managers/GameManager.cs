using System;
using System.Collections.Generic;
using System.Linq;
using Shared;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GameObject powerUpPrefab;
    [SerializeField] private Player player;

    public event EventHandler GamePaused;
    public event EventHandler<GameState> GameFinished;
    public event EventHandler GameStarted;

    private GameState _gameState;

    private void Start()
    {
        _gameState = GameState.StartMenu;
        Helpers.AssertIsNotNullOrQuit(powerUpPrefab, "GameManager.powerUpPrefab was not assigned");
        Helpers.AssertIsNotNullOrQuit(player, "GameManager.player was not assigned");
        StartGame();
    }
    
    // Update is called once per frame
    private void Update()
    {
        if (_gameState != GameState.Paused && Input.GetKeyDown(KeyCode.Escape))
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

    private void StartGame()
    {
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
        
        player.NewBalls(1);
        _gameState = GameState.Playing;
        GameStarted?.Invoke(this, EventArgs.Empty);
    }

    public void BallCrashed(Ball crashedBall)
    {
        AudioManager.Instance.PlayBallCrash();
        var ballsRemaining = FindObjectsOfType<Ball>().Except(new [] {crashedBall});
        if (!ballsRemaining.Any())
        {
            PlayerLostLife();
        }
    }

    public void BallBounced()
    {
        AudioManager.Instance.PlayBallBounce();
    }

    private void PlayerDied()
    {
        _gameState = GameState.Lost;
        GameFinished?.Invoke(this, _gameState);
    }

    private void PlayerLostLife()
    {
        Debug.Log("You're Dead :(");
        PlayerDied();
    }
}

public enum GameState
{
    StartMenu,
    Playing,
    Paused,
    Won,
    Lost
}