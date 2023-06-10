using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static event Action<GameState> OnGameStateChanged;
    [HideInInspector] public GameState State;

    [SerializeField] private Sprite _pointSprite;
    [SerializeField] private Sprite _coinSprite;
    [SerializeField] private float _pointRepopulationRate;
    [SerializeField] private float _coinRepopulationRate;

    private int _snakeCount;
    private int _snakeCountOnRoundStart;

    [HideInInspector] public float MinimumPositionX;
    [HideInInspector] public float MaximumPositionX;
    [HideInInspector] public float MinimumPositionY;
    [HideInInspector] public float MaximumPositionY;

    [HideInInspector] public int PointCount = 0;
    [HideInInspector] public int CoinCount = 0;
    [SerializeField] private int _maxPointCount;
    [SerializeField] private int _maxCoinCount;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        UpdateGameState(GameState.Menu);
    }

    public void UpdateGameState(GameState newState)
    {
        if (State != newState)
        {
            State = newState;

            CancelInvoke("SpawnNewPoint");
            CancelInvoke("SpawnNewCoin");

            switch (newState)
            {
                case GameState.Menu:
                    SceneManager.LoadScene("Assets/Scenes/Main Menu.unity");
                    break;
                case GameState.Round:
                    _snakeCountOnRoundStart = (int)GameObject.Find("/Canvas/Player Count").GetComponent<Slider>().value;
                    SceneManager.LoadScene("Assets/Scenes/Game.unity");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
            }

            OnGameStateChanged?.Invoke(newState);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Main Menu")
        {

        }
        else if (scene.name == "Game")
        {
            NewRoundHandler();
            SnakeManager.Instance.GameStart(_snakeCountOnRoundStart);
            GameObject.Find("Foreground").GetComponent<GameTimer>().StartRound();
        }
    }

    private void NewRoundHandler()
    {
        _snakeCount = SnakeManager.SnakeCount;

        _pointRepopulationRate = 1 / (_pointRepopulationRate - (_pointRepopulationRate * 3 / 4) + (_pointRepopulationRate / 4) * _snakeCount);
        _coinRepopulationRate = 1 / (_coinRepopulationRate - (_coinRepopulationRate * 3 / 4) + (_coinRepopulationRate / 4) * _snakeCount);

        for (int i = 0; i < 4; i++)
        {
            GameObject wall = new GameObject("Wall " + i, typeof(BoxCollider2D));

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

        MinimumPositionX = -15f - 5f * _snakeCount + 0.5f;
        MaximumPositionX = 15f + 5f * _snakeCount - 0.5f;
        MinimumPositionY = -15f - 5f * _snakeCount + 0.5f;
        MaximumPositionY = 15f + 5f * _snakeCount - 0.5f;

        GameObject.Find("Camera Bounds").transform.localScale = new Vector3(MaximumPositionX, MaximumPositionY, 1f);

        int coinsToSpawn = 100 + 50 * _snakeCount;
        for (int i = 0; i < coinsToSpawn; i++)
        {
            SpawnNewPoint();
        }

        InvokeRepeating("SpawnNewPoint", 0f, _pointRepopulationRate);
        InvokeRepeating("SpawnNewCoin", 10f, _coinRepopulationRate / 10);
    }

    private void SpawnNewPoint()
    {
        if (PointCount < _maxPointCount)
        {
            PointCount++;

            GameObject point = new GameObject("Point", typeof(SpriteRenderer), typeof(CircleCollider2D));
            point.layer = 10;
            point.tag = "Point";

            SpriteRenderer pointRenderer = point.GetComponent<SpriteRenderer>();
            pointRenderer.sprite = _pointSprite;

            CircleCollider2D circleCollider = point.GetComponent<CircleCollider2D>();
            circleCollider.radius = 1f;
            circleCollider.isTrigger = true;
            point.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

            point.transform.position = GetRandomPositionInBounds(MinimumPositionX, MaximumPositionX, MinimumPositionY, MaximumPositionY);
        }
    }

    private void SpawnNewCoin()
    {
        if (CoinCount < _maxCoinCount)
        {
            CoinCount++;

            GameObject point = new GameObject("Coin", typeof(SpriteRenderer), typeof(CircleCollider2D));

            point.layer = 10;
            point.tag = "Coin";

            SpriteRenderer pointRenderer = point.GetComponent<SpriteRenderer>();
            pointRenderer.sprite = _coinSprite;

            CircleCollider2D circleCollider = point.GetComponent<CircleCollider2D>();
            circleCollider.radius = 0.9f;
            circleCollider.isTrigger = true;
            point.transform.localScale = new Vector3(0.23f, 0.23f, 0.23f);

            point.transform.position = GetRandomPositionInBounds(MinimumPositionX, MaximumPositionX, MinimumPositionY, MaximumPositionY);
        }
    }

    private Vector2 GetRandomPositionInBounds(float minimumX, float maximumX, float minimumY, float maximumY)
    {
        Vector2 newPoint = new Vector2(UnityEngine.Random.Range(minimumX, maximumX), UnityEngine.Random.Range(minimumY, maximumY));
        return newPoint;
    }

    public void StartGameButton()
    { 
        UpdateGameState(GameState.Round); 
    }

    private void Update()
    {
        if (State == GameState.Round)
        {
            if (Input.GetKeyDown(KeyCode.Keypad7)) // End Round
            {
                GameObject.Find("Foreground").GetComponent<GameTimer>().GameTime = 1f;
            }
            if (Input.GetKeyDown(KeyCode.Keypad8)) // Reroll Items
            {
                SnakeManager.Instance.RerollAllHands();
            }
        }
    }

}

public enum GameState
{
    Menu,
    Round
}