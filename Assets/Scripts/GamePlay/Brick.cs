using System;
using Managers;
using Shared;
using UnityEngine;

namespace GamePlay
{
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class Brick : MonoBehaviour
    {
        [SerializeField] private int health;
        [SerializeField] private int initialHealth = 1;
        [SerializeField] private PowerUp powerUpPrefab;
        [SerializeField] private float powerUpDropForce = 3f;

        [SerializeField] private PowerUpType powerUpType;
        [SerializeField] private BrickPowerUpApplies powerUpApplies;

        private SpriteRenderer _spriteRenderer;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            Helpers.AssertIsNotNullOrQuit(_spriteRenderer, "Could not find Sprite Renderer component on Brick");
            
            SetHealth(initialHealth);
        }

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
            if (powerUpApplies is BrickPowerUpApplies.BrickDamaged or BrickPowerUpApplies.BrickDamagedOrDestroyed)
            {
                DropPowerUp();
            }
            SetHealth(health - damageLevel);
        }

        private void Die()
        {
            if (powerUpApplies is BrickPowerUpApplies.BrickDestroy or BrickPowerUpApplies.BrickDamagedOrDestroyed)
            {
                DropPowerUp();
            }
            Destroy(gameObject);
            GameManager.Instance.BrickDestroyed(this);
        }

        public void SetPowerUp(PowerUpType type, BrickPowerUpApplies applies)
        {
            powerUpType = type;
            powerUpApplies = applies;
        }

        public void SetHealth(int newHealth)
        {
            if (newHealth < 1)
            {
                Die();
                return;
            }
            health = newHealth;

            _spriteRenderer.color = health switch
            {
                <= 0 => throw new InvalidOperationException("Unreachable"),
                1 => BrickManager.Instance.oneHealthColor,
                2 => BrickManager.Instance.twoHealthColor,
                3 => BrickManager.Instance.threeHealthColor,
                >=4 => BrickManager.Instance.fourOrMoreHealthColor 
            };

        }
    }

    public enum BrickPowerUpApplies
    {
        None,
        BrickDamaged,
        BrickDestroy,
        BrickDamagedOrDestroyed
    }
}