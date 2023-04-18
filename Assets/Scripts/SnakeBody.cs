using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EdgeCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(LineRenderer))]
public class SnakeBody : MonoBehaviour
{

    private Snake _snake;
    private SnakeMovement _snakeMovement;
    private SnakeHand _snakeHand;

    private EdgeCollider2D _edgeCollider;
    private SpriteRenderer _spriteRenderer;
    private LineRenderer _lineRenderer;

    [HideInInspector] public List<Vector3> SnakePositions;
    [HideInInspector] public uint PointsToGrow;
    [HideInInspector] public uint SnakeLength;

    private void Start()
    {
        _snake = GetComponent<Snake>();
        _snakeMovement = GetComponent<SnakeMovement>();
        _snakeHand = GetComponent<SnakeHand>();

        _edgeCollider = GetComponent<EdgeCollider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _lineRenderer = GetComponent<LineRenderer>();

        SnakeLength = SnakeManager.DefaultSnakeLength;
        PointsToGrow = SnakeManager.PointsToGrow;

        SetPlayerColor();

        SnakePositions = new List<Vector3>();

        InvokeRepeating("CreateBody", 0f, 0.2f);
    }

    private void Update()
    {
        SetEdgeCollider();
    }

    private void FixedUpdate()
    {
        Vector3 currentPosition = new Vector3(transform.position.x, transform.position.y, 0f);
        SnakePositions.Add(currentPosition);
        _lineRenderer.positionCount = SnakePositions.Count;
        _lineRenderer.SetPositions(SnakePositions.ToArray());
        SnakePositions.Remove(currentPosition);
    }

    public void CreateBody()
    {
        SnakePositions.Add(new Vector3(transform.position.x, transform.position.y, 0f));

        while (SnakePositions.Count > SnakeLength)
        {
            SnakePositions.RemoveAt(0);
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

    // RUN ON AWAKE
    private void SetPlayerColor()
    {
        Color snakeStartColor = Color.HSVToRGB(_snake.PlayerID * (1f / _snake.PlayerCount), 1f, 1.0f, false);
        Color snakeEndColor = Color.HSVToRGB(_snake.PlayerID * (1f / _snake.PlayerCount), 1f, 0.5f, false);
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
}