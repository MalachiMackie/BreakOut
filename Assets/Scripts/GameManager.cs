using System.Linq;
using Shared;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public GameObject powerUpPrefab;
    public float ballDropForce = 2f;
    public Player player;

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

    public void DropPowerUp(Vector3 position, PowerUpType type)
    {
        var powerUp = Instantiate(powerUpPrefab, position, Quaternion.identity);
        var powerUpScript = powerUp.GetComponent<PowerUp>();
        powerUpScript.type = type;
        var powerUpRb = powerUp.GetComponent<Rigidbody2D>();
        powerUpRb.AddForce(new Vector2(0, -1) * ballDropForce, ForceMode2D.Impulse);
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