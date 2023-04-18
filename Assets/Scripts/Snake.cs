using UnityEngine;
using TMPro;

public class Snake : MonoBehaviour
{

    private SnakeMovement _snakeMovement;
    private SnakeBody _snakeBody;
    private SnakeHand _snakeHand;

    [HideInInspector] public int PlayerID;
    [HideInInspector] public int PlayerCount;

    public uint Coins;
    public float PickupRadius;

    private TextMeshPro _coinText;
    private LayerMask _collectibleLayerMask;
    private int _playerLayer;
    private float GrowthProgress;

    private void Start()
    {
        _snakeMovement = GetComponent<SnakeMovement>();
        _snakeBody = GetComponent<SnakeBody>();
        _snakeHand = GetComponent<SnakeHand>();

        _coinText = GetComponentInChildren<TextMeshPro>();

        _collectibleLayerMask = SnakeManager.CollectibleLayerMask;
        PickupRadius = SnakeManager.PickupRadius;
        _playerLayer = SnakeManager.PlayerLayer;
    }

    private void Respawn()
    {
        _snakeBody.SnakePositions.Clear();

        _snakeHand.EndPowerup(_snakeHand._mainSlot);
        _snakeHand.EndPowerup(_snakeHand._weakSlot);

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

        _snakeBody.SnakeLength = SnakeManager.DefaultSnakeLength;

        _snakeMovement.MoveSpeed = SnakeManager.MoveSpeed;
        _snakeMovement.Movement = _snakeMovement.SpawnVelocity;
    }

    private void PickupCollectibles()
    {
        Collider2D[] collidersWithinRadius = new Collider2D[5];
        Physics2D.OverlapCircleNonAlloc(transform.position, PickupRadius, collidersWithinRadius, _collectibleLayerMask);

        foreach (Collider2D collider in collidersWithinRadius)
        {
            if (collider != null)
            {
                Debug.Log("Collectible within pickup radius");
                switch (collider.gameObject.tag)
                {
                    case "Point":
                        GrowthProgress += 1f / _snakeBody.PointsToGrow;
                        if (GrowthProgress >= 1f) { GrowthProgress = 0f; _snakeBody.SnakeLength++; }

                        Destroy(collider.gameObject);
                        Debug.Log("Point Collected");

                        break;

                    case "Coin":
                        if (Coins < 20) UpdateCoins(1);

                        Destroy(collider.gameObject);
                        Debug.Log("Coin Collected");

                        break;
                }
            }
            
        }
    }

    public void UpdateCoins(int newValue, bool set = false)
    {
        if (set) Coins = (uint)newValue;
        else
        {
            int newCoins = (int)Coins + newValue;
            if (newCoins < 0) Coins = 0;
            else Coins = (uint)newCoins;
        }
        _coinText.text = Coins.ToString();
    }

    private void Update() => PickupCollectibles();

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collision entered");

        // collision.collider is the INCOMING collider
        // collision.otherCollider is OUR collider
        if (collision.gameObject.layer == _playerLayer && collision.collider.GetType() == typeof(EdgeCollider2D))
        {
            Debug.Log("Colliding with player edge");
            Respawn();
        }
    }
}
