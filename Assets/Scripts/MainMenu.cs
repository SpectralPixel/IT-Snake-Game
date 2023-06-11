using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    [SerializeField] private TMPro.TMP_Text _playerCountTxt;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            GameObject.Find("Player Count").GetComponent<Slider>().value--;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            GameObject.Find("Player Count").GetComponent<Slider>().value++;
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            GameObject.Find("Game Manager").GetComponent<GameManager>().StartGameButton();
        }
    }

    public void SliderChanged(float playerCount)
    {
        SnakeManager.SnakeCount = Mathf.RoundToInt(playerCount);
        _playerCountTxt.text = "Player Count: " + SnakeManager.SnakeCount.ToString();

        if (playerCount > 2) GameObject.Find("Keyboard Warning").GetComponent<TextMeshProUGUI>().enabled = true;
        else GameObject.Find("Keyboard Warning").GetComponent<TextMeshProUGUI>().enabled = false;

        GameObject.Find("Player Count").GetComponent<Slider>().value = SnakeManager.SnakeCount;
    }
}
