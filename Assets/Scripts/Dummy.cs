using UnityEngine;

public class Dummy : MonoBehaviour
{
    [SerializeField] private int _dummyID;

    private void Start()
    {
        UpdateDummy();
    }

    public void UpdateDummy()
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        LineRenderer lineRenderer = GetComponent<LineRenderer>();

        if (_dummyID < SnakeManager.SnakeCount)
        {
            renderer.enabled = true;
            lineRenderer.enabled = true;

            Color snakeStartColor = Color.HSVToRGB(_dummyID * (1f / SnakeManager.SnakeCount), 0.2f, 1.0f, false);
            Color snakeEndColor = Color.HSVToRGB(_dummyID * (1f / SnakeManager.SnakeCount), 1f, 0.5f, false);
            lineRenderer.colorGradient = CreateSimpleGradient(snakeStartColor, snakeEndColor, true);

            renderer.color = snakeStartColor;
            renderer.sprite = SnakeManager.ActiveSnakeSkins[_dummyID];

            gameObject.transform.Find("Canvas").GetComponent<Canvas>().enabled = true;

            if (_dummyID == SnakeManager.LeadingPlayerID) gameObject.transform.Find("Crown").GetComponent<SpriteRenderer>().enabled = true;
            else gameObject.transform.Find("Crown").GetComponent<SpriteRenderer>().enabled = false;
        }
        else
        {
            renderer.enabled = false;
            lineRenderer.enabled = false;
            gameObject.transform.Find("Canvas").GetComponent<Canvas>().enabled = false;
        }
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
}
