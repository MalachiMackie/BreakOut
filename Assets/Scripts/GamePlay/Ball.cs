using Managers;
using Shared;
using UnityEngine;

namespace GamePlay
{
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class Ball : MonoBehaviour
    {
        private Rigidbody2D _rigidbody;
        private bool _spawnFinished;

        // Start is called before the first frame update
        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _spawnFinished = transform.parent == null;
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            if (col.gameObject.CompareTag(Tags.Player) && _spawnFinished)
            {
                var currentVelocity = _rigidbody.velocity;
                currentVelocity = (transform.position - col.transform.position).normalized * currentVelocity.magnitude;
                _rigidbody.velocity = currentVelocity;
                Bounced();
            }
        }

        public void Bounced()
        {
            GameManager.Instance.BallBounced();   
        }

        public void Spawned()
        {
            _spawnFinished = true;
        }

        /// <summary>
        /// Kicks the balls in a given direction with a given force
        /// </summary>
        /// <param name="normalizedDirection">normalized direction to kick start the ball</param>
        /// <param name="force"></param>
        public void KickStart(Vector2 normalizedDirection, float force)
        {
            _rigidbody.AddForce(normalizedDirection * force, ForceMode2D.Impulse);
        }

        public void Crashed()
        {
            GameManager.Instance.BallCrashed(this);
            Destroy(gameObject);
        }
    }
}
