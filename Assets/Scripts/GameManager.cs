using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static event Action<GameState> OnGameStateChanged;
    public Rect GameArea;
    public GameState State;

    [SerializeField] private Sprite _wallSprite;
    [SerializeField] private Transform _wallParent;
    [SerializeField] private Transform _collectibleParent;
    [SerializeField] private Sprite _pointSprite;
    [SerializeField] private Sprite _coinSprite;
    [SerializeField] private float _pointRepopulationRate;
    [SerializeField] private float _coinRepopulationRate;

    private int _snakeCount;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _snakeCount = SnakeManager.SnakeCount;

        _pointRepopulationRate = 1 / (_pointRepopulationRate - (_pointRepopulationRate * 3/4) + (_pointRepopulationRate / 4) * _snakeCount);
        _coinRepopulationRate = 1 / (_coinRepopulationRate - (_coinRepopulationRate * 3/4) + (_coinRepopulationRate / 4) * _snakeCount);

        UpdateGameState(GameState.Round);
    }

    public void UpdateGameState(GameState newState)
    {
        State = newState;

        CancelInvoke("SpawnNewPoint");
        CancelInvoke("SpawnNewCoin");

        switch (newState)
        {
            case GameState.Menu:
                break;
            case GameState.Round:
                NewRoundHandler();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        OnGameStateChanged?.Invoke(newState);
    }

    private void NewRoundHandler()
    {
        for (int i = 0; i < 4; i++)
        {
            GameObject wall = new GameObject("Wall " + i, typeof(SpriteRenderer), typeof(BoxCollider2D));
            wall.transform.parent = _wallParent;

            SpriteRenderer wallRenderer = wall.GetComponent<SpriteRenderer>();
            wallRenderer.sprite = _wallSprite;

            BoxCollider2D boxCollider = wall.GetComponent<BoxCollider2D>();
            boxCollider.size = new Vector2(1f, 1f);

            switch (i)
            {
                case 0:
                    wall.transform.localScale = new Vector3(1f, 31f + 10f * _snakeCount, 1f);
                    wall.transform.position = new Vector3(-15f - 5f * _snakeCount, 0f, 0f);
                    break;
                case 1:
                    wall.transform.localScale = new Vector3(1f, 31f + 10f * _snakeCount, 1f);
                    wall.transform.position = new Vector3(15f + 5f * _snakeCount, 0f, 0f);
                    break;
                case 2:
                    wall.transform.localScale = new Vector3(31f + 10f * _snakeCount, 1f, 1f);
                    wall.transform.position = new Vector3(0f, -15f - 5f * _snakeCount, 0f);
                    break;
                case 3:
                    wall.transform.localScale = new Vector3(31f + 10f * _snakeCount, 1f, 1f);
                    wall.transform.position = new Vector3(0f, 15f + 5f * _snakeCount, 0f);
                    break;
            }
        }

        int coinsToSpawn = 300 + 50 * _snakeCount;
        for (int i = 0; i < coinsToSpawn; i++)
        {
            SpawnNewPoint();
        }

        InvokeRepeating("SpawnNewPoint", 0f, _pointRepopulationRate);
        InvokeRepeating("SpawnNewCoin", 10f, _coinRepopulationRate);
    }

    private void SpawnNewPoint()
    {
        Debug.Log("New Point Spawned");

        GameObject point = new GameObject("Point", typeof(SpriteRenderer), typeof(CircleCollider2D));
        point.transform.parent = _collectibleParent;
        point.layer = 10;
        point.tag = "Point";

        SpriteRenderer pointRenderer = point.GetComponent<SpriteRenderer>();
        pointRenderer.sprite = _pointSprite;

        CircleCollider2D circleCollider = point.GetComponent<CircleCollider2D>();
        circleCollider.radius = 1f;
        point.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        float minimumPositionX = -15f - 5f * _snakeCount + 0.5f;
        float maximumPositionX = 15f + 5f * _snakeCount - 0.5f;
        float minimumPositionY = 15f + 5f * _snakeCount - 0.5f;
        float maximumPositionY = -15f - 5f * _snakeCount + 0.5f;
        point.transform.position = GetRandomPositionInBounds(minimumPositionX, maximumPositionX, minimumPositionY, maximumPositionY);
    }

    private void SpawnNewCoin()
    {
        Debug.Log("New Coin Spawned");

        GameObject point = new GameObject("Coin", typeof(SpriteRenderer), typeof(CircleCollider2D));
        point.transform.parent = _collectibleParent;
        point.layer = 10;
        point.tag = "Coin";

        SpriteRenderer pointRenderer = point.GetComponent<SpriteRenderer>();
        pointRenderer.sprite = _coinSprite;

        CircleCollider2D circleCollider = point.GetComponent<CircleCollider2D>();
        circleCollider.radius = 0.9f;
        point.transform.localScale = new Vector3(0.23f, 0.23f, 0.23f);

        float minimumPositionX = -15f - 5f * _snakeCount + 0.5f;
        float maximumPositionX = 15f + 5f * _snakeCount - 0.5f;
        float minimumPositionY = 15f + 5f * _snakeCount - 0.5f;
        float maximumPositionY = -15f - 5f * _snakeCount + 0.5f;
        point.transform.position = GetRandomPositionInBounds(minimumPositionX, maximumPositionX, minimumPositionY, maximumPositionY);
    }

    private Vector2 GetRandomPositionInBounds(float minimumX, float maximumX, float minimumY, float maximumY)
    {
        Vector2 newPoint = new Vector2(UnityEngine.Random.Range(minimumX, maximumX), UnityEngine.Random.Range(minimumY, maximumY));
        return newPoint;
    }
}

public enum GameState
{
    Menu,
    Round
}