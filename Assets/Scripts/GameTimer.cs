using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{

    public float GameTime = Mathf.Infinity;

    [SerializeField] private TMP_Text _timerObj;
    [SerializeField] private TMP_Text _winConditionObj;

    [SerializeField] private Vector2[] _UIPositions;
    [SerializeField] private float _objectiveRelPos;

    [HideInInspector] public int WinCondition;
    private Snake _leader;

    private Snake[] _snakes;

    public void StartRound()
    {
        GameTime = 120f;
        WinCondition = Random.Range(0, 3);

        if (WinCondition == 0) _winConditionObj.text = "Person with the most points collected wins!";
        if (WinCondition == 1) _winConditionObj.text = "Person with the most coins collected wins!";
        if (WinCondition == 2) _winConditionObj.text = "Person with the most vanquishes wins!";
        if (WinCondition == 3) _winConditionObj.text = "Person with the most powerups used wins!";

        _timerObj.gameObject.SetActive(true);
        _winConditionObj.gameObject.SetActive(true);

        // SET TIMER POSTIOTION
        Vector3 timerPos = new Vector3(_UIPositions[SnakeManager.SnakeCount - 2].x, _UIPositions[SnakeManager.SnakeCount - 2].y, 0f);

        _timerObj.rectTransform.localPosition = timerPos;
        _winConditionObj.rectTransform.localPosition = new Vector3(timerPos.x, timerPos.y + _objectiveRelPos, timerPos.z);

        _snakes = new Snake[SnakeManager.SnakeCount];
        for (int i = 0; i < SnakeManager.SnakeCount; i++)
        {
            _snakes[i] = SnakeManager.Snakes[i].GetComponent<Snake>();
        }
        _leader = _snakes[0];

        InvokeRepeating("CheckForLeader", 0f, 0.2f);
    }

    void FixedUpdate()
    {
        GameTime -= Time.fixedDeltaTime;

        if (GameTime < 0 && GameManager.Instance.State == GameState.Round)
        {
            GameManager.Instance.UpdateGameState(GameState.Menu);
        }

        _timerObj.text = Mathf.CeilToInt(GameTime).ToString();
    }

    private void CheckForLeader()
    {
        for (int i = 0; i < _snakes.Length; i++)
        {
            if (_snakes[i].WinConditions[WinCondition] > _leader.WinConditions[WinCondition])
            {
                _leader = _snakes[i];
            }
        }

        for (int i = 0; i < _snakes.Length; i++)
        {
            _snakes[i].gameObject.transform.Find("Crown").gameObject.SetActive(false);
        }

        _leader.gameObject.transform.Find("Crown").gameObject.SetActive(true);
    }
}