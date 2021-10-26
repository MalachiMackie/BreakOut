using Shared;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Brick : MonoBehaviour
{
    public float health = 1;

    public PowerUpType powerUpType;
    public BrickPowerUpApplies powerUpApplies;

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag(Tags.Ball))
        {
            Damage(1);
            var ball = col.gameObject.GetComponent<Ball>();
            ball.Bounced();
        }
    }

    private void ApplyPowerUp()
    {
        if (powerUpType is PowerUpType.None)
        {
            return;
        }
        GameManager.Instance.DropPowerUp(transform.position, powerUpType);
    }

    private void Damage(int damageLevel)
    {
        health -= damageLevel;
        if (powerUpApplies is BrickPowerUpApplies.Damage || powerUpApplies is BrickPowerUpApplies.DamageOrDie)
        {
            ApplyPowerUp();
        }
        if (health >= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (powerUpApplies is BrickPowerUpApplies.Die || powerUpApplies is BrickPowerUpApplies.DamageOrDie)
        {
            ApplyPowerUp();
        }
        Destroy(gameObject);
    }
}

public enum BrickPowerUpApplies
{
    None,
    Damage,
    Die,
    DamageOrDie
}
