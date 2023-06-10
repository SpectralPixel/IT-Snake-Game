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

    public float MoveCooldown;
    private TimeSpan _moveCooldownTimeSpan;
    private DateTime _lastMove;
    private Vector2 _nextMove;

    public bool FreeMove = false;
    public bool Frozen = false;
    public bool Confused = false;


    private void Start()
    {
        _snake = GetComponent<Snake>();
        _snakeBody = GetComponent<SnakeBody>();
        _snakeHand = GetComponent<SnakeHand>();

        _rigidbody = GetComponent<Rigidbody2D>();

        MoveSpeed = SnakeManager.MoveSpeed;
        MoveCooldown = SnakeManager.MoveCooldown;

        Movement = SpawnVelocity;
        _moveCooldownTimeSpan = TimeSpan.FromSeconds(MoveCooldown);
    }

    private void ChangePlayerDirection(Vector2 movement)
    {
        if (!FreeMove)
        {
            if (DateTime.UtcNow - _lastMove > _moveCooldownTimeSpan)
            {
                if (!Confused) Movement = movement;
                else Movement = -movement;
                _nextMove = Vector2.zero;

                _snakeBody.CreateBody();

                _lastMove = DateTime.UtcNow;
            }
            else
            {
                _nextMove = movement;
            }
        }
        else
        {
            if (!Confused) Movement = movement;
            else Movement = -movement;
            _snakeBody.CreateBody();
        }
    }

    void Update()
    {
        if (!FreeMove)
        {
            if (Input.GetKeyDown(_moveKeys[0]) && Movement.y == 0) { ChangePlayerDirection(Vector2.up); }
            if (Input.GetKeyDown(_moveKeys[1]) && Movement.x == 0) { ChangePlayerDirection(Vector2.left); }
            if (Input.GetKeyDown(_moveKeys[2]) && Movement.y == 0) { ChangePlayerDirection(Vector2.down); }
            if (Input.GetKeyDown(_moveKeys[3]) && Movement.x == 0) { ChangePlayerDirection(Vector2.right); }
        }
        else
        {
            if (Input.GetKeyDown(_moveKeys[0])) { ChangePlayerDirection(Vector2.up); }
            if (Input.GetKeyDown(_moveKeys[1])) { ChangePlayerDirection(Vector2.left); }
            if (Input.GetKeyDown(_moveKeys[2])) { ChangePlayerDirection(Vector2.down); }
            if (Input.GetKeyDown(_moveKeys[3])) { ChangePlayerDirection(Vector2.right); }
        }

        if (DateTime.UtcNow - _lastMove > _moveCooldownTimeSpan && _nextMove != Vector2.zero) ChangePlayerDirection(_nextMove);
    }

    private void FixedUpdate()
    {
        if (!Frozen) _rigidbody.MovePosition(_rigidbody.position + Movement * MoveSpeed * Time.fixedDeltaTime);

        if (transform.position.x < GameManager.Instance.MinimumPositionX)
        {
            Debug.Log("X too negative");
            _snake.Respawn();
            transform.position = new Vector3(GameManager.Instance.MinimumPositionX + 1, transform.position.y, 0f);
        }
        if (transform.position.x > GameManager.Instance.MaximumPositionX)
        {
            Debug.Log("X too positive");
            _snake.Respawn();
            transform.position = new Vector3(GameManager.Instance.MaximumPositionX - 1, transform.position.y, 0f);
        }
        if (transform.position.y < GameManager.Instance.MinimumPositionY)
        {
            Debug.Log("Y too negative");
            _snake.Respawn();
            transform.position = new Vector3(transform.position.x, GameManager.Instance.MinimumPositionY + 1, 0f);
        }
        if (transform.position.y > GameManager.Instance.MaximumPositionY)
        {
            Debug.Log("Y too positive");
            _snake.Respawn();
            transform.position = new Vector3(transform.position.x, GameManager.Instance.MaximumPositionY - 1, 0f);
        }
    }

}