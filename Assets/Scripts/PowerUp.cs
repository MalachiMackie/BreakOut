using System;
using Shared;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PowerUp : MonoBehaviour
{
    public PowerUpType type;
    
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag(Tags.Player))
        {
            var player = col.gameObject.GetComponent<Player>();
            player.GivePowerUp(type);
            Destroy(gameObject);
        }

        if (col.gameObject.CompareTag(Tags.KillZone))
        {
            Destroy(gameObject);
        }
    }
}

public enum PowerUpType
{
    None,
    MultiBall,
    SpeedUp,
    SpeedDown,
    IncreaseLength,
    DecreaseLength,
    BallSpeedUp,
    BallSpeedDown
}