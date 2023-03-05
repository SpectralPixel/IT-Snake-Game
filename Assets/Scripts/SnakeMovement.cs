using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(EdgeCollider2D))]
public class SnakeMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private EdgeCollider2D _edgeCollider;
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private TextMeshPro _coinsText;
    [SerializeField] private LayerMask _collectibleLayerMask;
    [SerializeField] private GameObject[] _powerupGameObjects;
    [SerializeField] private Sprite[] _powerupSprites;
    [SerializeField] private int _playerLayer;
    [SerializeField] private float _pickupRadius;
    public KeyCode[] _keybinds;

    [HideInInspector] public Vector2 InitialVelocityOverride;
    [HideInInspector] public int PlayerID;
    [HideInInspector] public int PlayerCount;
    private Powerup[] _playerPowerups;
    private Powerup[] _allPowerups;
    private Vector3 _primaryPowerupPosition;
    private Vector3 _secondaryPowerupPosition;
    private Vector2 _initialVelocity;
    private Vector2 _movement;
    private Vector2 _nextMove;
    private uint _snakeLength;
    private float _snakeLengthIncrement;
    private float _moveSpeed;
    private uint _pointsToGrow;
    private uint _coins;

    private List<Vector3> _snakePositions;

    [SerializeField] private float _moveCooldown;
    private TimeSpan _moveCooldownTimeSpan;
    private DateTime _lastMove;


    private void Start()
    {
        GetManagerVariables();

        if (InitialVelocityOverride != Vector2.zero) _initialVelocity = InitialVelocityOverride;
        _movement = _initialVelocity;

        SetPlayerColor();

        _playerPowerups = new Powerup[_powerupGameObjects.Length];
        for (int i = 0; i < _playerPowerups.Length; i++)
        {
            _playerPowerups[i] = Powerup.None;
        }

        _allPowerups = (Powerup[])typeof(Powerup).GetEnumValues(); // Makes an array of all Powerups
        _allPowerups = _allPowerups.Skip(1).ToArray(); // Makes an array of all Powerups
        string test = "";
        for (int i = 0; i < _allPowerups.Length; i++)
        {
            test = test + ", " + _allPowerups[i].ToString();
        }
        Debug.Log(test);

        _snakePositions = new List<Vector3>();
        _moveCooldownTimeSpan = TimeSpan.FromSeconds(_moveCooldown);

        InvokeRepeating("CreateBody", 0f, 0.2f);
    }

    private void GetManagerVariables()
    {
        _snakeLength = SnakeManager.DefaultSnakeLength;
        _moveSpeed = SnakeManager.MoveSpeed;
        _pointsToGrow = SnakeManager.PointsToGrow;
    }

    private void SetPlayerColor()
    {
        Color snakeStartColor = Color.HSVToRGB(PlayerID * (1f / PlayerCount), 1f, 1.0f, false);
        Color snakeEndColor = Color.HSVToRGB(PlayerID * (1f / PlayerCount), 1f, 0.5f, false);
        _spriteRenderer.color = snakeStartColor;
        _lineRenderer.colorGradient = CreateSimpleGradient(snakeStartColor, snakeEndColor, true);
    }

    private Gradient CreateSimpleGradient(Color startColor, Color endColor, bool inverse = false)
    {
        Gradient snakeBodyColor = new Gradient();
        snakeBodyColor.SetKeys(
            new GradientColorKey[]
            {
                new GradientColorKey(
                    inverse ? endColor : startColor,
                    0f
                ),
                new GradientColorKey(
                    inverse ? startColor : endColor,
                    1f
                )
            },
            new GradientAlphaKey[]
            {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(1f, 1f)
            }
        );
        return snakeBodyColor;
    }

    private void CreateBody()
    {
        _snakePositions.Add(new Vector3(_rigidbody.position.x, _rigidbody.position.y, 0f));

        while (_snakePositions.Count > _snakeLength)
        {
            _snakePositions.RemoveAt(0);
        }
    }

    private void SetEdgeCollider()
    {
        List<Vector2> edges = new List<Vector2>();

        for (int point = 0; point < _lineRenderer.positionCount; point++)
        {
            Vector3 lineRendererPoint = transform.InverseTransformPoint(_lineRenderer.GetPosition(point));
            edges.Add(new Vector2(lineRendererPoint.x, lineRendererPoint.y));
        }

        _edgeCollider.SetPoints(edges);
    }

    private void ChangePlayerDirection(Vector2 movement)
    {
        if (DateTime.UtcNow - _lastMove > _moveCooldownTimeSpan)
        {
            _movement = movement;
            _nextMove = Vector2.zero;

            CreateBody();

            _lastMove = DateTime.UtcNow;
        }
        else
        {
            _nextMove = movement;
        }
    }

    private void Respawn()
    {
        Vector3 snakeRespawnPosition = Vector3.zero;
        switch (PlayerID)
        {
            case 0:
                snakeRespawnPosition = new Vector3(0f, 1f, 0f);
                _initialVelocity = new Vector2(0f, 1f);
                break;

            case 1:
                snakeRespawnPosition = new Vector3(1f, 0f, 0f);
                _initialVelocity = new Vector2(1f, 0f);
                break;

            case 2:
                snakeRespawnPosition = new Vector3(0f, -1f, 0f);
                _initialVelocity = new Vector2(0f, -1f);
                break;

            case 3:
                snakeRespawnPosition = new Vector3(-1f, 0f, 0f);
                _initialVelocity = new Vector2(-1f, 0f);
                break;
        }

        _snakePositions.Clear();
        _snakeLength = SnakeManager.DefaultSnakeLength;
        _moveSpeed = SnakeManager.MoveSpeed;
        _rigidbody.position = snakeRespawnPosition;
        _movement = _initialVelocity;
    }

    void Update()
    {
        if (Input.GetKeyDown(_keybinds[0]) && _movement.y == 0) { ChangePlayerDirection(Vector2.up); }
        if (Input.GetKeyDown(_keybinds[1]) && _movement.x == 0) { ChangePlayerDirection(Vector2.left); }
        if (Input.GetKeyDown(_keybinds[2]) && _movement.y == 0) { ChangePlayerDirection(Vector2.down); }
        if (Input.GetKeyDown(_keybinds[3]) && _movement.x == 0) { ChangePlayerDirection(Vector2.right); }

        if (DateTime.UtcNow - _lastMove > _moveCooldownTimeSpan && _nextMove != Vector2.zero) ChangePlayerDirection(_nextMove);

        PickupCollectibles();
        SetEdgeCollider();

        _primaryPowerupPosition = _rigidbody.position - (_movement * 3/4);
        _secondaryPowerupPosition = _rigidbody.position - (_movement * 3/4) * 2;
        PlacePowerup(_powerupGameObjects, _playerPowerups, _primaryPowerupPosition, _secondaryPowerupPosition);
    }
    private void PlacePowerup(GameObject[] powerupGameObjects, Powerup[] powerups, Vector3 primaryPowerupPosition, Vector3 secondaryPowerupPosition)
    {
        powerupGameObjects[0].transform.position = primaryPowerupPosition;
        powerupGameObjects[1].transform.position = secondaryPowerupPosition;

        for (int i = 0; i < powerupGameObjects.Length; i++)
        {
            powerupGameObjects[i].SetActive(powerups[i] != Powerup.None);

            int indexOfPowerup = Array.IndexOf(_allPowerups, powerups[i]); // gets the index of the powerup (essentially an ID)
            powerupGameObjects[i].GetComponent<SpriteRenderer>().sprite = _powerupSprites[indexOfPowerup + 1]; // gets the sprite corresponding to that ID
        }
    }

    private void FixedUpdate()
    {
        _rigidbody.MovePosition(_rigidbody.position + _movement * _moveSpeed * Time.fixedDeltaTime);

        Vector3 currentPosition = new Vector3(_rigidbody.position.x, _rigidbody.position.y, 0f);
        _snakePositions.Add(currentPosition);
        _lineRenderer.positionCount = _snakePositions.Count;
        _lineRenderer.SetPositions(_snakePositions.ToArray());
        _snakePositions.Remove(currentPosition);
    }

    private void PickupCollectibles()
    {
        Collider2D[] collidersWithinRadius = new Collider2D[5];
        Physics2D.OverlapCircleNonAlloc(_rigidbody.position, _pickupRadius, collidersWithinRadius, _collectibleLayerMask);

        foreach (Collider2D collider in collidersWithinRadius)
        {
            if (collider != null)
            {
                Debug.Log("Collectible within pickup radius");
                switch (collider.gameObject.tag)
                {
                    case "Point":
                        _snakeLengthIncrement += 1f / _pointsToGrow;
                        if (_snakeLengthIncrement >= 1f) { _snakeLengthIncrement = 0f; _snakeLength++; }

                        Destroy(collider.gameObject);
                        Debug.Log("Point Collected");

                        break;

                    case "Coin":
                        UpdateCoins(_coins + 1);
                        if (_coins >= 10 && _playerPowerups[1] == Powerup.None)
                        {
                            if (_playerPowerups[0] == Powerup.None) _playerPowerups[0] = GetRandomPowerup();
                            else                                    _playerPowerups[1] = GetRandomPowerup();

                            _coins = 0;
                        }

                        Destroy(collider.gameObject);
                        Debug.Log("Coin Collected");

                        break;
                }
            }
            
        }
    }

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

    private void UpdateCoins(uint newValue)
    {
        _coins = newValue;
        _coinsText.text = _coins.ToString();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_rigidbody.position, _pickupRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_primaryPowerupPosition, 0.45f);
        Gizmos.DrawWireSphere(_secondaryPowerupPosition, 0.3f);
    }

    private Powerup GetRandomPowerup() // https://dirask.com/posts/C-NET-get-random-element-from-enum-X13Qrp
    {
        Powerup powerup = Powerup.None;

        System.Random random = new System.Random();
        while (powerup == Powerup.None)
        {
            int index = random.Next(_allPowerups.Length);
            powerup = (Powerup)_allPowerups.GetValue(index);
        }
        
        return powerup;
    }
}

public enum Powerup
{
    None,          /* Basically null */
    Magnet,        /* Increases pickup range */
    Speed,         /* Player go brrrr */
    CloneGrenade   /* NPC clones get shot into random directions */
    //FreeMove,      /* Player no longer abides by snake movement laws */
    //Freeze,        /* Nearby enemy players freeze */
    //LongTail,      /* Body doesn't get destroyed */
    //Confusion,     /* Enemy player's controls are inverted */
    //FarView,       /* Player view gets expanded */
    //Teleport,      /* Teleport to nearby player (auto body reset) */
    //Blackout,      /* All enemy screens are deactivated  */
    //SelfKill,      /* Nearby players will die when running into themselves (body reset for affected players upon activation) */
    //Hax            /* Draws a line towards all coins and players */
}