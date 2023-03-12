using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SnakeManager : MonoBehaviour
{
    public static SnakeManager Instance;

    [SerializeField] private int _playerCount;
    public static int SnakeCount;
    [SerializeField] private uint _defaultSnakeLength;
    public static uint DefaultSnakeLength;
    [SerializeField] private float _defaultMoveSpeed;
    public static float MoveSpeed;
    [SerializeField] uint _pointsToGrow;
    public static uint PointsToGrow;

    [SerializeField] private GameObject _snakePrefab;

    private void Awake()
    {
        Instance = this;
        GameManager.OnGameStateChanged += GameManagerOnGameStateChanged;
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= GameManagerOnGameStateChanged;
    }

    private void GameManagerOnGameStateChanged(GameState state)
    {
        Debug.Log("State chenge detected by SnakeManager");
    }

    private void Start()
    {
        SnakeCount = _playerCount;
        DefaultSnakeLength = _defaultSnakeLength;
        MoveSpeed = _defaultMoveSpeed;
        PointsToGrow = _pointsToGrow;

        for (int i = 0; i < SnakeCount; i++)
        {
            GameObject snake = _snakePrefab;

            SnakeMovement snakeMovement = snake.GetComponent<SnakeMovement>();
            snakeMovement.PlayerID = i;
            snakeMovement.PlayerCount = _playerCount;

            Camera camera = snake.GetComponentInChildren<Camera>();
            Vector3 snakePosition = Vector3.zero;
            switch (i)
            {
                case 0:
                    snakePosition = new Vector3(0f, 1f, 0f);
                    snakeMovement.InitialVelocityOverride = new Vector2(0f, 1f);
                    snakeMovement._keybinds = new KeyCode[] { KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.Q, KeyCode.E, KeyCode.E };

                    if (SnakeCount == 1) camera.rect = new Rect(0f, 0f, 1f, 1f);
                    if (SnakeCount == 2) camera.rect = new Rect(0f, 0f, 0.4975f, 1f);
                    if (SnakeCount > 2) camera.rect = new Rect(0f, 0f, 0.4975f, 0.495f);
                    break;

                case 1:
                    snakePosition = new Vector3(1f, 0f, 0f);
                    snakeMovement.InitialVelocityOverride = new Vector2(1f, 0f);
                    snakeMovement._keybinds = new KeyCode[] { KeyCode.UpArrow, KeyCode.LeftArrow, KeyCode.DownArrow, KeyCode.RightArrow, KeyCode.RightControl, KeyCode.RightShift, KeyCode.RightShift };

                    if (SnakeCount == 2) camera.rect = new Rect(0.5025f, 0f, 0.4975f, 1f);
                    if (SnakeCount > 2) camera.rect = new Rect(0.5025f, 0f, 0.4975f, 0.495f);
                    break;

                case 2:
                    snakePosition = new Vector3(0f, -1f, 0f);
                    snakeMovement.InitialVelocityOverride = new Vector2(0f, -1f);
                    snakeMovement._keybinds = new KeyCode[] { KeyCode.I, KeyCode.J, KeyCode.K, KeyCode.L, KeyCode.U, KeyCode.O, KeyCode.O };

                    camera.rect = new Rect(0f, 0.505f, 0.498f, 0.495f);
                    break;

                case 3:
                    snakePosition = new Vector3(-1f, 0f, 0f);
                    snakeMovement.InitialVelocityOverride = new Vector2(-1f, 0f);
                    snakeMovement._keybinds = new KeyCode[] { KeyCode.T, KeyCode.F, KeyCode.G, KeyCode.H, KeyCode.R, KeyCode.Y, KeyCode.Z };

                    camera.rect = new Rect(0.5025f, 0.505f, 0.4975f, 0.495f);
                    break;
            }

            Instantiate(snake, snakePosition, Quaternion.identity);
        }
    }

}
