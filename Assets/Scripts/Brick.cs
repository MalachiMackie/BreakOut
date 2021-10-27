using Shared;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Brick : MonoBehaviour
{
    [SerializeField] private float health = 1;
    [SerializeField] private PowerUp powerUpPrefab;
    [SerializeField] private float powerUpDropForce = 3f;

    [SerializeField] private PowerUpType powerUpType;
    [SerializeField] private BrickPowerUpApplies powerUpApplies;

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag(Tags.Ball))
        {
            Damage(1);
            var ball = col.gameObject.GetComponent<Ball>();
            ball.Bounced();
        }
    }

    private void DropPowerUp()
    {
        if (powerUpType is PowerUpType.None)
        {
            return;
        }

        var localTransform = transform;
        var powerUp = Instantiate(powerUpPrefab, localTransform.position, localTransform.rotation);
        powerUp.SetType(powerUpType);
        var powerUpRb = powerUp.GetComponent<Rigidbody2D>();
        powerUpRb.AddForce(new Vector2(0, -1) * powerUpDropForce, ForceMode2D.Impulse);
    }

    private void Damage(int damageLevel)
    {
        health -= damageLevel;
        if (powerUpApplies is BrickPowerUpApplies.BrickDamaged || powerUpApplies is BrickPowerUpApplies.BrickDamagedOrDestroyed)
        {
            DropPowerUp();
        }
        if (health >= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (powerUpApplies is BrickPowerUpApplies.BrickDestroy || powerUpApplies is BrickPowerUpApplies.BrickDamagedOrDestroyed)
        {
            DropPowerUp();
        }
        Destroy(gameObject);
    }

    public void SetPowerUp(PowerUpType type, BrickPowerUpApplies applies)
    {
        powerUpType = type;
        powerUpApplies = applies;
    }
}

public enum BrickPowerUpApplies
{
    None,
    BrickDamaged,
    BrickDestroy,
    BrickDamagedOrDestroyed
}
