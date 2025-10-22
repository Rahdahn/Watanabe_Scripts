using UnityEngine;

public class SummonedEnemy : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private bool isFlyingEnemy = false;

    private Transform player;
    private SpriteRenderer spriteRenderer;

    private Vector2 moveDirection;
    private float fixedY;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (player != null)
        {
            float dirX = (player.position.x - transform.position.x) > 0 ? 1 : -1;
            moveDirection = new Vector2(dirX, 0).normalized;
            spriteRenderer.flipX = dirX < 0;

            if (isFlyingEnemy)
            {
                fixedY = transform.position.y;
            }
        }
        else
        {
            moveDirection = Vector2.right;
        }
    }

    void Update()
    {
        if (isFlyingEnemy)
        {
            transform.position = new Vector2(
                transform.position.x + moveDirection.x * moveSpeed * Time.deltaTime,
                fixedY
            );
        }
        else
        {
            transform.position += (Vector3)(moveDirection * moveSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerHealth>()?.TakeDamage(1);
            Destroy(gameObject);
        }
        else if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
