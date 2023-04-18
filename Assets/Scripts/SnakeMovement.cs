using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SnakeMovement : MonoBehaviour
{

    private Snake _snake;
    private SnakeBody _snakeBody;
    private SnakeHand _snakeHand;

    [HideInInspector] public Vector2 SpawnVelocity;
    [HideInInspector] public Vector2 Movement;
    [HideInInspector] public float MoveSpeed;
    [HideInInspector] public KeyCode[] _moveKeys;

    private Rigidbody2D _rigidbody;

    private float _moveCooldown;
    private TimeSpan _moveCooldownTimeSpan;
    private DateTime _lastMove;
    private Vector2 _nextMove;


    private void Start()
    {
        _snake = GetComponent<Snake>();
        _snakeBody = GetComponent<SnakeBody>();
        _snakeHand = GetComponent<SnakeHand>();

        _rigidbody = GetComponent<Rigidbody2D>();

        MoveSpeed = SnakeManager.MoveSpeed;
        _moveCooldown = SnakeManager.MoveCooldown;

        Movement = SpawnVelocity;
        _moveCooldownTimeSpan = TimeSpan.FromSeconds(_moveCooldown);
    }

    private void ChangePlayerDirection(Vector2 movement)
    {
        if (DateTime.UtcNow - _lastMove > _moveCooldownTimeSpan)
        {
            Movement = movement;
            _nextMove = Vector2.zero;

            _snakeBody.CreateBody();

            _lastMove = DateTime.UtcNow;
        }
        else
        {
            _nextMove = movement;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(_moveKeys[0]) && Movement.y == 0) { ChangePlayerDirection(Vector2.up); }
        if (Input.GetKeyDown(_moveKeys[1]) && Movement.x == 0) { ChangePlayerDirection(Vector2.left); }
        if (Input.GetKeyDown(_moveKeys[2]) && Movement.y == 0) { ChangePlayerDirection(Vector2.down); }
        if (Input.GetKeyDown(_moveKeys[3]) && Movement.x == 0) { ChangePlayerDirection(Vector2.right); }

        if (DateTime.UtcNow - _lastMove > _moveCooldownTimeSpan && _nextMove != Vector2.zero) ChangePlayerDirection(_nextMove);
    }

    private void FixedUpdate()
    {
        _rigidbody.MovePosition(_rigidbody.position + Movement * MoveSpeed * Time.fixedDeltaTime);
    }

}