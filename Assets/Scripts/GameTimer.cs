using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{

    public float GameTime;

    [SerializeField] private TMP_Text _timerObj;
    [SerializeField] private TMP_Text _winConditionObj;

    public void StartRound()
    {
        GameTime = 120f;

        _timerObj.gameObject.SetActive(true);
        _winConditionObj.gameObject.SetActive(true);
    }

    void FixedUpdate()
    {
        GameTime -= Time.fixedDeltaTime;

        if (GameTime < 0 && GameManager.Instance.State == GameState.Round) GameManager.Instance.UpdateGameState(GameState.Menu);

        _timerObj.text = Mathf.CeilToInt(GameTime).ToString();
    }
}