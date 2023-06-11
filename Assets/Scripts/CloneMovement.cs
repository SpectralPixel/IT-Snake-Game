using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CloneMovement : MonoBehaviour
{

    [HideInInspector] public Vector2 Movement;
    [HideInInspector] public float MoveSpeed;

    private Rigidbody2D _rigidbody;
    private float _lifespan;


    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();

        MoveSpeed = SnakeManager.MoveSpeed;

        _lifespan = 0f;
    }

    public void SetVelocity(Vector2 velocity)
    {
        Movement = velocity;
    }

    private void FixedUpdate()
    {
        _rigidbody.MovePosition(_rigidbody.position + Movement * MoveSpeed * Time.fixedDeltaTime);

        if (_lifespan > 10f)
        {
            GameObject.Destroy(gameObject);
        }

        _lifespan += Time.fixedDeltaTime;
    }

}