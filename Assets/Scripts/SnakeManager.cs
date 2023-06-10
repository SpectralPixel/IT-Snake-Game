using UnityEngine;

public class SnakeManager : MonoBehaviour
{
    public static SnakeManager Instance;

    public static GameObject[] Snakes;

    public static int SnakeCount = 2;
    public static uint DefaultSnakeLength;
    public static float MoveSpeed;
    public static uint PointsToGrow;
    public static float MoveCooldown;
    public static float PickupRadius;
    public static float CameraSize;
    public static LayerMask CollectibleLayerMask;
    public static int PlayerLayer;
    public static Sprite[] PowerupSprites;
    public static Sprite[] WeakPowerupSprites;

    [SerializeField] private uint _defaultSnakeLength;
    [SerializeField] private float _defaultMoveSpeed;
    [SerializeField] uint _pointsToGrow;
    [SerializeField] private float _moveCooldown;
    [SerializeField] private float _defaultPickupRadius;
    [SerializeField] private float _cameraSize;
    [Space]
    [SerializeField] private LayerMask _collectibleLayerMask;
    [SerializeField] private int _playerLayer;
    [Space]
    [SerializeField] private Sprite[] _powerupSprites;
    [SerializeField] private Sprite[] _weakPowerupSprites;

    [SerializeField] private GameObject _snakePrefab;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
    }

    public void GameStart(int playerCount)
    {
        SnakeCount = playerCount;
        DefaultSnakeLength = _defaultSnakeLength;
        PointsToGrow = _pointsToGrow;
        PickupRadius = _defaultPickupRadius;
        CameraSize = _cameraSize;

        MoveSpeed = _defaultMoveSpeed;
        MoveCooldown = _moveCooldown;

        CollectibleLayerMask = _collectibleLayerMask;
        PlayerLayer = _playerLayer;

        PowerupSprites = _powerupSprites;
        WeakPowerupSprites = _weakPowerupSprites;

        Snakes = new GameObject[SnakeCount];

        for (int i = 0; i < SnakeCount; i++)
        {
            GameObject snake = _snakePrefab;

            Snake snakeScript = snake.GetComponent<Snake>();
            SnakeMovement snakeMovement = snake.GetComponent<SnakeMovement>();
            SnakeHand snakeHand = snake.GetComponent<SnakeHand>();

            snakeScript.PlayerID = i;
            snakeScript.PlayerCount = SnakeCount;

            Camera camera = snake.GetComponentInChildren<Camera>();
            Vector3 snakePosition = Vector3.zero;
            switch (i)
            {
                case 0:
                    snakeMovement._moveKeys = new KeyCode[]
                    {
                        KeyCode.W,
                        KeyCode.A,
                        KeyCode.S,
                        KeyCode.D
                    };

                    snakeHand.MainPowerupKey = KeyCode.E;
                    snakeHand.MainPowerupAlt = KeyCode.E;
                    snakeHand.WeakPowerupKey = KeyCode.Q;

                    if (SnakeCount == 1) camera.rect = new Rect(0f, 0f, 1f, 1f);
                    if (SnakeCount == 2) camera.rect = new Rect(0f, 0f, 0.5f, 1f);
                    if (SnakeCount > 2) camera.rect = new Rect(0f, 0f, 0.5f, 0.5f);
                    break;

                case 1:
                    snakeMovement._moveKeys = new KeyCode[]
                    {
                        KeyCode.UpArrow,
                        KeyCode.LeftArrow,
                        KeyCode.DownArrow,
                        KeyCode.RightArrow
                    };

                    snakeHand.MainPowerupKey = KeyCode.RightControl;
                    snakeHand.MainPowerupAlt = KeyCode.RightAlt;
                    snakeHand.WeakPowerupKey = KeyCode.RightShift;

                    if (SnakeCount == 2) camera.rect = new Rect(0.5f, 0f, 0.5f, 1f);
                    if (SnakeCount > 2) camera.rect = new Rect(0.5f, 0f, 0.5f, 0.5f);
                    break;

                case 2:
                    snakeMovement._moveKeys = new KeyCode[]
                    {
                        KeyCode.I,
                        KeyCode.J,
                        KeyCode.K,
                        KeyCode.L
                    };

                    snakeHand.MainPowerupKey = KeyCode.O;
                    snakeHand.MainPowerupAlt = KeyCode.O;
                    snakeHand.WeakPowerupKey = KeyCode.U;

                    camera.rect = new Rect(0f, 0.5f, 0.5f, 0.5f);
                    break;

                case 3:
                    snakeMovement._moveKeys = new KeyCode[]
                    {
                        KeyCode.T,
                        KeyCode.F,
                        KeyCode.G,
                        KeyCode.H
                    };

                    snakeHand.MainPowerupKey = KeyCode.Y;
                    snakeHand.MainPowerupAlt = KeyCode.Z;
                    snakeHand.WeakPowerupKey = KeyCode.R;

                    camera.rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
                    break;
            }

            Snakes[i] = Instantiate(snake, snakePosition, Quaternion.identity);
        }
    }

    public void SliderChanged(int playerCount)
    {
        SnakeCount = playerCount;
    }

    public void RerollAllHands()
    {
        for (int i = 0; i < Snakes.Length; i++)
        {
            Snakes[i].GetComponent<SnakeHand>().EndPowerup(0);
            Snakes[i].GetComponent<SnakeHand>().EndPowerup(1);
        }
    }

}
