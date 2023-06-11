using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EdgeCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(LineRenderer))]
public class CloneBody : MonoBehaviour
{

    [HideInInspector] public GameObject Host;

    private EdgeCollider2D _edgeCollider;
    private SpriteRenderer _spriteRenderer;
    private LineRenderer _lineRenderer;

    [HideInInspector] public List<Vector3> SnakePositions;
    [HideInInspector] public uint SnakeLength;

    private void Start()
    {
        _edgeCollider = GetComponent<EdgeCollider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _lineRenderer = GetComponent<LineRenderer>();

        SnakeLength = SnakeManager.DefaultSnakeLength;

        SnakePositions = new List<Vector3>();

        InvokeRepeating("CreateBody", 0f, 0.2f);
    }

    public void SetAppearance(Sprite sprite, Color color, Gradient gradient)
    {
        _spriteRenderer.sprite = sprite;
        _spriteRenderer.color = color;
        _lineRenderer.colorGradient = gradient;
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
}