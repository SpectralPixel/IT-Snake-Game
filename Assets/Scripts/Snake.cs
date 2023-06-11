using UnityEngine;
using TMPro;
using Cinemachine;
using UnityEngine.UIElements;
using System.Collections;

public class Snake : MonoBehaviour
{

    private SnakeMovement _snakeMovement;
    private SnakeBody _snakeBody;
    private SnakeHand _snakeHand;

    [HideInInspector] public int PlayerID;
    [HideInInspector] public int PlayerCount;

    [HideInInspector] public uint Coins;
    [HideInInspector] public float PickupRadius;

    private LayerMask _collectibleLayerMask;
    private int _playerLayer;
    private float GrowthProgress;

    // WIN CONDITION 0: POINTS COLLECTED
    // WIN CONDITION 1: COINS COLLECTED
    // WIN CONDITION 2: VANQUISHES
    // WIN CONDITION 3: PWRUPS USED
    // WIN CONDITION 4: LONGEST SNAKE
    [HideInInspector] public int[] WinConditions;

    private IEnumerator Start()
    {
        _snakeMovement = GetComponent<SnakeMovement>();
        _snakeBody = GetComponent<SnakeBody>();
        _snakeHand = GetComponent<SnakeHand>();

        _collectibleLayerMask = SnakeManager.CollectibleLayerMask;
        _playerLayer = SnakeManager.PlayerLayer;

        WinConditions = new int[5];

        gameObject.GetComponent<SpriteRenderer>().sprite = SnakeManager.ActiveSnakeSkins[PlayerID];

        yield return new WaitForSeconds(0.1f);

        Respawn();
        SnakeManager.GameLoaded = true;
    }

    public void Respawn()
    {
        try
        {
            GameObject.Find("LOADING").SetActive(false);
        }
        catch { }
        

        Camera cam = GetComponentInChildren<Camera>();
        cam.cullingMask &= ~(1 << cam.gameObject.layer); // TURN OFF THIS LAYER     http://answers.unity.com/answers/353137/view.html
        cam.gameObject.layer += PlayerID;
        cam.cullingMask |= 1 << (cam.gameObject.layer); // TURN ON THIS LAYER
        
        _snakeBody.SnakePositions.Clear();

        Vector3 snakeRespawnPosition = Vector3.zero;
        switch (PlayerID)
        {
            case 0:
                snakeRespawnPosition = new Vector3(0f, 1f, 0f);
                _snakeMovement.SpawnVelocity = new Vector2(0f, 1f);
                break;

            case 1:
                snakeRespawnPosition = new Vector3(1f, 0f, 0f);
                _snakeMovement.SpawnVelocity = new Vector2(1f, 0f);
                break;

            case 2:
                snakeRespawnPosition = new Vector3(0f, -1f, 0f);
                _snakeMovement.SpawnVelocity = new Vector2(0f, -1f);
                break;

            case 3:
                snakeRespawnPosition = new Vector3(-1f, 0f, 0f);
                _snakeMovement.SpawnVelocity = new Vector2(-1f, 0f);
                break;
        }

        transform.position = snakeRespawnPosition;

        PickupRadius = SnakeManager.PickupRadius;

        GetComponentInChildren<Canvas>().enabled = false;

        CinemachineVirtualCamera vCam = GetComponentInChildren<CinemachineVirtualCamera>();
        vCam.m_Lens.OrthographicSize = SnakeManager.CameraSize;
        vCam.gameObject.layer = cam.gameObject.layer;

        CinemachineConfiner2D cmConfiner = GetComponentInChildren<CinemachineConfiner2D>();
        cmConfiner.m_BoundingShape2D = GameObject.Find("Camera Bounds").GetComponent<PolygonCollider2D>();

        _snakeBody.SnakeLength = SnakeManager.DefaultSnakeLength;

        _snakeMovement.MoveSpeed = SnakeManager.MoveSpeed;
        _snakeMovement.Movement = _snakeMovement.SpawnVelocity;

        _snakeHand.EndPowerup(_snakeHand._mainSlot);
        _snakeHand.EndPowerup(_snakeHand._weakSlot);
    }

    private void PickupCollectibles()
    {
        Collider2D[] collidersWithinRadius = new Collider2D[5];
        Physics2D.OverlapCircleNonAlloc(transform.position, PickupRadius, collidersWithinRadius, _collectibleLayerMask);

        foreach (Collider2D collider in collidersWithinRadius)
        {
            if (collider != null)
            {
                switch (collider.gameObject.tag)
                {
                    case "Point":
                        WinConditions[0]++;

                        GrowthProgress += 1f / _snakeBody.PointsToGrow;
                        if (GrowthProgress >= 1f)
                        {
                            GrowthProgress = 0f;
                            _snakeBody.SnakeLength++;
                            WinConditions[4] = (int)_snakeBody.SnakeLength;
                        }

                        Destroy(collider.gameObject);
                        GameManager.Instance.PointCount--;

                        break;

                    case "Coin":
                        WinConditions[1]++;

                        UpdateCoins(1);

                        Destroy(collider.gameObject);
                        GameManager.Instance.CoinCount--;

                        break;
                }
            }
            
        }
    }

    public void UpdateCoins(int newValue)
    {
        int newCoins = (int)Coins + newValue;
        if (newCoins < 0) Coins = 0;
        else Coins = (uint)newCoins;
    }

    private void Update()
    {
        PickupCollectibles();

        // CHEATS
        if (Input.GetKeyDown(KeyCode.Keypad9)) UpdateCoins(100);
        if (Input.GetKeyDown(KeyCode.Keypad8)) { _snakeHand.EndPowerup(0); _snakeHand.EndPowerup(1); }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (SnakeManager.GameLoaded)
        {
            // collision.collider is the INCOMING collider
            // collision.otherCollider is OUR collider
            if (collision.gameObject.layer == _playerLayer && collision.collider.GetType() == typeof(EdgeCollider2D) && collision.gameObject.tag != PlayerID.ToString())
            {
                // increase other player's kill count
                collision.gameObject.GetComponent<Snake>().WinConditions[2]++;

                Respawn();
            }
        }
    }
}
