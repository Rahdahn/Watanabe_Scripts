using UnityEngine;

public class MagicBullet : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    private Transform target;
    private Vector3 direction;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (target != null)
        {
            direction = (target.position - transform.position).normalized;
        }
        Destroy(gameObject, 5f);
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
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
