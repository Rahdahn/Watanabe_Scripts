using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float stopDistance = 3f;
    [SerializeField] private float jumpForce = 7f;
    private bool isGrounded;

    private Transform player;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private bool isAttacking = false;

    [SerializeField] private GameObject magicBulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float minAttackDelay = 2f;
    [SerializeField] private float maxAttackDelay = 5f;
    private float attackTimer;

    [SerializeField] private GameObject flyingEnemyPrefab;
    [SerializeField] private GameObject groundEnemyPrefab;

    [SerializeField] private Transform[] flyingLeftSpawnPoints;
    [SerializeField] private Transform[] flyingRightSpawnPoints;
    [SerializeField] private Transform[] groundLeftSpawnPoints;
    [SerializeField] private Transform[] groundRightSpawnPoints;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        ResetAttackTimer();
    }

    void Update()
    {
        if (player == null) return;

        spriteRenderer.flipX = player.position.x < transform.position.x;

        float distance = Vector2.Distance(transform.position, player.position);

        if (!isAttacking)
        {
            if (distance > stopDistance)
            {
                Vector2 target = new Vector2(player.position.x, transform.position.y);
                transform.position = Vector2.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
                animator.SetBool("Run", true);
            }
            else
            {
                animator.SetBool("Run", false);
            }

            animator.SetBool("Fall", !isGrounded && rb.velocity.y < -0.1f);

            if (isGrounded && (player.position.y > transform.position.y + 1.5f || Random.value < 0.005f))
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                animator.SetTrigger("Jump");
                isGrounded = false;
            }
        }

        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f && !isAttacking)
        {
            TriggerRandomAttack();
            ResetAttackTimer();
        }
    }

    private void TriggerRandomAttack()
    {
        animator.ResetTrigger("MAttack");
        animator.ResetTrigger("SAttack");

        if (Random.value > 0.5f)
            animator.SetTrigger("MAttack");
        else
            animator.SetTrigger("SAttack");
    }

    private void ResetAttackTimer()
    {
        attackTimer = Random.Range(minAttackDelay, maxAttackDelay);
    }

    public void CastMagic()
    {
        isAttacking = true;
        if (magicBulletPrefab != null && firePoint != null)
        {
            Instantiate(magicBulletPrefab, firePoint.position, firePoint.rotation);
        }
    }

    public void SummonEnemy()
    {
        isAttacking = true;
        bool spawnFlying = Random.value > 0.5f;
        bool spawnRight = Random.value > 0.5f;

        Transform[] spawnPoints = spawnFlying ? (spawnRight ? flyingRightSpawnPoints : flyingLeftSpawnPoints) :
                                                (spawnRight ? groundRightSpawnPoints : groundLeftSpawnPoints);

        if (spawnPoints.Length == 0) return;

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject enemyPrefab = spawnFlying ? flyingEnemyPrefab : groundEnemyPrefab;

        GameObject newEnemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);

        SpriteRenderer sr = newEnemy.GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.flipX = spawnRight ? true : false;
    }

    public void EndAttack()
    {
        isAttacking = false;
        animator.ResetTrigger("MAttack");
        animator.ResetTrigger("SAttack");
    }

    public void SetGrounded(bool grounded)
    {
        isGrounded = grounded;
    }
}
