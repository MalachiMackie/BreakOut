using System;
using Shared;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class PowerUp : MonoBehaviour
{
    [SerializeField] private PowerUpType type;

    [SerializeField] private Color positivePowerUpTint = Color.green;
    [SerializeField] private Color negativePowerUpTint = Color.red;

    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
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

    public void SetType(PowerUpType newType)
    {
        type = newType;
        UpdateColorFromPowerUp();
    }

    private void UpdateColorFromPowerUp()
    {
        Color newColor;
        switch (GetPowerUpCharge(type))
        {
            case PowerUpCharge.Positive:
                newColor = positivePowerUpTint;
                break;
            case PowerUpCharge.Negative:
                newColor = negativePowerUpTint;
                break;
            case PowerUpCharge.None:
            default:
                return;
        }

        _spriteRenderer.color = newColor;
    }

    private static PowerUpCharge GetPowerUpCharge(PowerUpType type)
    {
        switch (type)
        {
            case PowerUpType.MultiBall:
            case PowerUpType.PlayerSpeedUp:
            case PowerUpType.PlayerIncreaseLength:
            case PowerUpType.BallSpeedDown:
                return PowerUpCharge.Positive;
            case PowerUpType.BallSpeedUp:
            case PowerUpType.PlayerSpeedDown:
            case PowerUpType.PlayerDecreaseLength:
                return PowerUpCharge.Negative;
            case PowerUpType.None:
            default:
                return PowerUpCharge.None;
        }
    }
}

public enum PowerUpType
{
    None,
    MultiBall,
    PlayerSpeedUp,
    PlayerSpeedDown,
    PlayerIncreaseLength,
    PlayerDecreaseLength,
    BallSpeedUp,
    BallSpeedDown
}

public enum PowerUpCharge
{
    None,
    Positive,
    Negative
}