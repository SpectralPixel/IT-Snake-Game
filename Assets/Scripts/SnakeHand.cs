using System;
using UnityEngine;
using System.Linq;

public class SnakeHand : MonoBehaviour
{

    private Snake _snake;
    private SnakeMovement _snakeMovement;
    private SnakeBody _snakeBody;

    [SerializeField] private GameObject[] _powerupGameObjects;

    [HideInInspector] public KeyCode MainPowerupKey;
    [HideInInspector] public KeyCode MainPowerupAlt;
    [HideInInspector] public KeyCode WeakPowerupKey;

    [HideInInspector] public string[] Powerups;
    [HideInInspector] public string[] WeakPowerups;

    private Sprite[] _powerupSprites;
    private Sprite[] _weakPowerupSprites;

    private string[] _playerHand;

    private Vector3 _primaryPowerupPosition;
    private Vector3 _secondaryPowerupPosition;

    private string _currentPowerup;
    private string _currentWeakPowerup;

    private float _powerupRemainingTime;
    private float _weakPowerupRemainingTime;

    [HideInInspector] public uint _mainSlot = 0;
    [HideInInspector] public uint _weakSlot = 1;

    // variables for powerups
    private Camera[] _allCams;

    void Start()
    {
        _snake = GetComponent<Snake>();
        _snakeMovement = GetComponent<SnakeMovement>();
        _snakeBody = GetComponent<SnakeBody>();

        _powerupSprites = SnakeManager.PowerupSprites;
        _weakPowerupSprites = SnakeManager.WeakPowerupSprites;

        Powerups = new string[7]
        {
            "None",          /* Basically null */
            "Speed",         /* Player go brrrr */
            "CloneGrenade",   /* NPC clones get shot into random directions */
            "Confusion",     /* Enemy player's controls are inverted */
            "Blackout",      /* All enemy screens are deactivated  */
            "SelfKill",      /* Nearby players will die when running into themselves (body reset for affected players upon activation) */
            "Freeze"        /* Nearby enemy players freeze */
            //"FreeMove",      /* Player no longer abides by snake movement laws */
            //"LongTail",      /* Body doesn't get destroyed */
        };
        
        WeakPowerups = new string[4]
        {
            "None",          /* Basically null */
            "Magnet",        /* Increases pickup range */
            "TailReset",     /* Resets tail and cures debuffs */
            "Hax"            /* Draws a line towards all coins and players */
            //"FarView",       /* Player view gets expanded */
            //"Teleport"      /* Teleport to nearby player (auto body reset) */
        };

        _playerHand = new string[_powerupGameObjects.Length];
        SetPowerup(_mainSlot);
        SetPowerup(_weakSlot);

        _powerupGameObjects[_mainSlot].gameObject.SetActive(false);
        _powerupGameObjects[_weakSlot].gameObject.SetActive(false);

        // setting powerup variables
        _allCams = Camera.allCameras;
    }

    private void PlacePowerup(GameObject[] powerupGameObjects, string[] powerups, Vector3 primaryPowerupPosition, Vector3 secondaryPowerupPosition)
    {
        powerupGameObjects[0].transform.position = primaryPowerupPosition;
        powerupGameObjects[1].transform.position = secondaryPowerupPosition;

        if (_snake.Coins >= 5)  _powerupGameObjects[_weakSlot].gameObject.SetActive(true);
        else              _powerupGameObjects[_weakSlot].gameObject.SetActive(false);
        if (_snake.Coins >= 10) _powerupGameObjects[_mainSlot].gameObject.SetActive(true);
        else              _powerupGameObjects[_mainSlot].gameObject.SetActive(false);

        for (int i = 0; i < powerupGameObjects.Length; i++)
        {
            if (powerupGameObjects[i].activeSelf == true)
            {
                int indexOfPowerup;
                if (i == 0)
                {
                    indexOfPowerup = Array.IndexOf(Powerups.Skip(1).ToArray(), powerups[i]); // gets the index of the powerup (essentially an ID)
                    powerupGameObjects[i].GetComponent<SpriteRenderer>().sprite = _powerupSprites[indexOfPowerup]; // gets the sprite corresponding to that ID
                }

                else {
                    indexOfPowerup = Array.IndexOf(WeakPowerups.Skip(1).ToArray(), powerups[i]); // gets the index of the powerup (essentially an ID)
                    powerupGameObjects[i].GetComponent<SpriteRenderer>().sprite = _weakPowerupSprites[indexOfPowerup]; // gets the sprite corresponding to that ID
                }
            }
        }
    }

    private void SetPowerup(uint slot)
    {
        _playerHand[slot] = GetRandomPowerup(slot == 1); // if slot = 1 (weak slot) then get weak powerup)
    }

    private void UsePowerup(string powerup, uint slot)
    {
        if (slot == 0) _currentPowerup = powerup;
        if (slot == 1) _currentWeakPowerup = powerup;

        int coinSubtraction = (10 - ((int)slot * 5));
        _snake.UpdateCoins(-coinSubtraction);

        if (slot == 0)
        {
            switch (_currentPowerup)
            {
                case "Speed": // just multiply speed
                    _snakeMovement.MoveSpeed *= 2;
                    _powerupRemainingTime = 6f;
                    break;
                case "CloneGrenade": // creates new clones, no idea how to implement yet might need to rework snake logic first
                    _powerupRemainingTime = 0f;
                    break;
                case "FreeMove": // if statement at input
                    _powerupRemainingTime = 8f;
                    break;
                case "Freeze": // just freeze nearby players and tail removal
                    _powerupRemainingTime = 5f;
                    break;
                case "LongTail": // make it so end of the snake doesnt get removed; no code needed here
                    _powerupRemainingTime = 8f;
                    break;
                case "Confusion": // get nearby players and invert controls
                    _powerupRemainingTime = 6f;
                    break;
                case "Blackout": // deactivate all other cameras
                    Camera activatorCam = gameObject.GetComponentInChildren<Camera>();
                    for (int i = 0; i < _allCams.Length; i++)
                    {
                        if (_allCams[i] != activatorCam)
                        {
                            _allCams[i].gameObject.SetActive(false);
                        }
                    }
                    _powerupRemainingTime = 7f;
                    break;
                case "SelfKill": // rework collision system to allow self collisions (possibly split head and body objects)
                    _powerupRemainingTime = 6f;
                    break;
                default:
                    Debug.LogError("PowerupNotAssignedError");
                    _powerupRemainingTime = 0f;
                    break;
            }
        }
        if (slot == 1)
        {
            switch (_currentWeakPowerup)
            {
                case "Magnet": // Increase pickup range
                    _weakPowerupRemainingTime = 8f;
                    break;
                case "TailReset": // Remove all tailpositions
                    _snakeBody.SnakePositions.Clear();
                    _weakPowerupRemainingTime = 0f;
                    break;
                case "Hax": // draw lines to all coins and players
                    _weakPowerupRemainingTime = 8f;
                    break;
                case "FarView": // move cam back
                    _weakPowerupRemainingTime = 10f;
                    break;
                case "Teleport": // get distances to all players (pythag) and teleport to furthest one
                    _weakPowerupRemainingTime = 0f;
                    break;
                default:
                    Debug.LogError("PowerupNotAssignedError");
                    _powerupRemainingTime = 0f;
                    break;
            }
        }
    }

    public void EndPowerup(uint slot)
    {
        ///////////////////////////////////////////////////// only do this if the powerup was speed!!!
        _snakeMovement.MoveSpeed = SnakeManager.MoveSpeed;

        for (int i = 0; i < _allCams.Length; i++)
        {
            _allCams[i].gameObject.SetActive(true);
        }

        SetPowerup(slot);
    }

    private string GetRandomPowerup(bool weak = false) // https://dirask.com/posts/C-NET-get-random-element-from-enum-X13Qrp
    {
        if (weak) return WeakPowerups[UnityEngine.Random.Range(1, WeakPowerups.Length)];
        return Powerups[UnityEngine.Random.Range(1, Powerups.Length)]; // Gets a random item in the powerups list, skipping "None"
    }

    void Update()
    {
        if (Input.GetKeyDown(WeakPowerupKey) && _snakeMovement.Movement.x == 0 && _powerupRemainingTime < 0f && _snake.Coins >= 5)
        {
            UsePowerup(_playerHand[1], _weakSlot);
        }
        if ((Input.GetKeyDown(MainPowerupKey) || Input.GetKeyDown(MainPowerupAlt)) && _snakeMovement.Movement.x == 0 && _powerupRemainingTime < 0f && _snake.Coins >= 10)
        {
            UsePowerup(_playerHand[0], _mainSlot);
        }
    }

    private void LateUpdate()
    {
        Vector3 movement = _snakeMovement.Movement;

        _primaryPowerupPosition = transform.position - (movement * 3/4);
        _secondaryPowerupPosition = transform.position - (movement * 3/4) * 2;
        PlacePowerup(_powerupGameObjects, _playerHand, _primaryPowerupPosition, _secondaryPowerupPosition);
    }

    private void FixedUpdate()
    {
        if (_powerupRemainingTime <= 0f) _currentPowerup = "";
        if (_weakPowerupRemainingTime <= 0f) _currentWeakPowerup = "";
        if (_currentPowerup != "") EndPowerup(_mainSlot);
        if (_currentWeakPowerup != "") EndPowerup(_weakSlot);
        _powerupRemainingTime -= Time.fixedDeltaTime;
        _weakPowerupRemainingTime -= Time.fixedDeltaTime;
    }



    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, SnakeManager.PickupRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_primaryPowerupPosition, 0.45f);
        Gizmos.DrawWireSphere(_secondaryPowerupPosition, 0.3f);
    }
}
