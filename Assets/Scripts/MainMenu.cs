using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
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
}
