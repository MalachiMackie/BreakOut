using System;
using System.Collections.Generic;
using System.Linq;
using Shared;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    [SerializeField] private float initialKickForce = 4f;
    [SerializeField] private float maxKickForce = 8f;
    [SerializeField] private float minKickForce = 1f;
    [SerializeField] private Ball ballPrefab;
    [SerializeField] private float initialMovementSpeed = 3.5f;
    [SerializeField] private float maxMovementSpeed = 10f;
    [SerializeField] private float initialPlayerLength = 2.5f;
    [SerializeField] private float minPlayerLength = 0.5f;
    [SerializeField] private float maxPlayerLength = 4.5f;
    
    [Range(0f, 80f)]
    [SerializeField] private float maxBounceAngle = 30f;


    private float _kickForce;
    private float _movementSpeed;
    private bool _hasBalls;
    
    private void Awake()
    {
        Helpers.AssertIsNotNullOrQuit(ballPrefab, "Player.ballPrefab was not assigned");
        _movementSpeed = initialMovementSpeed;
        _kickForce = initialKickForce;
        var scale = transform.localScale;
        scale.x = initialPlayerLength;
        transform.localScale = scale;
    }

    private void Start()
    {
        _hasBalls = GetComponentsInChildren<Ball>().Any();
    }

    private void Update()
    {
        if (_hasBalls && Input.GetKeyDown(KeyCode.Space))
        {
            KickBalls();
        }

        if (!_hasBalls && Input.GetKeyDown(KeyCode.DoubleQuote))
        {
            NewBalls(1);
        }
    }

    private void FixedUpdate()
    {
        DoMovement();
    }

    private Vector3 DoBoundsCheck(Vector3 position)
    {
        Assert.IsNotNull(Camera.main);
        var halfBoardWidth = Camera.main.orthographicSize * Screen.width / Screen.height;

        var halfXScale = transform.localScale.x / 2;
        if (position.x - halfXScale < -halfBoardWidth)
        {
            position.x = -halfBoardWidth + halfXScale;
        }

        if (position.x + halfXScale > halfBoardWidth)
        {
            position.x = halfBoardWidth - halfXScale;
        }

        return position;
    }

    private void DoMovement()
    {
        var isLeftPressed = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
        var isRightPressed = Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.RightArrow);

        if (isLeftPressed == isRightPressed)
        {
            return;
        }

        var currentPosition = transform.position;
        if (isLeftPressed)
        {
            currentPosition.x -= _movementSpeed * Time.fixedDeltaTime;
        }
        else
        {
            currentPosition.x += _movementSpeed * Time.fixedDeltaTime;
        }

        transform.position = DoBoundsCheck(currentPosition);
    }

    /// <summary>
    /// Do some custom shit to make the ball bounce off in a different direction depending on which end of the player it hits
    /// Tried to use an ellipse collider, but was broken as shit and wouldn't work
    /// </summary>
    /// <param name="col"></param>
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (!col.gameObject.CompareTag(Tags.Ball))
        {
            return;
        }

        var otherRigidbody = col.gameObject.GetComponent<Rigidbody2D>();
        Assert.IsNotNull(otherRigidbody);
        
        var otherVelocity = otherRigidbody.velocity;
        // colliding with a ball, there should only be one collision point
        var contact = col.contacts.First();
        var localTransform = transform;
        var angle = (localTransform.position.x - contact.point.x) / localTransform.localScale.x * maxBounceAngle * 2 + 270;
        var rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        otherVelocity = Vector3.Normalize(rotation * Vector3.left) * otherVelocity.magnitude;
        otherRigidbody.velocity = otherVelocity;
    }

    public void NewBalls(int ballCount)
    {
        NewBalls(ballCount, ballPrefab);
    }

    /// <summary>
    /// Add an amount of balls to the player, placing them evenly accounting for player length and ball size
    /// </summary>
    /// <param name="ballCount">The number of balls to spawn</param>
    /// <param name="prefab">The prefab of the ball to use</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void NewBalls(int ballCount, Ball prefab)
    {
        var originalBallScale = prefab.transform.localScale;
        var localTransform = transform;
        var localScale = localTransform.localScale; 
        var ballLimit = (int)(localScale.x / originalBallScale.x);
        if (ballCount < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(ballCount), $"{nameof(ballCount)} must be at least 1");
        }
        if (ballCount > ballLimit)
        {
            throw new ArgumentOutOfRangeException(nameof(ballCount), $"This player can only fit {ballLimit} balls");
        }

        var balls = new List<Ball>();
        var placeableLength = (localScale.x - originalBallScale.x / 2) / localScale.x;
        var sectionLength = placeableLength / ballCount;
        var xStart = placeableLength / -2f;
        for (var section = 0; section < ballCount; section++)
        {
            var x = xStart + (sectionLength * section) + sectionLength / 2f;
            var ball = Instantiate(prefab, localTransform, true);
            var ballTransform = ball.transform;
            var ballScale = ballTransform.localScale;
            ballTransform.localPosition = new Vector3(x, 1 + (ballScale.y - 1) / 2, 0);
            var ballComponent = ball.GetComponent<Ball>();
            balls.Add(ballComponent);
        }
        
        StartCoroutine(Helpers.DoAfterMilliseconds(10, balls, x => x.ForEach(y => y.Spawned())));
        _hasBalls = true;
    }

    private void KickBalls()
    {
        var balls = GetComponentsInChildren<Ball>();

        foreach (var ball in balls)
        {
            var ballTransform = ball.transform;
            ballTransform.SetParent(null, true);
            ball.KickStart(Vector2.up, _kickForce);
        }

        _hasBalls = false;
    }

    private void ChangeXScale(float difference)
    {
        if (difference == 0)
        {
            return;
        }
        var scale = transform.localScale;

        scale.x = difference > 0
            ? Math.Min(maxPlayerLength, scale.x += difference)
            : Math.Max(minPlayerLength, scale.x += difference);
        
        transform.localScale = scale;
    }

    private void ChangeBallSpeed(float difference)
    {
        if (difference == 0)
        {
            return;
        }

        _kickForce = difference > 0
            ? Math.Min(maxKickForce, _kickForce += difference)
            : Math.Max(minKickForce, _kickForce += difference);

        var allBalls = FindObjectsOfType<Ball>();
        foreach (var ball in allBalls)
        {
            if (ball.transform.parent != null)
            {
                continue;
            }

            var ballRb = ball.GetComponent<Rigidbody2D>();
            var ballVelocity = ballRb.velocity;
            ballRb.velocity = Vector2.zero;
            ballRb.AddForce(ballVelocity.normalized * _kickForce, ForceMode2D.Impulse);
        }
    }

    public void GivePowerUp(PowerUpType type)
    {
        switch (type)
        {
            case PowerUpType.None:
                throw new InvalidOperationException($"{type} is not a valid power up");
            case PowerUpType.MultiBall:
                NewBalls(3, ballPrefab);
                break;
            case PowerUpType.PlayerSpeedUp:
                _movementSpeed = Math.Min(_movementSpeed + 2, maxMovementSpeed);
                break;
            case PowerUpType.PlayerSpeedDown:
                _movementSpeed = Math.Max(0, _movementSpeed - 2);
                break;
            case PowerUpType.PlayerIncreaseLength:
                ChangeXScale(1);
                break;
            case PowerUpType.PlayerDecreaseLength:
                ChangeXScale(-1);
                break;
            case PowerUpType.BallSpeedUp:
                ChangeBallSpeed(1);
                break;
            case PowerUpType.BallSpeedDown:
                ChangeBallSpeed(-1);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }

        AudioManager.Instance.PlayPowerUp();
    }
}