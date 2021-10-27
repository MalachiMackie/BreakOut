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

    private void Start()
    {
        Helpers.AssertIsNotNullOrQuit(powerUpPrefab, "GameManager.powerUpPrefab was not assigned");
        Helpers.AssertIsNotNullOrQuit(player, "GameManager.player was not assigned");
        StartGame();
    }
    
    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Helpers.Quit();
        }
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
    }

    public void BallCrashed()
    {
        AudioManager.Instance.PlayBallCrash();
        var ballsRemaining = FindObjectsOfType<Ball>();
        if (!ballsRemaining.Any())
        {
            PlayerLostLife();
        }
    }

    public void BallBounced()
    {
        AudioManager.Instance.PlayBallBounce();
    }

    private void PlayerLostLife()
    {
        Debug.Log("You're Dead :(");
    }
}