using UnityEngine;

public class SnakeManager : MonoBehaviour
{
    public static SnakeManager Instance;

    public static int SnakeCount;
    public static uint DefaultSnakeLength;
    public static float MoveSpeed;
    public static uint PointsToGrow;
    public static float MoveCooldown;
    public static float PickupRadius;
    public static LayerMask CollectibleLayerMask;
    public static int PlayerLayer;
    public static Sprite[] PowerupSprites;
    public static Sprite[] WeakPowerupSprites;

    [SerializeField] private int _playerCount;
    [SerializeField] private uint _defaultSnakeLength;
    [SerializeField] private float _defaultMoveSpeed;
    [SerializeField] uint _pointsToGrow;
    [SerializeField] private float _moveCooldown;
    [SerializeField] private float _defaultPickupRadius;
    [SerializeField] private LayerMask _collectibleLayerMask;
    [SerializeField] private int _playerLayer;
    [SerializeField] private Sprite[] _powerupSprites;
    [SerializeField] private Sprite[] _weakPowerupSprites;

    [SerializeField] private GameObject _snakePrefab;

    private void Awake()
    {
        SnakeCount = _playerCount;
        DefaultSnakeLength = _defaultSnakeLength;
        PointsToGrow = _pointsToGrow;
        PickupRadius = _defaultPickupRadius;

        MoveSpeed = _defaultMoveSpeed;
        MoveCooldown = _moveCooldown;

        CollectibleLayerMask = _collectibleLayerMask;
        PlayerLayer = _playerLayer;

        PowerupSprites = _powerupSprites;
        WeakPowerupSprites = _weakPowerupSprites;
    }

    private void Start()
    {
        for (int i = 0; i < SnakeCount; i++)
        {
            GameObject snake = _snakePrefab;

            Snake snakeScript = snake.GetComponent<Snake>();
            SnakeMovement snakeMovement = snake.GetComponent<SnakeMovement>();
            SnakeHand snakeHand = snake.GetComponent<SnakeHand>();

            snakeScript.PlayerID = i;
            snakeScript.PlayerCount = _playerCount;

            Camera camera = snake.GetComponentInChildren<Camera>();
            Vector3 snakePosition = Vector3.zero;
            switch (i)
            {
                case 0:
                    snakePosition = new Vector3(0f, 1f, 0f);
                    snakeMovement.SpawnVelocity = new Vector2(0f, 1f);
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
                    if (SnakeCount == 2) camera.rect = new Rect(0f, 0f, 0.4975f, 1f);
                    if (SnakeCount > 2) camera.rect = new Rect(0f, 0f, 0.4975f, 0.495f);
                    break;

                case 1:
                    snakePosition = new Vector3(1f, 0f, 0f);
                    snakeMovement.SpawnVelocity = new Vector2(1f, 0f);
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

                    if (SnakeCount == 2) camera.rect = new Rect(0.5025f, 0f, 0.4975f, 1f);
                    if (SnakeCount > 2) camera.rect = new Rect(0.5025f, 0f, 0.4975f, 0.495f);
                    break;

                case 2:
                    snakePosition = new Vector3(0f, -1f, 0f);
                    snakeMovement.SpawnVelocity = new Vector2(0f, -1f);
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

                    camera.rect = new Rect(0f, 0.505f, 0.498f, 0.495f);
                    break;

                case 3:
                    snakePosition = new Vector3(-1f, 0f, 0f);
                    snakeMovement.SpawnVelocity = new Vector2(-1f, 0f);
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

                    camera.rect = new Rect(0.5025f, 0.505f, 0.4975f, 0.495f);
                    break;
            }

            Instantiate(snake, snakePosition, Quaternion.identity);
        }
    }

}
