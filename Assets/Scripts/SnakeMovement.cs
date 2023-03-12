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
    public string[] Powerups;
    public string[] WeakPowerups;

    [HideInInspector] public Vector2 InitialVelocityOverride;
    [HideInInspector] public int PlayerID;
    [HideInInspector] public int PlayerCount;

    private string[] _playerPowerups;
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
    private float _timeSincePowerup;

    private uint _primarySlot = 0;
    private uint _secondarySlot = 1;

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

        Powerups = new string[3]
        {
            "None",          /* Basically null */
            "Speed",         /* Player go brrrr */
            "CloneGrenade"   /* NPC clones get shot into random directions */
            //"FreeMove",      /* Player no longer abides by snake movement laws */
            //"Freeze",        /* Nearby enemy players freeze */
            //"LongTail",      /* Body doesn't get destroyed */
            //"Confusion",     /* Enemy player's controls are inverted */
            //"Blackout",      /* All enemy screens are deactivated  */
            //"SelfKill",      /* Nearby players will die when running into themselves (body reset for affected players upon activation) */
        };
        
        WeakPowerups = new string[2]
        {
            "None",          /* Basically null */
            "Magnet",        /* Increases pickup range */
            //"FarView",       /* Player view gets expanded */
            //"Teleport",      /* Teleport to nearby player (auto body reset) */
            //"TailReset",     /* Resets tail and cures debuffs */
            //"Hax"            /* Draws a line towards all coins and players */
        };

        _playerPowerups = new string[_powerupGameObjects.Length];
        SetPowerup(_primarySlot);
        SetPowerup(_secondarySlot);

        _powerupGameObjects[_primarySlot].gameObject.SetActive(false);
        _powerupGameObjects[_secondarySlot].gameObject.SetActive(false);

        _snakePositions = new List<Vector3>();
        _moveCooldownTimeSpan = TimeSpan.FromSeconds(_moveCooldown);

        InvokeRepeating("CreateBody", 0f, 0.2f);
    }

    private void UsePowerup(string powerup, uint slot)
    {
        _timeSincePowerup = 0f;
        UpdateCoins((uint) -(10 - (slot * 5)));
        SetPowerup(slot);

        switch (powerup)
        {
            case "Magnet":
                break;
            case "Speed":
                break;
            case "CloneGrenade":
                break;
            case "FreeMove":
                break;
            case "Freeze":
                break;
            case "LongTail":
                break;
            case "Confusion":
                break;
            case "FarView":
                break;
            case "Teleport":
                break;
            case "Blackout":
                break;
            case "SelfKill":
                break;
            case "TailReset":
                break;
            case "Hax":
                break;
        }
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
        if (Input.GetKeyDown(_keybinds[4]) && _movement.x == 0)
        {
            UsePowerup(_playerPowerups[1], _secondarySlot);
        }
        if ((Input.GetKeyDown(_keybinds[5]) || Input.GetKeyDown(_keybinds[6])) && _movement.x == 0)
        {
            UsePowerup(_playerPowerups[0], _primarySlot);
        }

        if (DateTime.UtcNow - _lastMove > _moveCooldownTimeSpan && _nextMove != Vector2.zero) ChangePlayerDirection(_nextMove);

        PickupCollectibles();
        SetEdgeCollider();

        _primaryPowerupPosition = _rigidbody.position - (_movement * 3/4);
        _secondaryPowerupPosition = _rigidbody.position - (_movement * 3/4) * 2;
        PlacePowerup(_powerupGameObjects, _playerPowerups, _primaryPowerupPosition, _secondaryPowerupPosition);

        _timeSincePowerup += Time.deltaTime;
    }
    private void PlacePowerup(GameObject[] powerupGameObjects, string[] powerups, Vector3 primaryPowerupPosition, Vector3 secondaryPowerupPosition)
    {
        powerupGameObjects[0].transform.position = primaryPowerupPosition;
        powerupGameObjects[1].transform.position = secondaryPowerupPosition;

        for (int i = 0; i < powerupGameObjects.Length; i++)
        {
            if (powerupGameObjects[i].activeSelf == true)
            {
                int indexOfPowerup;
                if (i == 0) indexOfPowerup = Array.IndexOf(Powerups.Skip(1).ToArray(), powerups[i]); // gets the index of the powerup (essentially an ID)
                else        indexOfPowerup = Array.IndexOf(WeakPowerups.Skip(1).ToArray(), powerups[i]); // gets the index of the powerup (essentially an ID)
                powerupGameObjects[i].GetComponent<SpriteRenderer>().sprite = _powerupSprites[indexOfPowerup]; // gets the sprite corresponding to that ID
            }
        }
    }

    private void SetPowerup(uint slot)
    {
        _playerPowerups[slot] = GetRandomPowerup(slot == 1); // if slot = 1 (weak slot) then get weak powerup)
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
                        if (_coins < 20) UpdateCoins(_coins + 1);

                        if (_coins >= 5)  _powerupGameObjects[_secondarySlot].gameObject.SetActive(true);
                        else              _powerupGameObjects[_secondarySlot].gameObject.SetActive(false);
                        if (_coins >= 10) _powerupGameObjects[_primarySlot].gameObject.SetActive(true);
                        else              _powerupGameObjects[_primarySlot].gameObject.SetActive(false);

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

    private string GetRandomPowerup(bool weak = false) // https://dirask.com/posts/C-NET-get-random-element-from-enum-X13Qrp
    {
        if (weak) return WeakPowerups[UnityEngine.Random.Range(1, WeakPowerups.Length)];
        return Powerups[UnityEngine.Random.Range(1, Powerups.Length)]; // Gets a random item in the powerups list, skipping "None"
    }
}