using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CloneMovement : MonoBehaviour
{

    [HideInInspector] public Vector2 SpawnVelocity;
    [HideInInspector] public Vector2 Movement;
    [HideInInspector] public float MoveSpeed;

    private Rigidbody2D _rigidbody;


    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();

        MoveSpeed = SnakeManager.MoveSpeed;

        Movement = SpawnVelocity;
    }

    private void FixedUpdate()
    {
        _rigidbody.MovePosition(_rigidbody.position + Movement * MoveSpeed * Time.fixedDeltaTime);

        if (transform.position.x < GameManager.Instance.MinimumPositionX || transform.position.x > GameManager.Instance.MaximumPositionX || transform.position.y < GameManager.Instance.MinimumPositionY || transform.position.y > GameManager.Instance.MaximumPositionY)
        {
            GameObject.Destroy(gameObject);
        }
    }

}