using System;
using UnityEngine;
using System.Linq;
using static Powerup;
using Cinemachine;

public class SnakeHand : MonoBehaviour
{

    private Snake _snake;
    private SnakeMovement _snakeMovement;
    private SnakeBody _snakeBody;

    [SerializeField] private GameObject[] _powerupGameObjects;

    [SerializeField] private GameObject _clonePrefab;

    [HideInInspector] public KeyCode MainPowerupKey;
    [HideInInspector] public KeyCode MainPowerupAlt;
    [HideInInspector] public KeyCode WeakPowerupKey;

    [HideInInspector] public string[] Powerups;
    [HideInInspector] public string[] WeakPowerups;

    private Sprite[] _powerupSprites;
    private Sprite[] _weakPowerupSprites;

    private Powerup[] _playerHand;

    private Vector3 _primaryPowerupPosition;
    private Vector3 _secondaryPowerupPosition;

    [HideInInspector] public uint _mainSlot = 0;
    [HideInInspector] public uint _weakSlot = 1;

    private void Awake()
    {
        WeakPowerups = new string[4]
        {
            "None",          /* Basically null */
            "Magnet",        /* Increases pickup range */
            "TailReset",     /* Resets tail and cures debuffs */
            //"Hax"            /* Draws a line towards all coins and players */
            "FarView",       /* Player view gets expanded */
            //"Teleport"      /* Teleport to nearby player (auto body reset) */
        };

        Powerups = new string[7]
        {
            "None",          /* Basically null */
            "Speed",         /* Player go brrrr */
            "CloneGrenade",   /* NPC clones get shot into random directions */
            "Confusion",     /* Enemy player's controls are inverted */
            "Blackout",      /* All enemy screens are deactivated  */
            "Freeze",            /* Nearby enemy players freeze */
            "FreeMove",      /* Player no longer abides by snake movement laws */
            //"LongTail",      /* Body doesn't get destroyed */
        };
    }

    void Start()
    {
        _snake = GetComponent<Snake>();
        _snakeMovement = GetComponent<SnakeMovement>();
        _snakeBody = GetComponent<SnakeBody>();

        _powerupSprites = SnakeManager.PowerupSprites;
        _weakPowerupSprites = SnakeManager.WeakPowerupSprites;

        _playerHand = new Powerup[_powerupGameObjects.Length];
        SetPowerup(_mainSlot);
        SetPowerup(_weakSlot);

        _powerupGameObjects[_mainSlot].gameObject.SetActive(false);
        _powerupGameObjects[_weakSlot].gameObject.SetActive(false);
    }

    private void SetPowerup(uint slot)
    {
        _playerHand[slot] = GetRandomPowerup(slot == 1); // if slot = 1 (weak slot) then get weak powerup)
    }

    private void UsePowerup(Powerup powerup, uint slot)
    {
        _snake.WinConditions[3]++;

        int coinSubtraction = (10 - ((int)slot * 5));
        _snake.UpdateCoins(-coinSubtraction);

        powerup.StartPowerup();
    }

    public void EndPowerup(uint slot)
    {
        SetPowerup(slot);
    }

    private Powerup GetRandomPowerup(bool weak = false) // https://dirask.com/posts/C-NET-get-random-element-from-enum-X13Qrp
    {
        string powerupName;
        float timeLimit;

        PowerupBehaviour startBehaviour;
        PowerupBehaviour tickBehaviour;
        PowerupBehaviour endBehaviour;

        // DLELETE MEeEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
        powerupName = null;
        if (weak)
        {
            powerupName = WeakPowerups[UnityEngine.Random.Range(1, WeakPowerups.Length)];
            Debug.Log(powerupName.ToString() + ", " + WeakPowerups.Length.ToString());
        }
        else
        {
            int randnum = UnityEngine.Random.Range(1, Powerups.Length);
            string powlen = Powerups.Length.ToString();
            string randnumtxt = randnum.ToString();
            try
            {
                powerupName = Powerups[randnum];
                Debug.Log(powerupName.ToString() + ", " + Powerups.Length.ToString());
            }
            catch
            {
                Debug.Log(powlen + "-" + randnumtxt);
            }
        }

        if (!weak)
        {
            switch (powerupName)
            {
                case "Speed": // just multiply speed

                    startBehaviour = delegate ()
                    {
                        _snakeMovement.MoveSpeed *= 2;
                    };

                    tickBehaviour = delegate ()
                    {
                        _snakeMovement.MoveCooldown = 0;
                    };

                    endBehaviour = delegate ()
                    {

                    };

                    timeLimit = 6f;

                    break;

                case "CloneGrenade": // creates new clones, no idea how to implement yet might need to rework snake logic first

                    startBehaviour = delegate ()
                    {

                        int splinters = UnityEngine.Random.Range(3, 5);
                        for (int i = 0; i < splinters; i++)
                        {
                            GameObject newClone = Instantiate(_clonePrefab);

                            float angle = UnityEngine.Random.Range(1, 360) * (Mathf.PI / 180);
                            float dx = SnakeManager.MoveSpeed * Mathf.Cos(angle);
                            float dy = SnakeManager.MoveSpeed * Mathf.Sin(angle);
                            newClone.GetComponent<CloneMovement>().SpawnVelocity = new Vector2(dx, dy);
                        }
                    };

                    tickBehaviour = delegate ()
                    {

                    };

                    endBehaviour = delegate ()
                    {

                    };

                    timeLimit = 0f;

                    break;

                case "FreeMove": // if statement at input

                    startBehaviour = delegate ()
                    {
                        _snakeMovement.FreeMove = true;
                    };

                    tickBehaviour = delegate ()
                    {

                    };

                    endBehaviour = delegate () 
                    {
                        _snakeMovement.FreeMove = false;
                    };

                    timeLimit = 8f;

                    break;

                case "Freeze": // just freeze nearby players and tail removal

                    startBehaviour = delegate ()
                    {
                        for (int i = 0; i < SnakeManager.SnakeCount; i++)
                        {
                            if (SnakeManager.Snakes[i] != gameObject)
                            {
                                SnakeManager.Snakes[i].GetComponent<SnakeMovement>().Frozen = true;
                            }
                        }
                    };

                    tickBehaviour = delegate ()
                    {

                    };

                    endBehaviour = delegate ()
                    {
                        for (int i = 0; i < SnakeManager.SnakeCount; i++)
                        {
                            if (SnakeManager.Snakes[i] != gameObject)
                            {
                                SnakeManager.Snakes[i].GetComponent<SnakeMovement>().Frozen = false;
                            }
                        }
                    };

                    timeLimit = 5f;

                    break;

                case "LongTail": // make it so end of the snake doesnt get removed; no code needed here

                    startBehaviour = delegate ()
                    {
                        _snakeBody.InfiniteLength = true;
                    };

                    tickBehaviour = delegate ()
                    {

                    };

                    endBehaviour = delegate ()
                    {
                        _snakeBody.InfiniteLength = false;
                    };

                    timeLimit = 8f;

                    break;

                case "Confusion": // get nearby players and invert controls

                    startBehaviour = delegate ()
                    {
                        for (int i = 0; i < SnakeManager.SnakeCount; i++)
                        {
                            if (SnakeManager.Snakes[i] != gameObject)
                            {
                                SnakeManager.Snakes[i].GetComponent<SnakeMovement>().Confused = true;
                            }
                        }
                    };

                    tickBehaviour = delegate ()
                    {

                    };

                    endBehaviour = delegate ()
                    {
                        for (int i = 0; i < SnakeManager.SnakeCount; i++)
                        {
                            if (SnakeManager.Snakes[i] != gameObject)
                            {
                                SnakeManager.Snakes[i].GetComponent<SnakeMovement>().Confused = false;
                            }
                        }
                    };

                    timeLimit = 6f;

                    break;

                case "Blackout": // deactivate all other cameras

                    startBehaviour = delegate ()
                    {
                        for (int i = 0; i < SnakeManager.SnakeCount; i++)
                        {
                            if (SnakeManager.Snakes[i] != gameObject)
                            {
                                SnakeManager.Snakes[i].GetComponentInChildren<Canvas>().enabled = true;
                            }
                        }
                    };

                    tickBehaviour = delegate ()
                    {

                    };

                    endBehaviour = delegate ()
                    {
                        for (int i = 0; i < SnakeManager.SnakeCount; i++)
                        {
                            if (SnakeManager.Snakes[i] != gameObject)
                            {
                                SnakeManager.Snakes[i].GetComponentInChildren<Canvas>().enabled = false;
                            }
                        }
                    };

                    timeLimit = 7f;

                    break;

                case "SelfKill": // rework collision system to allow self collisions (possibly split head and body objects)

                    startBehaviour = delegate ()
                    {

                    };

                    tickBehaviour = delegate ()
                    {

                    };

                    endBehaviour = delegate ()
                    {

                    };

                    timeLimit = 6f;

                    break;

                default:
                    Debug.LogError("PowerupNotAssignedError");

                    startBehaviour = delegate () { };
                    tickBehaviour = delegate () { };
                    endBehaviour = delegate () { };
                    timeLimit = 0f;
                    break;
            }
        }
        else
        {
            switch (powerupName)
            {
                case "Magnet": // Increase pickup range

                    startBehaviour = delegate ()
                    {
                        _snake.PickupRadius *= 4;
                    };

                    tickBehaviour = delegate ()
                    {

                    };

                    endBehaviour = delegate ()
                    {
                        _snake.PickupRadius = SnakeManager.PickupRadius;
                    };

                    timeLimit = 8f;

                    break;

                case "TailReset": // Remove all tailpositions

                    startBehaviour = delegate ()
                    {
                        _snakeBody.SnakePositions.Clear();
                    };

                    tickBehaviour = delegate ()
                    {

                    };

                    endBehaviour = delegate ()
                    {

                    };

                    timeLimit = 0f;

                    break;

                case "Hax": // draw lines to all coins and players

                    startBehaviour = delegate ()
                    {

                    };

                    tickBehaviour = delegate ()
                    {

                    };

                    endBehaviour = delegate ()
                    {

                    };

                    timeLimit = 8f;

                    break;

                case "FarView": // move cam back

                    startBehaviour = delegate ()
                    {
                        GetComponentInChildren<CinemachineVirtualCamera>().m_Lens.OrthographicSize *= 2;
                    };

                    tickBehaviour = delegate ()
                    {

                    };

                    endBehaviour = delegate ()
                    {
                        GetComponentInChildren<CinemachineVirtualCamera>().m_Lens.OrthographicSize = SnakeManager.CameraSize;
                    };

                    timeLimit = 10f;

                    break;

                case "Teleport": // teleport to a random position until either there's no collider nearby or there's been 10 attempts

                    startBehaviour = delegate ()
                    {

                    };

                    tickBehaviour = delegate ()
                    {

                    };

                    endBehaviour = delegate ()
                    {

                    };

                    timeLimit = 0f;

                    break;

                default:
                    Debug.LogError("PowerupNotAssignedError");
                    startBehaviour = delegate () { };
                    tickBehaviour = delegate () { };
                    endBehaviour = delegate () { };
                    timeLimit = 0f;
                    break;
            }
        }

        return new Powerup(this, weak, powerupName, timeLimit, startBehaviour, tickBehaviour, endBehaviour);
    }

    void Update()
    {
        if (Input.GetKeyDown(WeakPowerupKey) && !_playerHand[1].IsActive && _snake.Coins >= 5)
        {
            UsePowerup(_playerHand[1], _weakSlot);
        }
        if ((Input.GetKeyDown(MainPowerupKey) || Input.GetKeyDown(MainPowerupAlt)) && !_playerHand[0].IsActive && _snake.Coins >= 10)
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
        _playerHand[0].UpdatePowerup(Time.fixedDeltaTime);
        _playerHand[1].UpdatePowerup(Time.fixedDeltaTime);
    }

    // places them appropiately on the player
    private void PlacePowerup(GameObject[] powerupGameObjects, Powerup[] playerHand, Vector3 primaryPowerupPosition, Vector3 secondaryPowerupPosition)
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
                    indexOfPowerup = Array.IndexOf(Powerups.ToArray(), playerHand[i].Name) - 1; // gets the index of the powerup (essentially an ID)
                    try
                    {
                        powerupGameObjects[i].GetComponent<SpriteRenderer>().sprite = _powerupSprites[indexOfPowerup]; // gets the sprite corresponding to that ID
                    }
                    catch
                    {
                        Debug.Log(indexOfPowerup + ", " + WeakPowerups.Length + ", " + playerHand[i]);
                    }
                }
                else
                {
                    indexOfPowerup = Array.IndexOf(WeakPowerups.ToArray(), playerHand[i].Name) - 1; // gets the index of the powerup (essentially an ID)
                    try
                    {
                        powerupGameObjects[i].GetComponent<SpriteRenderer>().sprite = _weakPowerupSprites[indexOfPowerup]; // gets the sprite corresponding to that ID
                    }
                    catch
                    {
                        Debug.Log(indexOfPowerup + ", " + Powerups.Length + ", " + playerHand[i]);
                    }
                }
            }
        }
    }

}
